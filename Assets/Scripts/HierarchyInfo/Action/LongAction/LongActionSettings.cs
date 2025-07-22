
using System;


namespace SpriteMapper
{
    /// <summary> Contains mandatory settings for a <see cref="HoldAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class LongActionSettings : ActionSettings
    {
        public readonly string ContextUsedWhenActive;


        public LongActionSettings(

            // Base --------------------------------------- Base
            bool conflictBehaviourForced,
            bool prioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability,

            // Long --------------------------------------- Long
            Type contextUsedWhenActive

            ) : base(conflictBehaviourForced, prioritized, shortcutState, descendantUsability)
        {
            ContextUsedWhenActive = HF.Hierarchy.FullNameToContext(contextUsedWhenActive.FullName);
        }
    }
}
