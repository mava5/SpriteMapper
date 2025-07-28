
using UnityEngine;


namespace SpriteMapper.Actions
{
    [InstantActionSettings(false, false, ActionShortcutState.Rebindable, ActionDescendantUsability.Limited)]
    public class Redo : ShortAction
    {
        public override bool Do() { Debug.Log(Info.Type.Name); /*App.Project.History.Redo();*/ return true; }
    }
}
