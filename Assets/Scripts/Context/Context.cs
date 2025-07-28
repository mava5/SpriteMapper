
using System.Collections;
using System.Collections.Generic;


namespace SpriteMapper
{
    public class Context
    {
        public string Full { get; private set; }
        public string Path { get; private set; }
        public readonly string Name;

        public readonly bool Detached;

        public Context Parent { get; private set; }
        public readonly Context[] Children;


        public Context(string name, bool detached, Context[] children)
        {
            Name = name;
            Detached = detached;
            Children = children;
        }


        #region Public Methods ==================================================================== Public Methods

        public void SetParentRecursive(Context parent)
        {
            Parent = parent;

            Path = Parent?.Path ?? "";
            Full = Path + (Detached ? "/" : ".") + Name;

            foreach (Context child in Children)
            {
                child.SetParentRecursive(this);
            }
        }


        /// <summary>
        /// <br/>   Tells if this context can access given ancestor context.
        /// <br/>   Ancestor contexts cannot be seen through detachments.
        /// <br/>   This applies to global contexts as well.
        /// </summary>
        public bool CanAccess(Context context)
        {
            return
                Path == context.Path ||
                context.Name == "Global" && !Path.Contains('/') ||
                Path.StartsWith(context.Path) && Path.LastIndexOf('/', context.Path.Length) == -1;
        }

        /// <summary> Tells if this context shares path with given context. </summary>
        public bool IsWithin(Context context)
        {
            return Path.StartsWith(context.Path);
        }

        #endregion Public Methods
    }
}

namespace SpriteMapper.ContextCreation
{
    #region Extension Methods =========================================================================================== Extension Methods

    public static class CreationMethods
    {
        public static Group Group(string name, Allowed<Group, MultiPanel, Panel, LongAction, Action> children)
        {
            return new(name, false, children.Contexts);
        }

        //public static MultiPanel For<T>(bool detached = false, Allowed<Panel, LongAction, Action> children = null) where T : SpriteMapper.MultiPanel
        //{
        //    return new(HierarchyInfo.GetMultiPanelInfo<T>().Type.Name, detached, children.Contexts);
        //}

        //public static Panel For<T>(bool detached = false, Allowed<Tool, LongAction, Action> children = null) where T : SpriteMapper.Panel
        //{
        //    return new(HierarchyInfo.GetPanelInfo<T>().Type.Name, detached, children.Contexts);
        //}

        public static Tool For<T>(bool detached = false, Allowed<LongAction, Action> children = null) where T : SpriteMapper.Tool
        {
            return new(HierarchyInfo.GetToolInfo<T>().Type.Name, detached, children.Contexts);
        }

        public static LongAction For<T>(bool detached = false, Allowed<Action> children = null) where T : SpriteMapper.LongAction
        {
            return new(HierarchyInfo.GetActionInfo<T>().Type.Name, detached, children.Contexts);
        }

        public static Action For<T>() where T : SpriteMapper.Action
        {
            return new(HierarchyInfo.GetActionInfo<T>().Type.Name, false, null);
        }
    }

    #endregion Extension Methods


    #region Context Types =================================================================================================== Context Types

    public class Group : Context        { public Group      (string n, bool d, Context[] c) : base(n, d, c) { } }
    public class MultiPanel : Context   { public MultiPanel (string n, bool d, Context[] c) : base(n, d, c) { } }
    public class Panel : Context        { public Panel      (string n, bool d, Context[] c) : base(n, d, c) { } }
    public class Tool : Context         { public Tool       (string n, bool d, Context[] c) : base(n, d, c) { } }
    public class LongAction : Context   { public LongAction (string n, bool d, Context[] c) : base(n, d, c) { } }
    public class Action : Context       { public Action     (string n, bool d, Context[] c) : base(n, d, c) { } }

    public class Allowed : IEnumerable<Context>
    {
        public Context[] Contexts => contexts.ToArray();

        protected readonly List<Context> contexts = new();

        public IEnumerator<Context> GetEnumerator() => contexts.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Allowed<C1> : Allowed
        where C1 : Context
    { public void Add(C1 context) => contexts.Add(context); }

    public class Allowed<C1, C2> : Allowed<C1>
        where C1 : Context where C2 : Context
    { public void Add(C2 context) => contexts.Add(context); }

    public class Allowed<C1, C2, C3> : Allowed<C1, C2>
        where C1 : Context where C2 : Context where C3 : Context
    { public void Add(C3 context) => contexts.Add(context); }

    public class Allowed<C1, C2, C3, C4> : Allowed<C1, C2, C3>
        where C1 : Context where C2 : Context where C3 : Context where C4 : Context
    { public void Add(C4 context) => contexts.Add(context); }

    public class Allowed<C1, C2, C3, C4, C5> : Allowed<C1, C2, C3, C4>
        where C1 : Context where C2 : Context where C3 : Context where C4 : Context where C5 : Context
    { public void Add(C5 context) => contexts.Add(context); }

    #endregion Context Types
}
