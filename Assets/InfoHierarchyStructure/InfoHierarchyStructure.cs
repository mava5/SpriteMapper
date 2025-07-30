
using System.Collections.Generic;

using static SpriteMapper.InfoHierarchy.Builder;


namespace SpriteMapper
{
    public partial class InfoHierarchy
    {
        public static List<InfoHierarchyNode> RootNodes = new()
        {
            Group(GlobalContext, new()
            {
                For<Actions.Undo>(),
                For<Actions.Redo>(),
                For<Actions.CancelLongActions>(),
                For<Actions.FinishLongActions>()
            }),

            Group("Viewport", new()
            {
                For<Actions.ResetView>(),

                Group("Editor", new()
                {

                })
            }),
        });
    }
}
