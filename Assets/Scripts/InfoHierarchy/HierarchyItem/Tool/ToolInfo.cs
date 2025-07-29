
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for a <see cref="Tool"/>. </summary>
    public class ToolInfo : InfoHierarchyItem
    {
        /// <summary> The type of the tool the info points to. </summary>
        public readonly Type ToolType;


        public ToolInfo()
        {
            ToolType = Type.GetType(FullName);
        }
    }
}
