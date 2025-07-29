
namespace SpriteMapper
{
    /// <summary> Contains mandatory settings for a pressed <see cref="ShortAction"/>. </summary>
    public class InstantActionSettings : ActionSettings
    {
        public override ActionBehaviourType Behaviour => ActionBehaviourType.Instant;
        public override ActionInputType InputType => ActionInputType.Pressed;
        public override ActionDuration Duration => ActionDuration.Short;


        public InstantActionSettings(

            // Base --------------------------------------- Base
            string description,
            bool conflictBehaviourForced,
            bool prioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability

            ) : base(description, conflictBehaviourForced, prioritized, shortcutState, descendantUsability)
        {

        }
    }
}
