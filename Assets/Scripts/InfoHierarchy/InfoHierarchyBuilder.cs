
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Node = SpriteMapper.InfoHierarchyNode;
using Info = SpriteMapper.HierarchyItemInfo<SpriteMapper.HierarchyItem>;


namespace SpriteMapper
{

    public partial class InfoHierarchy
    {
        public static class Builder
        {
            #region Creation Methods ================================================================================================ Creation Methods

            public static InfoGroup Group(string name, AllowedNodes<InfoGroup, MultiPanel, Panel, LongAction, Action> children)
            {
                return new(name, false, new(name), children.Nodes);
            }

            //public static MultiPanel For<T>(bool detached = false, AllowedNodes<Panel, LongAction, Action> children = null) where T : SpriteMapper.MultiPanel
            //{
            //    return new(InfoHierarchy.GetMultiPanelInfo<T>().Type.Name, detached, children.Nodes);
            //}

            public static Panel For<T>(bool detached = false, AllowedNodes<Tool, LongAction, Action> children = null) where T : SpriteMapper.Panel
            {
                return null; //new(InfoHierarchy.GetPanelInfo<T>().Type.Name, detached, children.Nodes);
            }

            public static Tool For<T>(bool detached = false, AllowedNodes<LongAction, Action> children = null) where T : SpriteMapper.Tool
            {
                return null; //new(InfoHierarchy.GetToolInfo<T>().Type.Name, detached, children.Nodes);
            }

            public static LongAction For<T>(bool detached = false, AllowedNodes<Action> children = null) where T : SpriteMapper.LongAction
            {
                return null; //new(InfoHierarchy.GetActionInfo<T>().Type.Name, detached, children.Nodes);
            }

            public static Action For<T>() where T : SpriteMapper.Action
            {
                return null; //new(InfoHierarchy.GetActionInfo<T>().Type.Name, false, null);
            }

            #endregion Creation Methods


            #region Node Types ====================================================================================================== Node Types

            public class InfoGroup : Node   { public InfoGroup  (string n, bool d, Info i, List<Node> c) : base(n, d, i, c) { } }
            public class LongAction : Node  { public LongAction (string n, bool d, Info i, List<Node> c) : base(n, d, i, c) { } }
            public class Action : Node      { public Action     (string n, bool d, Info i, List<Node> c) : base(n, d, i, c) { } }
            public class MultiPanel : Node  { public MultiPanel (string n, bool d, Info i, List<Node> c) : base(n, d, i, c) { } }
            public class Panel : Node       { public Panel      (string n, bool d, Info i, List<Node> c) : base(n, d, i, c) { } }
            public class Tool : Node        { public Tool       (string n, bool d, Info i, List<Node> c) : base(n, d, i, c) { } }

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