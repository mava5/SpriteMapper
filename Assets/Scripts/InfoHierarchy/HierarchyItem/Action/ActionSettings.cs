
using System;


namespace SpriteMapper
{
    /// <summary> Determines if <see cref="Action"/> has a <see cref="Shortcut"/> and in what state. </summary>
    public enum ActionShortcutState
    {
        /// <summary> Action has no shortcut. </summary>
        None,

        /// <summary> Action has a shortcut, which can be rebinded. </summary>
        Rebindable,

        /// <summary> Action has a shortcut but it cannot be rebinded. </summary>
        Locked,
    }

    /// <summary> Determines in what descendant contexts <see cref="Action"/> can be used. </summary>
    public enum ActionDescendantUsability
    {
        /// <summary> Action is only usable within its context </summary>
        None,

        /// <summary> Action can be used in all descendant contexts up to any detachments. </summary>
        UpToDetachment,

        /// <summary> Action can be used in all descendant contexts even through detachments. </summary>
        PastDetachments,
    }
    

    /// <summary>
    /// <br/>   Contains mandatory base settings for an <see cref="Action"/>.
    /// <br/>   Inherited with more specific information for each <see cref="ActionBehaviourType"/>.
    /// <br/>   Further specifies the behaviour of an action.
    /// </summary>
    public abstract class ActionSettings : HierarchyItemSettings
    {
        public abstract ActionBehaviourType Behaviour { get; }
        public abstract ActionInputType InputType { get; }
        public abstract ActionDuration Duration { get; }

        /// <summary>
        /// <br/>   Determines if action always behaves as if in a solvable <see cref="Shortcut"/> conflict.
        /// <br/>   
        /// <br/>   Differnet action types behave in the following way in a solvable conflict:
        /// <br/>   • Instant action: Only executes on shortcut release
        /// <br/>   • Toggle action: Only begins execution on shortcut release
        /// <br/>   • Timer hold action: Only executes after holding shortcut for certain time
        /// <br/>   • Dead zone hold action: Only executes when mouse exits circular dead zone
        /// </summary>
        public readonly bool ConflictBehaviourForced;

        /// <summary>
        /// <br/>   Determines if action is prioritized over other actions.
        /// <br/>   unprioritized actions are considerend only if this action is unsuccessful.
        /// </summary>
        public readonly bool Prioritized;

        public ActionShortcutState ShortcutState;

        public readonly ActionDescendantUsability DescendantUsability;


        public ActionSettings(

            string description,
            bool conflictBehaviourForced,
            bool prioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability
            
            ) : base(description)
        {
            ConflictBehaviourForced = conflictBehaviourForced;
            Prioritized = prioritized;
            ShortcutState = shortcutState;
            DescendantUsability = descendantUsability;
        }
    }
}
