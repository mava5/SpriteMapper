
using UnityEngine;


namespace SpriteMapper.Actions
{
    [InstantActionSettings(true, false, ActionShortcutState.Rebindable, ActionDescendantUsability.Full)]
    public class OpenContextMenu : ShortAction
    {
        public override bool Do() { Debug.Log(Info.Type.Name); /*App.Project.Panel.ContextMenu.Open();*/ return true; }
    }
}