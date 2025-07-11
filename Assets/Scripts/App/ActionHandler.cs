
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Handles controls and <see cref="Action"/> related parts of app.
    /// <br/>   
    /// <br/>   Takes care of:
    /// <br/>   • controls initialization based on each action's <see cref="Shortcut"/>
    /// <br/>   • action creation, updating and disposal
    /// <br/>   • shortcut rebinding
    /// </summary>
    public class ActionHandler
    {
        /// <summary>
        /// <br/>   Currently active long actions.
        /// <br/>   Only one long action of a given type can be active at once.
        /// <br/>   Mouse position is also captured before starting a long action.
        /// </summary>
        public Dictionary<Type, (ILong longAction, Vector2 mouseStartPosition)> ActiveLongActions { get; private set; } = new();


        // Keep track of actions to execute based on their shortcut
        // Lower priority actions will be disgarded if higher priority ones succeeded
        private Dictionary<Shortcut, Queue<(ActionInfo info, int priority)>> actionQueues = new();


        #region Public Methods ============================================================================== Public Methods

        /// <summary> Executes each highest priority <see cref="Action"/> in action queue. </summary>
        public void ProcessQueue()
        {
            // Execute highest priority actions from each queue
            foreach ((Shortcut shortcut, Queue<(ActionInfo info, int priority)> queue) in actionQueues)
            {
                queue.OrderByDescending(queuedAction => queuedAction.priority);

                // Go through each action in queue until one succeeds or begins successfully
                while (queue.Count > 0)
                {
                    ActionInfo info = queue.Dequeue().info;

                    Debug.Log(App.CurrentContext + " / " + info.Context);

                    if (info.Context != App.CurrentContext) { continue; }

                    Action action = (Action)Activator.CreateInstance(info.ActionType);

                    if (info.IsShort && ((IShort)action).Do()) { break; }
                    if (info.IsLong && ((ILong)action).Begin())
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
