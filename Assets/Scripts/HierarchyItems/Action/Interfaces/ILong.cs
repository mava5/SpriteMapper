
namespace SpriteMapper
{
    /// <summary>
    /// <br/>   Interface used to create a long <see cref="Action"/>.
    /// <br/>   
    /// <br/>   A long action:
    /// <br/>   • begins upon creation, gets updated each frame and ends
    /// <br/>   • has its changes applied only when it ends
    /// <br/>   • can get cancelled
    /// <br/>   → Cancelled action's don't apply any changes.
    /// </summary>
    public interface ILong
    {
        /// <summary>
        /// <br/>   A predicate that determines if action is to be ended.
        /// <br/>   Gets evaluated each MonoBehaviour Update().
        /// </summary>
        public bool EndPredicate { get; }

        /// <summary>
        /// <br/>   A predicate that determines if action is to be cancelled.
        /// <br/>   Gets evaluated each MonoBehaviour Update().
        /// </summary>
        public bool CancelPredicate { get; }


        /// <summary>
        /// <br/>   Gets called after action creation.
        /// <br/>   Should return a boolean, which tells if action successfully began.
        /// </summary>
        bool Begin();

        /// <summary>
        /// <br/>   Gets called each MonoBehaviour Update().
        /// <br/>   Only stops being called when action ends or is cancelled.
        /// </summary>
        void Update();

        /// <summary>
        /// <br/>   Gets called when action's <see cref="CancelPredicate"/> is true.
        /// <br/>   Cancels any changes the action would have done.
        /// </summary>
        void Cancel();

        /// <summary>
        /// <br/>   Gets called when action's <see cref="EndPredicate"/> is true.
        /// <br/>   Applies changes done by action.
        /// </summary>
        void End();
    }
}
