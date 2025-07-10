
using UnityEngine;


namespace SpriteMapper
{
    /// <summary>
    /// <br/>   A floating or docked area primarily parented under another such area.
    /// <br/>   Can for example be a <see cref="Panel"/> (no children) or a <see cref="Container"/> (2 or more children).
    /// <br/>   
    /// <br/>   Docking example:
    /// <code>
    /// ┌─────┬───────────┬───────────┬───────┐
    /// │ 1   │ 3         │ 4         │ 6     │
    /// │     │           │           ├───┬───┤
    /// │     │           │           │ 7 │ 8 │
    /// ├─────┤        ┌───┬───────┐  │   │   │
    /// │ 2   │        │ 9 │ 10    │  │   │   │
    /// │     ├────────│   │       │──┤   │   │
    /// │     │ 5      │   │       │  │   │   │
    /// │     │        └───┴───────┘  │   │   │
    /// │     │                       │   │   │
    /// └─────┴───────────────────────┴───┴───┘
    /// </code>
    /// <br/>   Hierarchy:
    /// <code>
    /// MainContainer [Horizontal]
    /// ├─ Container [Vertical]
    /// │  ├─ Panel 1
    /// │  └─ Panel 2
    /// ├─ Container [Vertical]
    /// │  ├─ Container [Horizontal]
    /// │  │  ├─ Panel 3
    /// │  │  └─ Panel 4
    /// │  └─ Panel 5
    /// └─ Container [Vertical]
    ///    ├─ Panel 6
    ///    └─ Container [Horizontal]
    ///       ├─ Panel 7
    ///       └─ Panel 8
    /// FloatingContainer [Horizontal]
    /// ├─ Panel 9
    /// └─ Panel 10
    /// </code>
    /// </summary>
    public class DockableElement
    {
        /// <summary> null = Element is at the root (e.g. main or floating container) </summary>
        public DockableElement Parent { get; internal set; }
    }
}
