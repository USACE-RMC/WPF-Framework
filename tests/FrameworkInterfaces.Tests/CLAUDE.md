# FrameworkInterfaces.Tests

Unit tests for the FrameworkInterfaces library.

## Framework

xUnit 2.9.2 with coverlet for code coverage. Targets `net9.0-windows` (WPF).

## Key Test Areas

- **Messaging**: `BasicMessageItemTests.cs`, `MessengerTests.cs` - Tests the messaging/event bus system
- **Undo**: `UndoManagerTests.cs` - Tests undo/redo manager operations
- **Undo Actions**: `AddElementActionTests.cs`, `MoveElementActionTests.cs`, `RemoveElementActionTests.cs` - Tests collection element undo actions
- **Undo Actions**: `CompositeActionTests.cs`, `DelegateActionTests.cs`, `PropertyChangeActionTests.cs` - Tests composite, delegate, and property change undo actions
- **Undo Bridge**: `UndoableStateBridgeTests.cs` - Tests bridge between undoable state and undo manager

## How to Run

```
dotnet test tests/FrameworkInterfaces.Tests/FrameworkInterfaces.Tests.csproj
```
