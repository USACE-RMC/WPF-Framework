# Undo/Redo System Guide

The WPF-Framework framework includes a comprehensive undo/redo system that tracks property changes and collection operations, allowing users to reverse their actions.

## Overview

The undo system is built on:

- **IUndoManager** - Interface for undo/redo stack management
- **IUndoableAction** - Interface for reversible actions
- **IUndoableElement** - Interface for elements that support undo
- **UndoManager** - Default implementation with configurable limits

## Quick Start

### Enable Undo for an Element

```csharp
using FrameworkInterfaces;
using FrameworkInterfaces.Undo;

public class MyElement : ElementBase, IUndoableElement
{
    private string _customValue;

    public MyElement(string name, IElementCollection parent)
        : base(name, parent)
    {
    }

    public string CustomValue
    {
        get => _customValue;
        set => SetPropertyWithUndo(ref _customValue, value, nameof(CustomValue));
    }

    // IUndoableElement is already implemented by ElementBase
    // UndoManager property is automatically available
}
```

That's it! The `SetPropertyWithUndo` method (inherited from `ElementBase`) automatically:
1. Creates a `PropertyChangeAction`
2. Pushes it to the undo stack
3. Sets the property value
4. Notifies property changed

### Keyboard Shortcuts

When using `MainWindow`, these shortcuts work automatically:
- **Ctrl+Z** - Undo
- **Ctrl+Y** - Redo

## Core Concepts

### UndoManager

Each undoable element has its own `UndoManager`:

```csharp
// Access the undo manager
IUndoManager undoManager = myElement.UndoManager;

// Check if undo/redo is available
bool canUndo = undoManager.CanUndo;
bool canRedo = undoManager.CanRedo;

// Perform undo/redo
undoManager.Undo();
undoManager.Redo();

// Clear history
undoManager.Clear();
```

### Action Types

| Action | Purpose |
|--------|---------|
| `PropertyChangeAction` | Single property change |
| `CompositeAction` | Group of related actions |
| `AddElementAction` | Element added to collection |
| `RemoveElementAction` | Element removed from collection |
| `MoveElementAction` | Element moved within/between collections |

## Property Change Tracking

### Using SetPropertyWithUndo

The simplest way to track property changes:

```csharp
private int _count;

public int Count
{
    get => _count;
    set => SetPropertyWithUndo(ref _count, value, nameof(Count));
}
```

### Manual Property Tracking

For more control, create actions manually:

```csharp
private string _name;

public string Name
{
    get => _name;
    set
    {
        if (_name == value) return;

        var action = new PropertyChangeAction(
            this,
            nameof(Name),
            _name,      // old value
            value,      // new value
            v => _name = (string)v,  // setter
            () => NotifyPropertyChanged(nameof(Name))
        );

        if (IsUndoEnabled)
        {
            UndoManager.Execute(action);
        }
        else
        {
            action.Execute();
        }
    }
}
```

## Composite Actions (Batch Operations)

Group multiple changes into a single undoable action:

```csharp
// Start a batch
UndoManager.BeginBatch("Update Element Properties");

// Make multiple changes - each tracked separately
element.Name = "New Name";
element.Description = "New Description";
element.Value = 42;

// End batch - all changes become one undo action
UndoManager.EndBatch();

// Now Ctrl+Z undoes ALL three changes at once
```

### Nested Batches

Batches can be nested:

```csharp
UndoManager.BeginBatch("Outer Operation");

    element.Name = "Name 1";

    UndoManager.BeginBatch("Inner Operation");
        element.Value = 100;
        element.Value = 200;
    UndoManager.EndBatch();

    element.Description = "Done";

UndoManager.EndBatch();  // Single undo for everything
```

## Collection Operations

### Tracking Add/Remove

```csharp
public class MyCollection : ElementCollectionBase<MyElement>
{
    public override void Add(MyElement element)
    {
        if (IsUndoEnabled)
        {
            var action = new AddElementAction(element, this, Count);
            UndoManager.Execute(action);
        }
        else
        {
            base.Add(element);
        }
    }

    public override void Remove(MyElement element)
    {
        if (IsUndoEnabled)
        {
            int index = IndexOf(element);
            var action = new RemoveElementAction(element, this, index);
            UndoManager.Execute(action);
        }
        else
        {
            base.Remove(element);
        }
    }
}
```

## Temporarily Disabling Undo

Disable undo recording during bulk operations or loading:

```csharp
// Disable undo recording
element.IsUndoEnabled = false;

// Make changes without tracking
element.Name = "Loaded Name";
element.Value = loadedValue;

// Re-enable undo
element.IsUndoEnabled = true;
```

### During Undo/Redo Operations

The undo system automatically disables recording during undo/redo to prevent recursive tracking.

## UI Integration

### MainWindow Integration

`MainWindow` automatically:
- Binds Ctrl+Z to Undo
- Binds Ctrl+Y to Redo
- Shows undo/redo buttons (configurable via `UserSettings.ShowUndoRedoButtons`)

### Custom UI

Create your own undo/redo buttons:

```csharp
// In your ViewModel or code-behind
public ICommand UndoCommand => new RelayCommand(
    () => _element.UndoManager.Undo(),
    () => _element.UndoManager.CanUndo);

public ICommand RedoCommand => new RelayCommand(
    () => _element.UndoManager.Redo(),
    () => _element.UndoManager.CanRedo);
```

### Displaying Undo History

```csharp
// Get descriptions of pending undo actions
foreach (var description in undoManager.UndoDescriptions)
{
    Console.WriteLine($"Undo: {description}");
}

// Get descriptions of pending redo actions
foreach (var description in undoManager.RedoDescriptions)
{
    Console.WriteLine($"Redo: {description}");
}
```

## Configuration

### Stack Size Limits

```csharp
// Set maximum undo stack size (default is unlimited)
undoManager.MaxUndoLevels = 100;
```

### Events

```csharp
// Subscribe to state changes
undoManager.StateChanged += (sender, e) =>
{
    // Update UI button states
    undoButton.IsEnabled = undoManager.CanUndo;
    redoButton.IsEnabled = undoManager.CanRedo;
};
```

## Creating Custom Actions

Implement `IUndoableAction` for custom undo behavior:

```csharp
public class CustomAction : IUndoableAction
{
    private readonly MyObject _target;
    private readonly object _oldState;
    private readonly object _newState;

    public CustomAction(MyObject target, object oldState, object newState)
    {
        _target = target;
        _oldState = oldState;
        _newState = newState;
    }

    public string Description => "Custom Operation";

    public void Execute()
    {
        _target.ApplyState(_newState);
    }

    public void Undo()
    {
        _target.ApplyState(_oldState);
    }
}

// Use it
var action = new CustomAction(myObject, oldState, newState);
undoManager.Execute(action);
```

## Best Practices

### DO

- Use `SetPropertyWithUndo` for simple properties
- Group related changes with `BeginBatch`/`EndBatch`
- Disable undo during file loading
- Provide meaningful action descriptions
- Clear undo history after saving (optional)

### DON'T

- Track every tiny change (use batching)
- Forget to call `EndBatch` after `BeginBatch`
- Manually manipulate undo stacks
- Track changes during undo/redo operations

## Demo Application

See `Demo_FrameworkUI/UI/UndoDemoControl.xaml` for a complete working example that demonstrates:

- Property change tracking
- Batch operations
- Undo/redo stack visualization
- Keyboard shortcuts

## Troubleshooting

### Undo not working

1. Verify element implements `IUndoableElement`
2. Check `IsUndoEnabled` is `true`
3. Ensure using `SetPropertyWithUndo` or manual action tracking
4. Verify `MainWindow` is using `GetControlElement` correctly

### Undo undoes too much/too little

Use batching to control granularity:

```csharp
// Too granular - each keystroke is separate undo
textBox.TextChanged += (s, e) => element.Text = textBox.Text;

// Better - batch on focus lost
textBox.LostFocus += (s, e) => element.Text = textBox.Text;
```

### Memory issues with large undo history

Set a reasonable limit:

```csharp
undoManager.MaxUndoLevels = 50;
```
