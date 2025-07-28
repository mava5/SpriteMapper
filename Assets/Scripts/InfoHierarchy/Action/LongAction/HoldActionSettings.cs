
using System;


namespace SpriteMapper
{
    /// <summary> Determines how a hold <see cref="Action"/> is cancelled or finished. </summary>
    public enum HoldActionEnding
    {
        /// <summary> Finished when <see cref="Shortcut"/> is released. </summary>
        Release,

        /// <summary>
        /// <br/>   Cancelled by <see cref="Hierarchy.Global.Context.CancelLongActions"/>.
        /// <br/>   Finished when <see cref="Shortcut"/> is released.
        /// </summary>
        CancelActionAndRelease,
    }

    /// <summary> Determines how a hold <see cref="Action"/> is resolved when in a solvable conflict. </summary>
    public enum HoldActionResolving
    {
        /// <summary> Execute action if mouse leaves a circular dead zone. </summary>
        UseDeadZone,

        /// <summary> Execute action if <see cref="Shortcut"/> is held long enough. </summary>
        UseTimer,
    }


    /// <summary> Contains mandatory settings for a held <see cref="LongAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class HoldActionSettings : ActionSettings
    {
        public override ActionBehaviourType Behaviour => ActionBehaviourType.Hold;
        public override ActionInputType InputType => ActionInputType.Held;
        public override ActionDuration Duration => ActionDuration.Long;

        public readonly HoldActionEnding EndingBehaviour;
        public readonly HoldActionResolving ConflictBehaviour;


        public HoldActionSettings(
            
            // Base --------------------------------------- Base
            bool conflictBehaviourForced,
            bool prioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability,

            // Specific ----------------------------------- Specific
            HoldActionEnding endingBehaviour,
            HoldActionResolving conflictBehaviour

            ) : base(conflictBehaviourForced, prioritized, shortcutState, descendantUsability)
        {
            EndingBehaviour = endingBehaviour;
            ConflictBehaviour = conflictBehaviour;
        }
    }
}
