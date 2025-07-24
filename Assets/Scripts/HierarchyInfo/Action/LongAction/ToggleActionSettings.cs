
using System;


namespace SpriteMapper
{
    /// <summary> Determines how a pressed <see cref="LongAction"/> is cancelled or finished. </summary>
    public enum ToggleActionEnding
    {
        /// <summary> Finished manually by action or from an outside source. </summary>
        Manual,

        /// <summary>
        /// <br/>   Cancelled by <see cref="Hierarchy.Global.Context.CancelLongActions"/>.
        /// <br/>   Finished by <see cref="Hierarchy.Global.Context.FinishLongActions"/>.
        /// </summary>
        CancelAndFinishAction,
    }


    /// <summary> Contains mandatory settings for a pressed <see cref="LongAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class ToggleActionSettings : LongActionSettings
    {
        public override ActionBehaviourType Behaviour => ActionBehaviourType.Toggle;
        public override ActionInputType InputType => ActionInputType.Pressed;
        public override ActionDuration Duration => ActionDuration.Long;

        public readonly ToggleActionEnding EndingBehaviour;


        public ToggleActionSettings(

            // Base --------------------------------------- Base
            bool conflictBehaviourForced,
            bool prioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability,

            // Long --------------------------------------- Long
            Type contextUsedWhenActive,

            // Specific ----------------------------------- Specific
            ToggleActionEnding endingBehaviour
            
            ) : base(conflictBehaviourForced, prioritized, shortcutState, descendantUsability, contextUsedWhenActive)
        {
            EndingBehaviour = endingBehaviour;
        }
    }
}
