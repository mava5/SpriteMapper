
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;


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

        | Despite there being no conflict, CBF instant action gets executed on shortcut release.
        | This happens because CBF setting forces action to act as if in a solvable conflict.
        | For an instant action this means being executed when shortcut is released.
        |
        | Note: In the case of an actual solvable conflict, CBF setting doesn't have an effect.
        |
        | [Instant, CBF] ConflictInstant → Shortcut is released.
        | or
        | [Held, DeadZone, CBF] ConflictHeldZone → Mouse exits dead zone
        */


        /// <summary>
        /// <br/>   Currently active long <see cref="Action"/>, which captures the context.
        /// <br/>   While active, only actions within its specified context can be used.
        /// <br/>   Also <see cref="ActionDescendantUsability.Full"/> actions from parent contexts can be used.
        /// <br/>   
        /// <br/>   Starting new such action will have the previous one cancelled.
        /// <br/>   Start mouse position is also trasnferred from the cancelled action.
        /// </summary>
        public Action ActiveContextCapturingLongAction { get; private set; } = null;

        /// <summary>
        /// <br/>   Each currently active long <see cref="Action"/>, which doesn't capture context.
        /// <br/>   These actions get cancelled whenever the context changes.
        /// </summary>
        public Dictionary<Type, Action> ActiveLongActions { get; private set; } = new();


        // Keep track of actions to execute based on their shortcut
        private Dictionary<Shortcut, Queue<ActionInfo>> actionQueues = new();

        // An unresolved input is created if a solvable conflict is caused by shortcut
        // Each unresolved input is then evaluated each frame
        private Dictionary<Shortcut, (float startTime, Vector2 startPosition,
            List<ActionInfo> deadZoneActions,
            List<ActionInfo> timerActions,
            List<ActionInfo> fallbackActions)>
            unresolvedInputs = new();


        #region Public Methods ============================================================================== Public Methods

        /// <summary> Finishes unresolved input if one is ongoing for given shortcut. </summary>
        public void ReleaseInput(Shortcut shortcut)
        {
            if (unresolvedInputs.TryGetValue(shortcut, out var input))
            {

            }
        }

        /// <summary>
        /// <br/>   Iterates through unresolved inputs and evaluates them.
        /// <br/>   Executes action and finishes unresolved input if action specific conditions are met.
        /// </summary>
        public void ProcessUnresolvedInputs()
        {
            foreach (var input in unresolvedInputs)
            {

            }
        }

        /// <summary>
        /// <br/>   Goes through each <see cref="Action"/> queued based on a <see cref="Shortcut"/>.
        /// <br/>   Executes or begins an action, or starts an ongoing input based on conflicts and action behaviours.
        /// </summary>
        public void ProcessQueue()
        {
            foreach ((Shortcut shortcut, Queue<ActionInfo> queue) in actionQueues)
            {
                List<ActionInfo> prioritizedActions = new();
                List<ActionInfo> pressActivatedActions = new();
                List<ActionInfo> deadZoneActions = new();
                List<ActionInfo> timerActions = new();

                bool oneOrMoreCBF = false;

                foreach (ActionInfo info in queue)
                {
                    if (info.Settings.DescendantUsability == ActionDescendantUsability.None &&
                        info.Context != App.CurrentContext) { continue; }

                    if (info.Settings.DescendantUsability == ActionDescendantUsability.Limited &&
                        !info.Context.StartsWith(App.CurrentContext)) { continue; }

                    if (info.Settings.DescendantUsability != ActionDescendantUsability.Full &&
                        ActiveContextCapturingLongAction != null) { continue; }


                    if (!oneOrMoreCBF && info.Settings.ConflictBehaviourForced) { oneOrMoreCBF = true; }

                    if (info.Settings.ExecutionPrioritized) { prioritizedActions.Add(info); }
                    else if (info.Behaviour == ActionBehaviourType.Instant) { pressActivatedActions.Add(info); }
                    else if (info.Behaviour == ActionBehaviourType.Toggle) { pressActivatedActions.Add(info); }
                    else if (info.Behaviour == ActionBehaviourType.Hold)
                    {
                        HoldActionSettings holdSettings = (HoldActionSettings)info.Settings;

                        if (holdSettings.ConflictBehaviour == HoldActionResolving.UseDeadZone)
                        {
                            deadZoneActions.Add(info);
                        }
                        else if (holdSettings.ConflictBehaviour == HoldActionResolving.UseTimer)
                        {
                            timerActions.Add(info);
                        }
                    }
                }


                // Try to execute prioritized actions first
                if (prioritizedActions.Count > 0)
                {
                    prioritizedActions = prioritizedActions.OrderBy(info => info.ActionType.FullName).ToList();

                    (bool succeeded, bool contextChanged) = TryExecuteActions(prioritizedActions);

                    if (contextChanged) { return; }
                    if (succeeded) { continue; }
                }


                int uniqueActionTypes =
                    (pressActivatedActions.Count > 0 ? 1 : 0) +
                    (deadZoneActions.Count > 0 ? 1 : 0) +
                    (timerActions.Count > 0 ? 1 : 0);

                if (uniqueActionTypes == 0) { continue; }


                if (uniqueActionTypes == 1)
                {
                    if (pressActivatedActions.Count > 0)
                    {
                        pressActivatedActions = pressActivatedActions.OrderBy(info => info.ActionType.FullName).ToList();

                        (bool succeeded, bool contextChanged) = TryExecuteActions(pressActivatedActions);

                        if (contextChanged) { return; }
                        if (succeeded) { continue; }

                        if (oneOrMoreCBF)
                        {
                            unresolvedInputs.Add(shortcut,
                                (
                                    Time.time, Input.mousePosition,
                                    null, null, 
                                ));
                        }
                    }
                    else if (deadZoneActions.Count > 0)
                    {
                        (bool succeeded, bool contextChanged) = TryExecuteActions(pressActivatedActions);

                        if (contextChanged) { return; }
                        if (succeeded) { continue; }
                    }
                    else if (pressActivatedActions.Count > 0)
                    {
                        (bool succeeded, bool contextChanged) = TryExecuteActions(pressActivatedActions);

                        if (contextChanged) { return; }
                        if (succeeded) { continue; }
                    }
                }


                if (uniqueActionTypes > 1)
                {
                    unresolvedInputs.Add(shortcut,
                        (
                            Time.time, Input.mousePosition,
                            prioritizedActions,
                            deadZoneActions,
                            timerActions,
                            pressActivatedActions
                        ));

                    continue;
                }
            }
            actionQueues.Clear();


            // Execute highest priority actions from each queue
            foreach ((Shortcut shortcut, Queue<ActionInfo> queue) in actionQueues)
            {
                if (queue.Count == 1)
                {

                }

                // Get hold action count
                int holdCount = 0;
                foreach (var item in queue)
                {
                    
                }





                queue = queue.OrderByDescending(queuedAction => queuedAction.priority);

                // Go through each action in queue until one succeeds or begins successfully
                while (queue.Count > 0)
                {
                    ActionInfo info = queue.Dequeue().info;

                    Debug.Log(App.CurrentContext + " / " + info.Context);

                    if (info.Context != App.CurrentContext) { continue; }


                    Action action = (Action)Activator.CreateInstance(info.ActionType);

                    if (info.IsShort && ((IShort)action).Do()) { break; }
                    if (info.IsLong && ((Ilong)action).Begin())
                    {
                        // Cancel currently active long action if there is one
                        if (ActiveLongActions.ContainsKey(info.ActionType))
                        {
                            ActiveLongActions[info.ActionType].longAction.Cancel();
                            ActiveLongActions.Remove(info.ActionType);
                        }

                        ActiveLongActions.Add(info.ActionType, ((ILong)action, Input.mousePosition));
                        break;
                    }
                }
            }
            actionQueues.Clear();
        }

        /// <summary> Updates each active <see cref="ILong"/> <see cref="Action"/> and re-evaluates them. </summary>
        public void UpdateLongActions()
        {
            // Evaluate currently active long actions
            List<Type> longActionsToRemove = new();
            foreach ((ILong action, Vector2 _) in ActiveLongActions.Values)
            {
                if (action != null)
                {
                    if (action.CancelPredicate)
                    {
                        action.Cancel();
                        longActionsToRemove.Add(action.GetType());
                    }
                    else if (action.EndPredicate)
                    {
                        action.End();
                        longActionsToRemove.Add(action.GetType());
                    }
                    else
                    {
                        action.Update();
                    }
                }
            }

            foreach (Type actionType in longActionsToRemove) { ActiveLongActions.Remove(actionType); }
        }


        /// <summary> Queues up <see cref="Action"/> based on given type. </summary>
        /// <param name="shortcutOverride"> Shortcut used to execute action if not the action's own one. </param>
        /// <param name="highPriority"> true = Action prioritized in queue <br/> false = Uses priority from <see cref="ActionInfo"/> </param>
        public void AddToQueue<T>(Shortcut shortcutOverride = null, bool highPriority = false) where T : Action
        {
            AddToQueue(HierarchyInfoDictionary.ActionInfos[typeof(T)], shortcutOverride, highPriority);
        }

        /// <summary> Queues up <see cref="Action"/> based on given type. </summary>
        /// <param name="shortcutOverride"> Shortcut used to execute action if not the action's own one. </param>
        /// <param name="highPriority"> true = Action prioritized in queue <br/> false = Uses priority from <see cref="ActionInfo"/> </param>
        public void AddToQueue(Type actionType, Shortcut shortcutOverride = null, bool highPriority = false)
        {
            AddToQueue(HierarchyInfoDictionary.ActionInfos[actionType], shortcutOverride, highPriority);
        }

        /// <summary> Queues up <see cref="Action"/> based on given info. </summary>
        /// <param name="shortcutOverride"> Shortcut used to execute action if not the action's own one. </param>
        /// <param name="highPriority"> true = Action prioritized in queue <br/> false = Uses priority from <see cref="ActionInfo"/> </param>
        public void AddToQueue(ActionInfo actionInfo, Shortcut shortcutOverride = null, bool highPriority = false)
        {
            Shortcut shortcut = shortcutOverride ?? actionInfo.DefaultShortcut1;
            int priority = highPriority ? int.MaxValue : (int)actionInfo.Priority;

            actionQueues.TryAdd(shortcut, new());
            actionQueues[shortcut].Enqueue((actionInfo, priority));
        }

        #endregion Public Methods
    }
}
