
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for a <see cref="Tool"/>. </summary>
    public class ToolInfo
    {
        /// <summary> The type of the tool the info points to. </summary>
        public readonly Type ToolType;

        /// <summary> The context used while tool is equipped. Determined by its namespace. </summary>
        public readonly string Context;

        /// <summary> Explanation for how the tool works. </summary>
        public readonly string Description;


        public ToolInfo(SerializedToolInfo serializedInfo)
        {
            ToolType = Type.GetType(serializedInfo.FullName);
            Context = serializedInfo.Context;
            Description = serializedInfo.Description;
        }
    }
}
