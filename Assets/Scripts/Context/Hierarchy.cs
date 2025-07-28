
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Contains a nested <see cref="Context"/> for each <see cref="HierarchyItem"/>.
    /// <br/>   Keeps track of application's current context.
    /// <br/>   The context then determines each <see cref="Action"/> user can use.
    /// </summary>
    public class Hierarchy
    {
        public Context CurrentContext { get; private set; }
        public Context GlobalContext { get; private set; }

        public System.Action ContextChanged { get; private set; }


        public Hierarchy()
        {
            foreach (Context rootContext in HierarchyStructure.RootContexts)
            {
                if (rootContext.Name == "Global") { GlobalContext = rootContext; }

                rootContext.SetParentRecursive(rootContext);
            }
        }


        public void ChangeTo(Context context)
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
