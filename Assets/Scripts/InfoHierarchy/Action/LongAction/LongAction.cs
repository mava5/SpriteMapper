
using UnityEngine;

namespace SpriteMapper
{
    /// <summary>
    /// <br/>   <see cref="Action"/> that begins, lasts multiple frames and either gets cancelled or finished.
    /// <br/>   Can be made to use its own context when active.
    /// </summary>
    public abstract class LongAction : Action
    {
        public bool BeganThisFrame { get; set; } = true;

        public Vector2 MouseStartPosition { get; internal set; }

        /// <summary> Determines whether long action can be cancelled by <see cref="Hierarchy.Global.Context.CancelLongActions"/>. </summary>
        public bool EndableByCancelAction
        {
            get
            {
                return Info.Settings.Behaviour switch
                {
                    ActionBehaviourType.Toggle => (Info.Settings as ToggleActionSettings).EndingBehaviour == ToggleActionEnding.CancelAndFinishAction,
                    ActionBehaviourType.Hold => (Info.Settings as HoldActionSettings).EndingBehaviour == HoldActionEnding.CancelActionAndRelease,
                    _ => false
                };
            }
        }

        /// <summary> Determines whether long action can be finished by <see cref="Hierarchy.Global.Context.FinishLongActions"/>. </summary>
        public bool EndableByFinishAction
        {
            get
            {
                return Info.Settings.Behaviour switch
                {
                    ActionBehaviourType.Toggle => (Info.Settings as ToggleActionSettings).EndingBehaviour == ToggleActionEnding.CancelAndFinishAction,
                    _ => false
                };
            }
        }


        /// <summary> Begins action execution. </summary>
        /// <returns> <see cref="bool"/>: Tells whether action began successfully. </returns>
        public abstract bool Begin(Vector2 mouseStartPosition);

        /// <summary> Gets called each MonoBehaviour Update(). </summary>
        public abstract void Update();

        /// <summary> Discards any changes the action would have done. </summary>
        public abstract void Cancel();

        /// <summary> Applies changes done by action. </summary>
        public abstract void Finish();
    }
}
