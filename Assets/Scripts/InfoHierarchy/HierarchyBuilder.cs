
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Item = SpriteMapper.InfoHierarchyItem;


namespace SpriteMapper
{

    public partial class InfoHierarchy
    {
        public static class Builder
        {
            #region Creation Methods ================================================================================================ Creation Methods

            public static ItemGroup Group(string name, Allowed<ItemGroup, MultiPanel, Panel, LongAction, Action> children)
            {
                return new(name, false, children.Items);
            }

            //public static MultiPanel For<T>(bool detached = false, Allowed<Panel, LongAction, Action> children = null) where T : SpriteMapper.MultiPanel
            //{
            //    return new(HierarchyInfo.GetMultiPanelInfo<T>().Type.Name, detached, children.Items);
            //}

            //public static Panel For<T>(bool detached = false, Allowed<Tool, LongAction, Action> children = null) where T : SpriteMapper.Panel
            //{
            //    return new(HierarchyInfo.GetPanelInfo<T>().Type.Name, detached, children.Items);
            //}

            public static Tool For<T>(bool detached = false, Allowed<LongAction, Action> children = null) where T : SpriteMapper.Tool
            {
                return new(HierarchyInfo.GetToolInfo<T>().Type.Name, detached, children.Items);
            }

            public static LongAction For<T>(bool detached = false, Allowed<Action> children = null) where T : SpriteMapper.LongAction
            {
                return new(HierarchyInfo.GetActionInfo<T>().Type.Name, detached, children.Items);
            }

            public static Action For<T>() where T : SpriteMapper.Action
            {
                return new(HierarchyInfo.GetActionInfo<T>().Type.Name, false, null);
            }

            #endregion Creation Methods


            #region Item Types ====================================================================================================== Item Types

            public class ItemGroup : Item   { public ItemGroup  (string n, bool d, List<Item> c) : base(n, d, c) { } }
            public class MultiPanel : Item  { public MultiPanel (string n, bool d, List<Item> c) : base(n, d, c) { } }
            public class Panel : Item       { public Panel      (string n, bool d, List<Item> c) : base(n, d, c) { } }
            public class Tool : Item        { public Tool       (string n, bool d, List<Item> c) : base(n, d, c) { } }
            public class LongAction : Item  { public LongAction (string n, bool d, List<Item> c) : base(n, d, c) { } }
            public class Action : Item      { public Action     (string n, bool d, List<Item> c) : base(n, d, c) { } }

            public class Allowed : IEnumerable<Item>
            {
                public List<Item> Items => items.ToList();

                protected readonly List<Item> items = new();

                public IEnumerator<Item> GetEnumerator() => items.GetEnumerator();
                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            public class Allowed<C1> : Allowed
                where C1 : Item
            { public void Add(C1 item) => items.Add(item); }

            public class Allowed<C1, C2> : Allowed<C1>
                where C1 : Item where C2 : Item
            { public void Add(C2 item) => items.Add(item); }

            public class Allowed<C1, C2, C3> : Allowed<C1, C2>
                where C1 : Item where C2 : Item where C3 : Item
            { public void Add(C3 item) => items.Add(item); }

            public class Allowed<C1, C2, C3, C4> : Allowed<C1, C2, C3>
                where C1 : Item where C2 : Item where C3 : Item where C4 : Item
            { public void Add(C4 item) => items.Add(item); }

            public class Allowed<C1, C2, C3, C4, C5> : Allowed<C1, C2, C3, C4>
                where C1 : Item where C2 : Item where C3 : Item where C4 : Item where C5 : Item
            { public void Add(C5 item) => items.Add(item); }

            #endregion Item Types
        }
    }
}