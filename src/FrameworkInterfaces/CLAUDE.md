## Purpose
Core contracts and base classes for the framework: messaging, undo/redo, project/element hierarchy, and shared utilities.

## Key Files
- `Messaging/Messenger.cs` — Singleton message hub. Messages keyed by (source, code) to prevent duplicates.
- `Messaging/BasicMessageItem.cs` — Concrete IMessageItem. Equality based on Code + Source identity.
- `Undo/UndoManager.cs` — Per-document undo/redo stacks (VS-style). 100 max levels. Supports transactions and action merging.
- `Undo/UndoableStateBridge.cs` — Monitors INotifyPropertyChanged via reflection, auto-creates PropertyChangeActions with shadow values.
- `Undo/UndoableCollectionBridge.cs` — Monitors INotifyCollectionChanged, maintains shadow copy for Reset, uses dynamic dispatch.
- `Undo/Actions/PropertyChangeAction.cs` — 500ms merge window for rapid successive changes to the same property.
- `Undo/Actions/CompositeAction.cs` — Groups actions from BeginTransaction into a single undoable unit.
- `Project/IProject.cs` — Top-level: extends IMetaData, ISave. Owns ElementCollections, AvalonDockLayout, ProjectImage.
- `Project/ElementBase.cs` — Abstract IElement base with undo support, dirty tracking, name validation (ERR-001..004).
- `Project/ElementBaseBuff.cs` — Simplified element base WITHOUT undo. Intentional duplication from ElementBase.
- `Project/ElementCollectionBase.cs` — Abstract IList<IElement> with undo-aware Add/Remove/Move recording.
- `Utilities/Methods.cs` — SetString/SetBoolean/etc. helpers with CallerMemberName auto property name.

## Dependencies
- .NET 9 (net9.0-windows)
- System.Drawing.Common (for Bitmap in IProject)
- No internal project dependencies (this is the leaf dependency)

## Patterns
- **Singleton messaging**: `Messenger.GetInstance()` with lazy thread-safe initialization.
- **Per-element undo**: Each element/collection/project owns its own UndoManager instance (lazy-created).
- **Bridge pattern for undo**: UndoableStateBridge and UndoableCollectionBridge wrap external objects to record undo actions.
- **Transaction grouping**: `undoManager.BeginTransaction()` returns IDisposable; dispose commits as CompositeAction.
- **ISave dirty tracking**: IsDirty property with PreviewObjectSaved/ObjectSaved events on all persistable objects.
- **SetProperty helpers**: Methods.SetString/SetBoolean etc. use CallerMemberName for PropertyChanged notifications.

## Gotchas
- Event-type messages auto-increment their Code property for uniqueness — do not rely on Code stability for Event messages.
- UndoableCollectionBridge uses `dynamic` dispatch because some collections hide base methods with `new` keyword.
- UndoableStateBridge checks `undoManager.IsExecutingAction` to skip recording during undo/redo replay.
- PropertyChangeAction merges only within 500ms AND same property AND same target — stale merges are rejected.
- ElementBaseBuff intentionally duplicates ElementBase code without undo — keep both in sync manually.
- UndoableStateBridge.SuspendRecording() returns IDisposable for temporary suppression of undo recording.
