Hierarchy folder:

Contains all Actions and Tools with a hierarchical structure.
The folder structure should reflect the class' namespaces for consistency.
A folder's Actions and Tools can be used in its child folders' contexts.
Global actions can be used anywhere.

When a Tool is active, only Tool specific and Tool bypassing Actions can be used.
Example tool bypassing actions: Global.CancelToolQuickUse, QuickUse[Tool name]


Folder and namespace structure example:

| Global:
| ¦ Undo (Actions.Global.Undo)
| ¦ Redo (Actions.Global.Redo)
| ¦ EndToolQuickUse (Actions.Global.EndToolQuickUse)
| ¦ CancelToolQuickUse (Actions.Global.CancelToolQuickUse)
| 
| Viewport:
| ¦ ResetView (Actions.Viewport.ResetView)
| ¦ 
| ¦ DrawImage:
| ¦ | Invert (Actions.Viewport.DrawImage.Invert)
| ¦ | 
| ¦ | DrawTool (Tools.Viewport.DrawImage.DrawTool)
| ¦ | ¦ EquipDrawTool (Actions.Viewport.DrawImage.EquipDrawTool)
| ¦ | 
| ¦ | RotateTool (Tools.Viewport.DrawImage.RotateTool)
| ¦ | ¦ EquipRotateTool (Actions.Viewport.DrawImage.EquipRotateTool)
| ¦ | ¦ QuickUseRotateTool (Actions.Viewport.DrawImage.QuickUseRotateTool)
| ¦ | ¦ 
| ¦ | ¦ Equipped:
| ¦ | ¦ Rotate (Tools.Viewport.DrawImage.RotateToolEquipped.Rotate)
| ¦ | ¦ 
| ¦ | ¦ Active:
| ¦ | ¦ CycleFilterMode (Actions.Viewport.DrawImage.RotateToolActive.CycleFilterMode)
| ¦ 
| ¦ MeshImage:
| ¦ | FlipX (Actions.Viewport.MeshImage.FlipX)
| ¦ | FlipY (Actions.Viewport.MeshImage.FlipY)
| ¦ | FlipZ (Actions.Viewport.MeshImage.FlipZ)
| ¦ | 
| ¦ | MoveTool (Tools.Viewport.MeshImage.MoveTool)
| ¦ | ¦ EquipMoveTool (Actions.Viewport.MeshImage.EquipMoveTool)
| ¦ | ¦ QuickUseMoveTool (Actions.Viewport.MeshImage.QuickUseMoveTool)
| ¦ | 
| ¦ | RotateTool (Tools.Viewport.MeshImage.RotateTool)
| ¦ | ¦ EquipRotateTool (Actions.Viewport.MeshImage.EquipRotateTool)
| ¦ | ¦ QuickUseRotateTool (Actions.Viewport.MeshImage.QuickUseRotateTool)


Terms:

- Action:
An user executable action that is used to interact with different parts of the application.
Each action has its own Shortcut and specific context (Global, specific panel or tool) where it can be used.

- Global:
Global actions can be used anywhere. Although, even global actions can't be used when a tool is active.
However, an Action can be made to bypass this restriction.

- Panel:
A focusable, resizable and dockable rectangular area, within which specific Actions and Tools can be used.
Inspired by Blender's areas that get focused on mouse hover, which unlocks tools and shortcuts specific to the area.
A panel can have multiple different contexts. For example ImageEditorPanel has different Image types' contexts.

- Tool:
A tool can be equipped or quick used with a Shortcut.
When a tool is active, only tool specific and tool bypassing Actions can be used.

- Shortcut:
A key combination, which consists of any modifier keys [Shift, Ctrl, Alt] and any other key.