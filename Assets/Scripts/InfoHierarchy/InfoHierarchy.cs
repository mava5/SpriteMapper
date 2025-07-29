
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains the nested structure of each <see cref="InfoHierarchyNode"/>.
    /// <br/>   Each of these infos point to a <see cref="HierarchyItem"/>.
    /// <br/>   Same item can appear multiple times in the hierarchy.
    /// <br/>   
    /// <br/>   Also keeps track of the current context and handles its changing.
    /// <br/>   The context then determines each <see cref="Action"/> at the user's disposal.
    /// </summary>
    public partial class InfoHierarchy
    {
        public static string GlobalContext { get; } = "Global";
        public string CurrentContext { get; private set; }

        public System.Action ContextChanged { get; private set; }


        public InfoHierarchy()
        {
            foreach (InfoHierarchyNode root in Roots)
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
