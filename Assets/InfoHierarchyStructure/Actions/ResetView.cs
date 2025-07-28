
using UnityEngine;


namespace SpriteMapper.Actions
{
    [InstantActionSettings(false, false, ActionShortcutState.Rebindable, ActionDescendantUsability.Limited)]
    public class ResetView : ShortAction
    {
        public override bool Do()
        {
            Debug.Log(Info.Type.Name);
            //(ViewportPanel)App.Project.Panel

            return true;
        }
    }
}
