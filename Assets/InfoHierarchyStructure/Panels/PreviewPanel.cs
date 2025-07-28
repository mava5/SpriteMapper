
namespace SpriteMapper.Panels
{

    public class PreviewPanel : ViewportPanel
    {
        public override string Description => "A panel used for editing any image type.";

        public override string Context { get; internal set; } //=
            //HF.Hierarchy.TypeToContext<Hierarchy.Viewport.Preview.Context>();


        private void Start()
        {
            ContextMenu = new ContextMenu();
        }




        public override void EquipTool<T>() { }
    }
}
