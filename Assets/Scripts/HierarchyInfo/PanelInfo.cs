
using System;


namespace SpriteMapper
{
    /// <summary> Contains necessary information for a <see cref="Panel"/>. </summary>
    public class PanelInfo
    {
        /// <summary> The type of the panel the info points to. </summary>
        public readonly Type PanelType;

        /// <summary> The panel's context. Determined by its namespace. </summary>
        public readonly string Context;

        /// <summary> Explanation for how the panel works. </summary>
        public readonly string Description;


        public PanelInfo(SerializedPanelInfo serializedInfo)
        {
            PanelType = Type.GetType(serializedInfo.FullName);
            Context = serializedInfo.Context;
            Description = serializedInfo.Description;
        }
    }
}
