
using System;


namespace SpriteMapper
{
    /// <summary> Determines how a <see cref="ToggleAction"/> is cancelled or finished. </summary>
    public enum ToggleActionEnding
    {
        /// <summary> Finished when <see cref="Shortcut"/> is pressed again. </summary>
        Repress,

        /// <summary>
        /// <br/>   Cancelled when Rmb or Esc is pressed.
        /// <br/>   Finished when Lmb or Enter is pressed.
        /// </summary>
        HardcodedKeys,
    }


    /// <summary> Contains mandatory settings for a <see cref="HoldAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class ToggleActionSettings : LongActionSettings
    {
        public override ActionInputType InputType => ActionInputType.Pressed;
        public override ActionDuration Duration => ActionDuration.Long;

        public readonly ToggleActionEnding EndingBehaviour;


        public ToggleActionSettings(

            // Base --------------------------------------- Base
            bool conflictBehaviourForced,
            bool executionPrioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability,

            // Long --------------------------------------- Long
            Type contextUsedWhenActive,

            // Specific ----------------------------------- Specific
            ToggleActionEnding endingBehaviour
            
            ) : base(conflictBehaviourForced, executionPrioritized, shortcutState, descendantUsability, contextUsedWhenActive)
        {
            EndingBehaviour = endingBehaviour;
        }
    }
}
