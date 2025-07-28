
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains the nested structure of each <see cref="InfoHierarchyNode"/>.
    /// <br/>   Keeps track of the current context and handles its changing.
    /// <br/>   The context then determines each usable <see cref="Action"/> for the user.
    /// </summary>
    public partial class InfoHierarchy
    {
        public static string GlobalContext { get; } = "Global";
        public string CurrentContext { get; private set; }

        public System.Action ContextChanged { get; private set; }


        public InfoHierarchy()
        {
            foreach (InfoHierarchyItem root in Roots)
            {
                root.SetParentRecursive(null);
            }
        }


        public void ChangeContextTo(string context)
        {
            if (context != CurrentContext) { CurrentContext = context; ContextChanged(); }
        }


        public void SubscribeToContextChanged(System.Action callbackMethod)
        {
            ContextChanged += callbackMethod;
        }

        public void UnsubscribeFromContextChanged(System.Action callbackMethod)
        {
            if (ContextChanged != null) { ContextChanged -= callbackMethod; }
        }
    }
}
