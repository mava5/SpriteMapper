
using UnityEngine;

namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        [InstantActionSettings(false, false, ActionShortcutState.Exists, ActionDescendantUsability.Limited)]
        public class Redo : ShortAction
        {
            public override bool Do() { Debug.Log(Info.ActionType.Name); /*App.Project.History.Redo();*/ return true; }
        }
    }
}
