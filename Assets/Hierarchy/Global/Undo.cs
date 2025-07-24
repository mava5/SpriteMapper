
using UnityEngine;

namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        [InstantActionSettings(false, false, ActionShortcutState.Exists, ActionDescendantUsability.Limited)]
        public class Undo : ShortAction
        {
            public override bool Do() { Debug.Log(Info.ActionType.Name); /*App.Project.History.Undo();*/ return true; }
        }
    }
}
