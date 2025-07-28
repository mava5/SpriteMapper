
using System;


namespace SpriteMapper
{
    /// <summary> Contains mandatory settings for a <see cref="LongAction"/>. </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class LongActionSettings : ActionSettings
    {
        /// <summary> If not left empty, long action will overwrite context to this while active. </summary>
        public readonly string ContextUsedWhenActive = "";


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
            ContextUsedWhenActive = HierarchyInfo.FullNameToContext(contextUsedWhenActive.FullName);
        }
    }
}
