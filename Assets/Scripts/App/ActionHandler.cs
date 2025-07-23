
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor.ShaderGraph;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;


namespace SpriteMapper
{
    /// <summary> Handles <see cref="Action"/> related parts of app. </summary>
    public class ActionHandler
    {
        /*
        Multiple actions can share the same shortcut and thus can create conflicts.
        These conflicts are handled based on the different actions' behaviours.
        Even in case of a conflict, only one action will ever be executed per shortcut input.

        Solvable conflict:
        All conflicting actions have different behaviours:
        → Any conflicting action can be performed based on how the user finishes the input.

        Overwriting conflict:
        Some conflicting actions have same behaviours:
        → Conflicting actions with same behaviours are iterated through alphabetically.
          The iterating ends when one of these conflicting actions is successfully executed.
          In other words, some conflicting actions in this case may never be executed.
        
        Base behaviours:
        • [Pressed, short]  Instant
        • [Pressed, long]   Toggle
        • [Held, long]      Hold
        
        Behaviour modifiers:
        • Priority (any)
        • ConflictBehaviourForced (any)
        • DeadZone / Timer (Hold)

        Note: While toggle actions share characteristics of both instant and held actions,
              they're ultimately considered instants actions, as they begin
              the same way instant actions do – with a shortcut press.

        Note: Every held action gets executed either based on a dead zone or timer when there is conflicts.
              Whether a held action uses one or the other is irrelevant however when it has priority.
              The prioritized held action will be executed first granted there aren't other prioritized actions.



        Example cases:
        ╔═╗ = Solvable conflict
        ┌─┐ = Overwriting conflict
        CBF = ConflictBehaviourForced
        → = "...is executed if..."


        | Two similar actions create an overwriting conflict.
        | The two are iterated through alpabetically, so InstantB will
        | only ever get a chance to execute if InstantA isn't successful:
        |
        | ┌ [Instant] InstantA → Shortcut is pressed.
        | └ [Instant] InstantB → InstantA failed.


        | Multiple different types of actions create a solvable conflict.
        | Any of the unique action types get a chance to execute based on how user handles input.
        | However, in this case two instant actions also create an overwriting conflict.
        | 
        | ╔ [Hold, DeadZone] HoldZone → Mouse exits circular dead zone while shortcut is held down.
        | ║ [Hold, Timer]    HoldTime → User keeps holding shortcut after specific amount of time.
        | ║ ┌ [Instant]      Instant1 → Shortcut is released within dead zone in time.
        | ╚ └ [Instant]      Instant2 → Instant1 failed.
        

        | Prioritized actions are executed first.
        | When multiple prioritized actions are present, they are always in a overwriting conflict.
        | This means that each prioritized action is iterated through in an alphabetic order.
        | If no prioritized actions succeed, non prioritized actions are tried to execute next.
        |
        | ┌ [Hold, Timer, Priority]     HoldTimePriority → Shortcut is pressed.
        | │ [Hold, DeadZone, Priority]  HoldZonePriority → HoldTimePriority failed.
        | │ [Instant, Priority]         InstantPriority  → HoldZonePriority failed.
        | └ [Toggle, Priority]          TogglePriority   → InstPriority failed.
        | ┌ [Instant]                   A_Instant        → Prioritized actions failed.
        | └ [Toggle]                    B_Toggle         → A_Instant failed.


        | Despite there being no conflict, CBF action gets executed based on its conflict behaviour.
        | This happens because CBF setting forces action to act as if it's in a solvable conflict.
        | For an instant action this means being executed when shortcut is released.
        |
        | [Instant, CBF] ConflictInstant → Shortcut is released.
        | or
        | [Hold, DeadZone, CBF] ConflictHoldZone → Mouse exits dead zone


        | In the case of an actual solvable conflict, CBF setting doesn't have an effect.
        | 
        | ╔ [Hold, Timer]    HoldTime  → User keeps holding shortcut after specific amount of time.
        | ║ ┌ [Instant, CBF] A_Instant → Shortcut is released in time.
        | ╚ └ [Instant]      B_Instant → A_Instant failed.


        | CBF actions cause an overwriting conflict with other similar type of actions.
        | The CBF action has to deal with both the overwriting and its conflict behaviour imposed limitations.
        | 
        | ┌ [Instant] NormalInstant   → Shortcut is pressed.
        | └ [Instant, CBF] CBFInstant → NormalInstant failed and shortcut is released.
        | or
        | ┌ [Hold, Timer]      NonCBFHold   → Shortcut is pressed.
        | └ [Hold, Timer, CBF] CBFTimerHold → NonCBFHold failed and user keeps holding shortcut after some time.


        | Prioritizing a CBF action will only make it perform before any other similar CBF actions.
        | The action will still act as if in a solvable conflict.
        |
        | ┌ [Instant, Prioritized, CBF] B_Instant → Shortcut is released.
        | └ [Instant, CBF]              A_Instant → B_Instant failed.
        */


        /// <summary>
        /// <br/>   Currently active <see cref="LongAction"/>, which overwrites the context.
        /// <br/>   While active, only actions within its specified context can be used.
        /// <br/>   Also <see cref="ActionDescendantUsability.Full"/> actions from parent contexts can be used.
        /// <br/>   
        /// <br/>   Starting new such action will have the previous one cancelled.
        /// <br/>   Start mouse position is also transferred from the cancelled action.
        /// </summary>
        public LongAction ActiveContextOverwritingLongAction { get; private set; } = null;


        /// <summary>
        /// <br/>   Each currently active <see cref="LongAction"/>, which doesn't overwrite context.
        /// <br/>   Only one long action can be active at once for any <see cref="Shortcut"/>.
        /// <br/>   These actions get cancelled whenever the context changes.
        /// </summary>
        private Dictionary<Type, LongAction> activeLongActions = new();

        /// <summary> Keeps track of actions to execute based on their shortcut. </summary>
        private Dictionary<Shortcut, Queue<ActionInfo>> actionQueues = new();

        /// <summary>
        /// <br/>   An unresolved input is created if a solvable conflict is caused by shortcut
        /// <br/>   Each unresolved input is then evaluated each frame
        /// <br/>   Unresolved inputs get dropped when context changes
        /// </summary>
        private Dictionary<Shortcut, (float startTime, Vector2 mouseStartPosition,
            List<ActionInfo> releaseActionInfos,
            List<ActionInfo> deadZoneActionInfos,
            List<ActionInfo> timerActionInfos)>
            unresolvedInputs = new();


        #region Public Methods ================================================================================================== Public Methods

        /// <summary> Finishes unresolved input if one is ongoing for given shortcut. </summary>
        public void ReleaseInput(Shortcut shortcut)
        {
            if (unresolvedInputs.TryGetValue(shortcut, out var input))
            {
                (bool succeeded, bool contextChanged) = TryExecuteActions(input.fallbackActionInfos);

                if (contextChanged) { OnContextChanged(); }
                else { unresolvedInputs.Remove(shortcut); }
            }
        }

        /// <summary>
        /// <br/>   Iterates through unresolved inputs and evaluates them.
        /// <br/>   Executes <see cref="Action"/> and finishes unresolved input if action specific conditions are met.
        /// </summary>
        public void ProcessUnresolvedInputs()
        {
            bool contextChanged = false;
            List<Shortcut> resolvedInputs = new();
            
            foreach ((Shortcut shortcut, var input) in unresolvedInputs)
            {
                bool succeeded = false;

                if (input.deadZoneActionInfos.Count > 0)
                {
                    float distance = ((Vector2)Input.mousePosition - input.mouseStartPosition).magnitude;

                    if (distance < Preferences.HoldDeadZoneRadius) { continue; }

                    (succeeded, contextChanged) = TryExecuteActions(input.deadZoneActionInfos);
                }

                if (!succeeded && input.timerActionInfos.Count > 0)
                {
                    if (Time.time < input.startTime + Preferences.HoldTimerLength) { continue; }

                    (succeeded, contextChanged) = TryExecuteActions(input.timerActionInfos);
                }

                if (contextChanged) { break; }
                if (succeeded) { resolvedInputs.Add(shortcut); }
            }


            if (contextChanged) { OnContextChanged(); return; }
            
            foreach (Shortcut shortcut in resolvedInputs) { unresolvedInputs.Remove(shortcut); }
        }


        /// <summary>
        /// <br/>   Goes through each <see cref="Action"/> queued based on a <see cref="Shortcut"/>.
        /// <br/>   Executes or begins an action, or starts an ongoing input based on conflicts and action behaviours.
        /// </summary>
        public void ProcessQueue()
        {
            float time = Time.time;
            Vector2 mousePosition = Input.mousePosition;
            bool contextChanged = false;

            foreach ((Shortcut shortcut, Queue<ActionInfo> queue) in actionQueues)
            {
                List<ActionInfo> prioritizedInfos = new();
                List<ActionInfo> pressActivatedInfos = new();
                List<ActionInfo> deadZoneInfos = new();
                List<ActionInfo> timerInfos = new();

                foreach (ActionInfo info in queue)
                {
                    if (!info.IsExecutableInContext(App.CurrentContext, ActiveContextOverwritingLongAction != null)) { continue; }

                    if (info.Settings.Prioritized && !info.Settings.ConflictBehaviourForced) { prioritizedInfos.Add(info); }
                    else if (info.Behaviour == ActionBehaviourType.Instant) { pressActivatedInfos.Add(info); }
                    else if (info.Behaviour == ActionBehaviourType.Toggle) { pressActivatedInfos.Add(info); }
                    else if (info.Behaviour == ActionBehaviourType.Hold)
                    {
                        HoldActionSettings holdSettings = (HoldActionSettings)info.Settings;

                        if (holdSettings.ConflictBehaviour == HoldActionResolving.UseDeadZone) { deadZoneInfos.Add(info); }
                        else if (holdSettings.ConflictBehaviour == HoldActionResolving.UseTimer) { timerInfos.Add(info); }
                    }
                }


                bool succeeded = false;
                (succeeded, contextChanged) = TryExecuteActions(prioritizedInfos);

                // Rest of inputs are ignored upon context change as they might cause undefined behaviour in new context
                if (contextChanged) { break; }
                if (succeeded) { continue; }


                int uniqueActionTypes =
                    (pressActivatedInfos.Count > 0 ?  1 : 0) +
                    (deadZoneInfos.Count > 0 ?        1 : 0) +
                    (timerInfos.Count > 0 ?           1 : 0);

                if (uniqueActionTypes == 1)
                {
                    List<ActionInfo> normalInfos = null;
                    List<ActionInfo> cbfInfos = null;

                    if (pressActivatedInfos.Count > 0) { (normalInfos, cbfInfos) = GetSortedActionInfos(pressActivatedInfos); }
                    else if (deadZoneInfos.Count > 0) { (normalInfos, cbfInfos) = GetSortedActionInfos(deadZoneInfos); }
                    else if (timerInfos.Count > 0) { (normalInfos, cbfInfos) = GetSortedActionInfos(timerInfos); }

                    (succeeded, contextChanged) = TryExecuteActions(normalInfos);

                    if (contextChanged) { break; }
                    if (!succeeded && cbfInfos.Count > 0)
                    {
                        if (pressActivatedInfos.Count > 0) { unresolvedInputs.Add(shortcut, (time, mousePosition, cbfInfos, null, null)); }
                        else if (deadZoneInfos.Count > 0) { unresolvedInputs.Add(shortcut, (time, mousePosition, null, cbfInfos, null)); }
                        else if (timerInfos.Count > 0) { unresolvedInputs.Add(shortcut, (time, mousePosition, null, null, cbfInfos )); }
                    }
                }
                else if (uniqueActionTypes > 1)
                {
                    unresolvedInputs.Add(shortcut, (time, mousePosition, deadZoneInfos, timerInfos, pressActivatedInfos));
                }
            }
            actionQueues.Clear();


            if (contextChanged) { OnContextChanged(); }
        }

        /// <summary> Updates each active <see cref="LongAction"/>. </summary>
        public void UpdateLongActions()
        {
            foreach (LongAction action in activeLongActions.Values) { action.Update(); }
        }


        /// <summary> Queues up <see cref="Action"/> based on given info. </summary>
        /// <param name="shortcutOverride"> Shortcut used to execute action. Overrides the action's own one. </param>
        public void AddToQueue(ActionInfo actionInfo, Shortcut shortcutOverride = null)
        {
            Shortcut shortcut = shortcutOverride ?? actionInfo.Shortcut;

            actionQueues.TryAdd(shortcut, new());
            actionQueues[shortcut].Enqueue(actionInfo);
        }

        #endregion Public Methods


        #region Private Methods ================================================================================================= Private Methods

        /// <summary>
        /// <br/>   Alphabetically sorts given <see cref="ActionInfo"/> list into normal and CBF lists.
        /// <br/>   Prioritized actions are also put at the front of the two lists.
        /// </summary>
        private (List<ActionInfo> normalInfos, List<ActionInfo> cbfInfos) GetSortedActionInfos(List<ActionInfo> infos)
        {
            int cbfCount = 0;
            List<ActionInfo> sortedActionInfos = infos.
                OrderBy(info =>
                {
                    if (info.Settings.ConflictBehaviourForced) { cbfCount++; }
                    return !info.Settings.ConflictBehaviourForced;
                }).
                ThenBy(info => info.Settings.Prioritized).
                ThenBy(info => info.ActionType.FullName).ToList();

            List<ActionInfo> normalActionInfos = sortedActionInfos.GetRange(0, cbfCount);
            List<ActionInfo> cbfActionInfos = sortedActionInfos.GetRange(cbfCount, sortedActionInfos.Count - cbfCount);

            return (normalActionInfos, cbfActionInfos);
        }


        /// <summary> Goes through given <see cref="ActionInfo"/> list until one succeeds. </summary>
        private (bool succeeded, bool contextChanged) TryExecuteActions(List<ActionInfo> infos)
        {
            bool succeeded = false;
            string startContext = App.CurrentContext;
            ActionInfo successfulInfo = null;

            foreach (ActionInfo info in infos)
            {
                if (info.Settings.Duration == ActionDuration.Short) { succeeded = TryExecuteShortAction(info); }
                else if (info.Settings.Duration == ActionDuration.Long) { succeeded = TryExecuteLongAction(info); }

                if (succeeded) { successfulInfo = info; break; }
            }

            return (succeeded, startContext != App.CurrentContext);
        }

        private bool TryExecuteShortAction(ActionInfo info)
        {
            ShortAction shortAction = (ShortAction)Activator.CreateInstance(info.ActionType);

            return shortAction.Do();
        }

        private bool TryExecuteLongAction(ActionInfo info)
        {
            LongAction longAction = (LongAction)Activator.CreateInstance(info.ActionType);
            bool isContextOverwriting = ((LongActionSettings)info.Settings).ContextUsedWhenActive != "";
            Vector2 mouseStartPosition = Input.mousePosition;

            if (isContextOverwriting && ActiveContextOverwritingLongAction != null)
            {
                mouseStartPosition = ActiveContextOverwritingLongAction.MouseStartPosition;
            }

            if (!longAction.Begin(mouseStartPosition)) { return false; }


            if (isContextOverwriting)
            {
                ActiveContextOverwritingLongAction?.Cancel();
                ActiveContextOverwritingLongAction = longAction;
            }
            else
            {
                if (activeLongActions.ContainsKey(info.ActionType))
                {
                    activeLongActions[info.ActionType].Cancel();
                    activeLongActions.Remove(info.ActionType);
                }

                activeLongActions.Add(info.ActionType, longAction);
            }

            return true;
        }


        /// <summary>
        /// <br/>   Cancels each <see cref="LongAction"/> and unresolved input.
        /// <br/>   This is done to prevent undefined behaviour with actions started in old context.
        /// </summary>
        private void OnContextChanged()
        {
            Debug.Log("Context changed: " + App.CurrentContext);

            // Active context overwriting long action is only cancelled if it didn't begin this frame
            // This is done so that the action wont immediately be cancelled after it overwrites the context
            if (ActiveContextOverwritingLongAction != null &&
                !ActiveContextOverwritingLongAction.BeganThisFrame)
            {
                ActiveContextOverwritingLongAction.Cancel();
                ActiveContextOverwritingLongAction = null;
            }

            List<Type> longActionsToRemove = new();
            foreach ((Type type, LongAction longAction) in activeLongActions)
            {
                longAction.Cancel();
                longActionsToRemove.Add(type);
            }

            foreach (Type actionType in longActionsToRemove) { activeLongActions.Remove(actionType); }

            unresolvedInputs.Clear();
        }

        #endregion Private Methods
    }
}
