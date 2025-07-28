
using System;


namespace SpriteMapper
{
    /// <summary> Contains mandatory settings for an <see cref="InstantAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class InstantActionSettings : ActionSettings
    {
        public override ActionBehaviourType Behaviour => ActionBehaviourType.Instant;
        public override ActionInputType InputType => ActionInputType.Pressed;
        public override ActionDuration Duration => ActionDuration.Short;


        public InstantActionSettings(

            // Base --------------------------------------- Base
            bool conflictBehaviourForced,
            bool prioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability

            ) : base(conflictBehaviourForced, prioritized, shortcutState, descendantUsability)
        {

        }
    }
}
