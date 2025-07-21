
using System;


namespace SpriteMapper
{
    /// <summary> Determines if <see cref="Action"/> has a <see cref="Shortcut"/> and in what state it is. </summary>
    public enum ActionShortcutState
    {
        /// <summary> Action has no shortcut. </summary>
        None,

        /// <summary> Action has a shortcut. </summary>
        Exists,

        /// <summary> Action has a shortcut but it's locked. </summary>
        Locked,
    }

    /// <summary> Determines in what descendant contexts <see cref="Action"/> can be used. </summary>
    public enum ActionDescendantUsability
    {
        /// <summary> Action is only usable within its context </summary>
        None,

        /// <summary>
        /// <br/>   Action can be used in most descendant contexts.
        /// <br/>   Not usable in descendant long actions' context.
        /// </summary>
        Limited,

        /// <summary> Action can be used in all descendant contexts. </summary>
        Full,
    }


    /// <summary>
    /// <br/>   Contains mandatory base settings for an <see cref="Action"/>.
    /// <br/>   Inherited with more specific information for each <see cref="ActionBehaviourType"/>.
    /// <br/>   Further specifies the behaviour of an action.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ActionSettings : Attribute
    {
        public abstract ActionInputType InputType { get; }
        public abstract ActionDuration Duration { get; }

        /// <summary>
        /// <br/>   Determines if action always behaves as if in a solvable <see cref="Shortcut"/> conflict.
        /// <br/>   
        /// <br/>   Differnet action types behave in the following way in a solvable conflict:
        /// <br/>   • <see cref="InstantAction"/>: Only executes on shortcut release
        /// <br/>   • <see cref="ToggleAction"/>: Only begins execution on shortcut release
        /// <br/>   • [Timer] <see cref="HoldAction"/>: Only executes after holding shortcut for certain time
        /// <br/>   • [DeadZone] <see cref="HoldAction"/>: Only executes when mouse exits circular dead zone
        /// </summary>
        public readonly bool ConflictBehaviourForced;

        /// <summary>
        /// <br/>   Determines if action is prioritized over other conflicting actions.
        /// <br/>   Other conflicting actions are considerend only if action is unsuccessful.
        /// </summary>
        public readonly bool ExecutionPrioritized;

        public readonly ActionShortcutState ShortcutState;

        public readonly ActionDescendantUsability DescendantUsability;


        public ActionSettings(
            bool conflictBehaviourForced,
            bool executionPrioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability)
        {
            ConflictBehaviourForced = conflictBehaviourForced;
            ExecutionPrioritized = executionPrioritized;
            ShortcutState = shortcutState;
            DescendantUsability = descendantUsability;
        }
    }
}
