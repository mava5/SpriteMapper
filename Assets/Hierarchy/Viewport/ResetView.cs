
using UnityEngine;

namespace SpriteMapper.Hierarchy.Viewport
{
    partial class Context
    {
        [InstantActionSettings(false, false, ActionShortcutState.Exists, ActionDescendantUsability.Limited)]
        public class ResetView : ShortAction
        {
            public override bool Do()
            {
                Debug.Log(Info.ActionType.Name);
                //(ViewportPanel)App.Project.Panel

                return true;
            }
        }
    }
}
