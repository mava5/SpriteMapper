
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
            bool executionPrioritized,
            ActionShortcutState shortcutState,
            ActionDescendantUsability descendantUsability,

            // Long --------------------------------------- Long
            Type contextUsedWhenActive

            ) : base(conflictBehaviourForced, executionPrioritized, shortcutState, descendantUsability)
        {
            ContextUsedWhenActive = HF.Hierarchy.FullNameToContext(contextUsedWhenActive.FullName);
        }
    }
}
