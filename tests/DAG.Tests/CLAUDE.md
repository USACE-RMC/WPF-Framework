# DAG.Tests

Unit tests for the DAG graph model library.

## Framework
MSTest 3.5.2 with coverlet for code coverage. Targets `net10.0`.

## Key Test Areas
- **GraphTests.cs** - Tests for cycle detection, topological sort, connection management, path queries (ancestors, descendants, root/leaf nodes), graph depth, and serialization
- **SimpleTestGraph.cs** - Minimal `Graph` subclass used as test fixture
- **SimpleTestNode.cs** - Minimal `NodeBase` subclass used as test fixture

## How to Run
```
dotnet test tests/DAG.Tests/DAG.Tests.csproj
```
