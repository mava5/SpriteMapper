
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A floating or docked area primarily parented under another such area.
    /// <br/>   
    /// <br/>   A dockable element can for example be:
    /// <br/>   • <see cref="PanelTabContainer"/>, stores each <see cref="Panel"/> it contains on a tab
    /// <br/>   • <see cref="Container"/>, contains two or more dockable elements
    /// <br/>   
    /// <br/>   Docking example:
    /// <code>
    /// ┌▄▄▄▄▄┬▄▄▄▄▄▄▄▄▄▄▄┬▄▄▄▄▄▄▄▄▄▄▄┬▄▄▄▄▄▄▄┐
    /// │ 1   │ 3         │ 4         │ 6     │
    /// │     │           │           ├▄▄▄┬▄▄▄┤
    /// │     │           │           │ 7 │ 8 │
    /// ├▄▄▄▄▄┤        ┌▄▄▄┬▄▄▄▄▄▄▄┐  │   │   │
    /// │ 2   │        │ 9 │ 10    │  │   │   │
    /// │     ├▄▄▄▄▄▄▄▄│   │       │▄▄┤   │   │
    /// │     │ 5      │   │       │  │   │   │
    /// │     │        └───┴───────┘  │   │   │
    /// │     │                       │   │   │
    /// └─────┴───────────────────────┴───┴───┘
    /// </code>
    /// <br/>   Hierarchy: (PTC = PanelTabContainer)
    /// <code>
    /// MainContainer [Horizontal]
    /// ├─ Container [Vertical]
    /// │  ├─ PTC 1
    /// │  └─ PTC 2
    /// ├─ Container [Vertical]
    /// │  ├─ Container [Horizontal]
    /// │  │  ├─ PTC 3
    /// │  │  └─ PTC 4
    /// │  └─ PTC 5
    /// └─ Container [Vertical]
    ///    ├─ PTC 6
    ///    └─ Container [Horizontal]
    ///       ├─ PTC 7
    ///       └─ PTC 8
    /// FloatingContainer [Horizontal]
    /// ├─ PTC 9
    /// └─ PTC 10
    /// </code>
    /// </summary>
    public class DockableElement : MonoBehaviour
    {
        /// <summary> null = Element is at the root (e.g. main or floating container) </summary>
        public DockableElement Parent { get; internal set; }
    }
}
