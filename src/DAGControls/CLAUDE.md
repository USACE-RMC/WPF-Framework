# DAGControls

## Purpose
WPF control library for interactive visualization and editing of DAG graphs. Provides a canvas with drag-and-drop node positioning, interactive connection creation via Bezier curves, zoom/pan navigation, and context menus.

## Key Files
- `FlowGraphCanvas.xaml.cs` - Main canvas control; handles node dragging, connection drawing, zoom/pan, and context menus
- `NodeControl.xaml.cs` - Visual representation of a single node with input/output connector indicators
- `Resources/` - Custom cursors (AddPointCursor.cur, Pan_Hand_Closed.cur) and icons (Delete.png)

## Dependencies
- **DAG** (project reference) - Graph, NodeBase, and connector model classes

## Patterns
- Set `FlowGraphCanvas.Graph` property to bind a `Graph` instance; the canvas auto-syncs with node/connection changes
- Canvas exposes delegate events (`CanvasContextMenu`, `NodeContextMenu`, `ConnectionAdded`, `ConnectionRemoved`, `NodeMoved`) for host app customization
- Class name `FlowGraphCanvas` is intentionally kept (not renamed to match `DAGControls` namespace)
