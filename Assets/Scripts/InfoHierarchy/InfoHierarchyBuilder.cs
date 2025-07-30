
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Node = SpriteMapper.InfoHierarchyNode;
using Info = SpriteMapper.HierarchyItemInfo<SpriteMapper.HierarchyItem>;


namespace SpriteMapper
{
    public partial class InfoHierarchy
    {
        /// <summary> Contains static methods for creating a specific type of <see cref="Node"/>. </summary>
        public static class Builder
        {
            #region Creation Methods ================================================================================================ Creation Methods

            public static GroupNode Group(string name, string description = "",
                AllowedNodes<GroupNode, MultiPanelNode, PanelNode, LongActionNode, ActionNode> children = null)
            {
                Info groupInfo = new(name, description);
                return new(false, groupInfo, children.Nodes);
            }

            //public static MultiPanelNode MultiPanel(
            //    string nameOverwrite = "", string descriptionOverwrite = "", bool detached = false,
            //    AllowedNodes<PanelNode, LongActionNode, ActionNode> children = null)
            //{
            //    PanelInfo<MultiPanel> multiPanelInfo = new(nameOverwrite, descriptionOverwrite);
            //    return new(detached, multiPanelInfo as Info, children.Nodes);
            //}

            public static PanelNode For<T>(
                string nameOverwrite = "", string descriptionOverwrite = "", bool detached = false,
                AllowedNodes<ToolNode, LongActionNode, ActionNode> children = null) where T : Panel
            {
                PanelInfo<T> panelInfo = new(nameOverwrite, descriptionOverwrite);
                return new(detached, panelInfo as Info, children.Nodes);
            }

            public static ToolNode For<T>(
                Shortcut defaultEquipShortcut,
                string nameOverwrite = "", string descriptionOverwrite = "", bool detached = false,
                AllowedNodes<LongActionNode, ActionNode> children = null) where T : Tool
            {
                ToolInfo<T> toolInfo = new(defaultEquipShortcut, descriptionOverwrite);
                return new(detached, toolInfo as Info, children.Nodes);
            }

            public static LongActionNode For<T>(
                Shortcut defaultShortcut,
                string nameOverwrite = "", string descriptionOverwrite = "", bool detached = false,
                AllowedNodes<ActionNode> children = null) where T : LongAction
            {
                ActionInfo<T> longActionInfo = new(defaultShortcut, descriptionOverwrite);
                return new(detached, longActionInfo as Info, children.Nodes);
            }

            public static ActionNode For<T>(
                Shortcut defaultShortcut,
                string nameOverwrite = "", string descriptionOverwrite = "") where T : Action
            {
                ActionInfo<T> actionInfo = new(defaultShortcut, descriptionOverwrite);
                return new(false, actionInfo as Info, null);
            }

            #endregion Creation Methods


            #region Node Types ====================================================================================================== Node Types

            public class GroupNode : Node       { public GroupNode      (bool d, Info i, List<Node> c) : base(d, i, c) { } }
            public class LongActionNode : Node  { public LongActionNode (bool d, Info i, List<Node> c) : base(d, i, c) { } }
            public class ActionNode : Node      { public ActionNode     (bool d, Info i, List<Node> c) : base(d, i, c) { } }
            public class MultiPanelNode : Node  { public MultiPanelNode (bool d, Info i, List<Node> c) : base(d, i, c) { } }
            public class PanelNode : Node       { public PanelNode      (bool d, Info i, List<Node> c) : base(d, i, c) { } }
            public class ToolNode : Node        { public ToolNode       (bool d, Info i, List<Node> c) : base(d, i, c) { } }

            public class AllowedNodes : IEnumerable<Node>
            {
                public List<Node> Nodes => nodes.ToList();

                protected readonly List<Node> nodes = new();

                public IEnumerator<Node> GetEnumerator() => nodes.GetEnumerator();
                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            public class AllowedNodes<C1> : AllowedNodes
                where C1 : Node
            { public void Add(C1 node) => nodes.Add(node); }

            public class AllowedNodes<C1, C2> : AllowedNodes<C1>
                where C1 : Node where C2 : Node
            { public void Add(C2 node) => nodes.Add(node); }

            public class AllowedNodes<C1, C2, C3> : AllowedNodes<C1, C2>
                where C1 : Node where C2 : Node where C3 : Node
            { public void Add(C3 node) => nodes.Add(node); }

            public class AllowedNodes<C1, C2, C3, C4> : AllowedNodes<C1, C2, C3>
                where C1 : Node where C2 : Node where C3 : Node where C4 : Node
            { public void Add(C4 node) => nodes.Add(node); }

            public class AllowedNodes<C1, C2, C3, C4, C5> : AllowedNodes<C1, C2, C3, C4>
                where C1 : Node where C2 : Node where C3 : Node where C4 : Node where C5 : Node
            { public void Add(C5 node) => nodes.Add(node); }

            #endregion Node Types
        }
    }
}