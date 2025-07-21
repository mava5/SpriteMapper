
using UnityEngine;

namespace SpriteMapper.Hierarchy.Global
{
    partial class Context
    {
        /// <summary> Opens context menu immediately. </summary>
        [NotShortcutExecutable, ActionPriority(PriorityLevel.Low)]
        public class OpenContextMenuShort : Action, IShort
        {
            public bool Do() { App.Project.Panel.ContextMenu.Open(); return true; }
        }

        /// <summary>
        /// <br/>   Used when current context has a <see cref="ILong"/> <see cref="Action"/> with right click <see cref="Shortcut"/>.
        /// <br/>   This action doesn't hinder other actions with right click shortcuts.
        /// <br/>   It only opens context menu if user lets go of right click within certain time frame.
        /// <br/>   For example prevents context menu opening when rotating camera in a Viewport3D panel.
        /// </summary>
        [NotShortcutExecutable, ActionPriority(PriorityLevel.Low)]
        public class OpenContextMenuLong : Action, IShort
        {
            private float startTime = Time.time;


            public bool Begin() { return false; }

            public void Update() { }

            public void Cancel() { }

            public void End() { App.Project.Panel.ContextMenu.Open(); }
        }
    }
}