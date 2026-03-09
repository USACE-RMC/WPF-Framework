# DAG

## Purpose
Platform-agnostic directed acyclic graph (DAG) model library providing node, connector, and graph abstractions with cycle detection, topological sorting, path queries, and XML serialization.

## Key Files
- `Graph.cs` - Abstract base class for DAG graphs; manages nodes/connections, cycle detection (`WouldCreateCycle`), topological sort, ancestor/descendant queries, and XML round-trip serialization
- `NodeBase.cs` - Abstract base class for nodes; has GUID, position, input/output connectors, and XML serialization
- `InConnector.cs` - Input connector attached to a node
- `OutConnector.cs` - Output connector attached to a node
- `Utilities.cs` - Property change notification helpers

## Dependencies
None (pure .NET library, no WPF dependency). Targets `net10.0`.

## Patterns
- Subclass `Graph` and `NodeBase` to create custom node types; override `ReadNodeRequested` and `AddToBaseElement` for serialization
- Connections stored as `Tuple<OutConnector, InConnector>`; serialized by connector index (deserialization constructors must recreate connectors in the same order)
- Connection add/remove fires preview events that allow cancellation

## Gotchas
- NodeBase serialization stores connections by connector index -- deserialization constructors must recreate connectors in the same order as the original
