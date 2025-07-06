Hierarchy folder:

Contains all Panels, Actions and Tools with a hierarchical structure.
A folder's Actions and Tools can be used by its child folders' Panels.
Global actions however can be used in any Panel.

When a Tool is active, only Tool specific and Tool bypassing Actions can be used.
Tool bypassing actions: CancelTool (Global), QuickUse[Tool] (Panel specific)


Folder and namespace structure example:

| Global:
| ¦ Undo (Actions.Global.Undo)
| ¦ Redo (Actions.Global.Redo)
| ¦ CancelTool (Actions.Global.CancelTool)
| 
| Viewport2D:
| ¦ ResetView (Actions.Viewport2D.ResetView)
| ¦ 
| ¦ DrawImage:
| ¦ | _DrawImagePanel (Panels.Viewport2D.DrawImagePanel)
| ¦ | Invert (Actions.Viewport2D.DrawImage.Invert)
| ¦ | 
| ¦ | DrawTool:
| ¦ | ¦ _DrawTool (Tools.Viewport2D.DrawImage.DrawTool)
| ¦ | ¦ | EquipDrawTool (Actions.Viewport2D.DrawImage.EquipDrawTool)
| ¦ | 
| ¦ | RotateTool:
| ¦ | ¦ _RotateTool (Tools.Viewport2D.DrawImage.RotateTool)
| ¦ | ¦ | EquipRotateTool (Actions.Viewport2D.DrawImage.EquipRotateTool)
| ¦ | ¦ | QuickUseRotateTool (Actions.Viewport2D.DrawImage.QuickUseRotateTool)
| ¦ | ¦ 
| ¦ | ¦ Equiped:
| ¦ | ¦ | 
| ¦ | ¦ 
| ¦ | ¦ 
| ¦ | ¦ CycleFilterMode (Actions.Viewport2D.DrawImage.RotateTool.CycleFilterMode)
|
| Viewport3D:
| ¦ ResetView (Actions.Viewport3D.ResetView)
| ¦ 
| ¦ MeshImage:
| ¦ | _MeshImagePanel (Panels.Viewport3D.MeshImagePanel)
| ¦ | FlipX (Actions.Viewport3D.MeshImage.FlipX)
| ¦ | FlipY (Actions.Viewport3D.MeshImage.FlipY)
| ¦ | FlipZ (Actions.Viewport3D.MeshImage.FlipZ)
| ¦ | 
| ¦ | MoveTool:
| ¦ | ¦ _MoveTool (Tools.Viewport3D.MeshImage.MoveTool)
| ¦ | ¦ | EquipMoveTool (Actions.Viewport3D.MeshImage.EquipMoveTool)
| ¦ | ¦ | QuickUseMoveTool (Actions.Viewport3D.MeshImage.QuickUseMoveTool)
| ¦ | 
| ¦ | RotateTool:
| ¦ | ¦ _RotateTool (Tools.Viewport3D.MeshImage.RotateTool)
| ¦ | ¦ | EquipRotateTool (Actions.Viewport3D.MeshImage.EquipRotateTool)
| ¦ | ¦ | QuickUseRotateTool (Actions.Viewport3D.MeshImage.QuickUseRotateTool)


Terms:

- Action:
An user executable action that is used to interact with different parts of the application.
Each action has its own Shortcut and specific context (Global, Panel, Tool) where it can be used.

- Global:
Global actions can be used anywhere. However, even global actions can't be used when a tool is active.
"Global/ToolBypass" is for global actions that can be used even when a tool is active.

- Panel:
A focusable, resizable and dockable rectangular area, within which specific Actions and Tools can be used.
For example something akin to the Unity Properties tab you might be reading this from.
Inspired by Blender's areas that get focused on mouse hover, which unlocks tools and shortcuts specific to the area.

- Tool:
A tool can be equiped or used with a Shortcut.
When a tool is active, only tool specific and tool bypassing Actions can be used.

- Shortcut:
A key combination, which consists any modifier keys [Shift, Ctrl, Alt] and any other key.