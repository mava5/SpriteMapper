
using System.Linq;
using UnityEngine;


namespace SpriteMapper
{
    /// <summary> A collection of nested static class with varying helper functions. </summary>
    public static class HF
    {
        /// <summary> Contains hierarchy info related functions. </summary>
        public static class Hierarchy
        {
            /// <summary>
            /// <br/>   Returns a hierarchy item type's FullName as a context name.
            /// <br/>   For example: "SpriteMapper.Actions.Global.Undo" -> "Global"
            /// </summary>
            public static string FullNameToContext(string fullName)
            {
                // For example:
                // X "Actions.Global.Undo"
                // X "SpriteMapper.Actions"
                // X "SpriteMapper.Global.Undo"
                // X "SpriteMapper.Actions.Undo"
                // ✓ "SpriteMapper.Actions.Global.Undo"
                if ((!fullName.StartsWith("SpriteMapper.Actions.") &&
                    !fullName.StartsWith("SpriteMapper.Tools.")) ||
                    fullName.Count(c => c == '.') <= 2)
                {
                    Debug.LogWarning($"Hierarchy item {fullName} has invalid namespace!");
                    return "Wrong Context";
                }

                return fullName[(fullName.IndexOf(".", fullName.IndexOf(".") + 1) + 1)..fullName.LastIndexOf(".")];
            }

            public static string TypeToContext<T>() { return FullNameToContext(typeof(T).FullName); }
        }
    }
}
