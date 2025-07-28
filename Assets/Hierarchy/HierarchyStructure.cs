
using SpriteMapper.Actions;
//using SpriteMapper.Panels;
//using SpriteMapper.Tools;

using CreateContext = SpriteMapper.ContextCreation.CreationMethods;


namespace SpriteMapper
{
    public static class HierarchyStructure
    {
        public static Context[] RootContexts { get; } =
        {
            CreateContext.Group("Global", new()
            {
                CreateContext.For<Undo>(),
                CreateContext.For<Redo>(),
                CreateContext.For<CancelLongActions>(),
                CreateContext.For<FinishLongActions>(),
            }),

            CreateContext.Group("Viewport", new()
            {
                CreateContext.For<ResetView>(),

                CreateContext.Group("Editor", new()
                {

                })
            }),
        };
    }
}
