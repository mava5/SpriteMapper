
using UnityEngine;

namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        [InstantActionSettings(true, false, ActionShortcutState.Exists, ActionDescendantUsability.Full)]
        public class OpenContextMenu : ShortAction
        {
            public override bool Do() { Debug.Log(Info.ActionType.Name); /*App.Project.Panel.ContextMenu.Open();*/ return true; }
        }
    }
}