
using UnityEngine;


namespace SpriteMapper.Panels
{
    public class PreviewPanel : Panel
    {
        public override string PanelContext => Context.Viewport.Preview.Name;
    }
}
