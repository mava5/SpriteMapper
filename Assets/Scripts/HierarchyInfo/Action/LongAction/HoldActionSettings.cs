
using System;


namespace SpriteMapper
{
    /// <summary> Determines how a <see cref="HoldAction"/> is cancelled or finished. </summary>
    public enum HoldActionEnding
    {
        /// <summary> Finished when <see cref="Shortcut"/> is released. </summary>
        Release,

        /// <summary>
        /// <br/>   Cancelled when Rmb or Esc is pressed.
        /// <br/>   Finished when <see cref="Shortcut"/> is released.
        /// </summary>
        ReleaseAndHardcodedKeys,
    }

    /// <summary> Determines how <see cref="HoldAction"/> is resolved when in a solvable conflict. </summary>
    public enum HoldActionResolving
    {
        /// <summary> Execute action if mouse leaves a circular dead zone. </summary>
        UseDeadZone,

        /// <summary> Execute action if <see cref="Shortcut"/> is held long enough. </summary>
        UseTimer,
    }


    /// <summary> Contains mandatory settings for a <see cref="HoldAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class HoldActionSettings : LongActionSettings
    {
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

            // Long --------------------------------------- Long
            Type contextUsedWhenActive,

            // Specific ----------------------------------- Specific
            HoldActionEnding endingBehaviour,
            HoldActionResolving conflictBehaviour

            ) : base(conflictBehaviourForced, prioritized, shortcutState, descendantUsability, contextUsedWhenActive)
        {
            EndingBehaviour = endingBehaviour;
            ConflictBehaviour = conflictBehaviour;
        }
    }
}
