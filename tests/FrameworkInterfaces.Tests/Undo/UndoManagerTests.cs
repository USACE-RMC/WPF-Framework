using Xunit;
using FrameworkInterfaces.Undo;
using FrameworkInterfaces.Undo.Actions;
using System.ComponentModel;

namespace FrameworkInterfaces.Tests.Undo
{
    public class UndoManagerTests
    {
        #region Initial State Tests

        [Fact]
        public void NewUndoManager_CannotUndo()
        {
            var manager = new UndoManager();
            Assert.False(manager.CanUndo);
        }

        [Fact]
        public void NewUndoManager_CannotRedo()
        {
            var manager = new UndoManager();
            Assert.False(manager.CanRedo);
        }

        [Fact]
        public void NewUndoManager_UndoDescriptionIsNull()
        {
            var manager = new UndoManager();
            Assert.Null(manager.UndoDescription);
        }

        [Fact]
        public void NewUndoManager_RedoDescriptionIsNull()
        {
            var manager = new UndoManager();
            Assert.Null(manager.RedoDescription);
        }

        [Fact]
        public void NewUndoManager_HasNotChangedSinceSave()
        {
            var manager = new UndoManager();
            Assert.False(manager.HasChangedSinceSave);
        }

        [Fact]
        public void NewUndoManager_IsNotExecutingAction()
        {
            var manager = new UndoManager();
            Assert.False(manager.IsExecutingAction);
        }

        [Fact]
        public void NewUndoManager_UndoStackIsEmpty()
        {
            var manager = new UndoManager();
            Assert.Empty(manager.UndoStack);
        }

        [Fact]
        public void NewUndoManager_RedoStackIsEmpty()
        {
            var manager = new UndoManager();
            Assert.Empty(manager.RedoStack);
        }

        [Fact]
        public void NewUndoManager_MaxUndoLevelsDefault()
        {
            var manager = new UndoManager();
            Assert.Equal(100, manager.MaxUndoLevels);
        }

        #endregion

        #region ExecuteAction Tests

        [Fact]
        public void ExecuteAction_ExecutesTheAction()
        {
            var manager = new UndoManager();
            var executed = false;
            var action = new DelegateAction("Test", () => executed = true, () => { });

            manager.ExecuteAction(action);

            Assert.True(executed);
        }

        [Fact]
        public void ExecuteAction_AddsToUndoStack()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });

            manager.ExecuteAction(action);

            Assert.True(manager.CanUndo);
            Assert.Single(manager.UndoStack);
        }

        [Fact]
        public void ExecuteAction_ClearsRedoStack()
        {
            var manager = new UndoManager();
            var action1 = new DelegateAction("Test1", () => { }, () => { });
            var action2 = new DelegateAction("Test2", () => { }, () => { });

            manager.ExecuteAction(action1);
            manager.Undo();
            manager.ExecuteAction(action2);

            Assert.False(manager.CanRedo);
        }

        [Fact]
        public void ExecuteAction_NullAction_ThrowsArgumentNullException()
        {
            var manager = new UndoManager();

            Assert.Throws<ArgumentNullException>(() => manager.ExecuteAction(null!));
        }

        [Fact]
        public void ExecuteAction_SetsHasChangedSinceSave()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });

            manager.ExecuteAction(action);

            Assert.True(manager.HasChangedSinceSave);
        }

        #endregion

        #region RecordAction Tests

        [Fact]
        public void RecordAction_DoesNotExecuteTheAction()
        {
            var manager = new UndoManager();
            var executed = false;
            var action = new DelegateAction("Test", () => executed = true, () => { });

            manager.RecordAction(action);

            Assert.False(executed);
        }

        [Fact]
        public void RecordAction_AddsToUndoStack()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });

            manager.RecordAction(action);

            Assert.True(manager.CanUndo);
        }

        [Fact]
        public void RecordAction_NullAction_ThrowsArgumentNullException()
        {
            var manager = new UndoManager();

            Assert.Throws<ArgumentNullException>(() => manager.RecordAction(null!));
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_CallsUndoOnAction()
        {
            var manager = new UndoManager();
            var undoCalled = false;
            var action = new DelegateAction("Test", () => { }, () => undoCalled = true);
            manager.ExecuteAction(action);

            manager.Undo();

            Assert.True(undoCalled);
        }

        [Fact]
        public void Undo_MovesActionToRedoStack()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);

            manager.Undo();

            Assert.False(manager.CanUndo);
            Assert.True(manager.CanRedo);
        }

        [Fact]
        public void Undo_WhenNoActions_DoesNothing()
        {
            var manager = new UndoManager();

            var exception = Record.Exception(() => manager.Undo());

            Assert.Null(exception);
            Assert.False(manager.CanRedo);
        }

        [Fact]
        public void Undo_RaisesStateChanged()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);
            var stateChanged = false;
            manager.StateChanged += (s, e) => stateChanged = true;

            manager.Undo();

            Assert.True(stateChanged);
        }

        #endregion

        #region Redo Tests

        [Fact]
        public void Redo_CallsExecuteOnAction()
        {
            var manager = new UndoManager();
            var executeCount = 0;
            var action = new DelegateAction("Test", () => executeCount++, () => { });
            manager.ExecuteAction(action);
            manager.Undo();

            manager.Redo();

            Assert.Equal(2, executeCount);
        }

        [Fact]
        public void Redo_MovesActionToUndoStack()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);
            manager.Undo();

            manager.Redo();

            Assert.True(manager.CanUndo);
            Assert.False(manager.CanRedo);
        }

        [Fact]
        public void Redo_WhenNoActions_DoesNothing()
        {
            var manager = new UndoManager();

            var exception = Record.Exception(() => manager.Redo());

            Assert.Null(exception);
        }

        #endregion

        #region UndoTo/RedoTo Tests

        [Fact]
        public void UndoTo_UndoesMultipleActions()
        {
            var manager = new UndoManager();
            var undoCount = 0;
            var action1 = new DelegateAction("Test1", () => { }, () => undoCount++);
            var action2 = new DelegateAction("Test2", () => { }, () => undoCount++);
            var action3 = new DelegateAction("Test3", () => { }, () => undoCount++);
            manager.ExecuteAction(action1);
            manager.ExecuteAction(action2);
            manager.ExecuteAction(action3);

            manager.UndoTo(action1);

            Assert.Equal(3, undoCount);
            Assert.False(manager.CanUndo);
        }

        [Fact]
        public void UndoTo_NullAction_ThrowsArgumentNullException()
        {
            var manager = new UndoManager();

            Assert.Throws<ArgumentNullException>(() => manager.UndoTo(null!));
        }

        [Fact]
        public void RedoTo_RedoesMultipleActions()
        {
            var manager = new UndoManager();
            var executeCount = 0;
            var action1 = new DelegateAction("Test1", () => executeCount++, () => { });
            var action2 = new DelegateAction("Test2", () => executeCount++, () => { });
            var action3 = new DelegateAction("Test3", () => executeCount++, () => { });
            manager.ExecuteAction(action1);
            manager.ExecuteAction(action2);
            manager.ExecuteAction(action3);
            manager.Undo();
            manager.Undo();
            manager.Undo();
            executeCount = 0;

            manager.RedoTo(action3);

            Assert.Equal(3, executeCount);
            Assert.False(manager.CanRedo);
        }

        [Fact]
        public void RedoTo_NullAction_ThrowsArgumentNullException()
        {
            var manager = new UndoManager();

            Assert.Throws<ArgumentNullException>(() => manager.RedoTo(null!));
        }

        #endregion

        #region Clear Tests

        [Fact]
        public void Clear_ClearsUndoStack()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);

            manager.Clear();

            Assert.False(manager.CanUndo);
            Assert.Empty(manager.UndoStack);
        }

        [Fact]
        public void Clear_ClearsRedoStack()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);
            manager.Undo();

            manager.Clear();

            Assert.False(manager.CanRedo);
            Assert.Empty(manager.RedoStack);
        }

        [Fact]
        public void Clear_ResetsHasChangedSinceSave()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);

            manager.Clear();

            Assert.False(manager.HasChangedSinceSave);
        }

        #endregion

        #region SavePoint Tests

        [Fact]
        public void MarkSavePoint_ResetsHasChangedSinceSave()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);

            manager.MarkSavePoint();

            Assert.False(manager.HasChangedSinceSave);
        }

        [Fact]
        public void AfterMarkSavePoint_NewActionSetsHasChangedSinceSave()
        {
            var manager = new UndoManager();
            var action1 = new DelegateAction("Test1", () => { }, () => { });
            var action2 = new DelegateAction("Test2", () => { }, () => { });
            manager.ExecuteAction(action1);
            manager.MarkSavePoint();

            manager.ExecuteAction(action2);

            Assert.True(manager.HasChangedSinceSave);
        }

        [Fact]
        public void UndoToSavePoint_ResetsHasChangedSinceSave()
        {
            var manager = new UndoManager();
            var action1 = new DelegateAction("Test1", () => { }, () => { });
            var action2 = new DelegateAction("Test2", () => { }, () => { });
            manager.ExecuteAction(action1);
            manager.MarkSavePoint();
            manager.ExecuteAction(action2);

            manager.Undo();

            Assert.False(manager.HasChangedSinceSave);
        }

        #endregion

        #region Transaction Tests

        [Fact]
        public void BeginTransaction_GroupsActionsIntoSingleUndo()
        {
            var manager = new UndoManager();

            using (manager.BeginTransaction("Grouped"))
            {
                manager.RecordAction(new DelegateAction("Test1", () => { }, () => { }));
                manager.RecordAction(new DelegateAction("Test2", () => { }, () => { }));
                manager.RecordAction(new DelegateAction("Test3", () => { }, () => { }));
            }

            Assert.Single(manager.UndoStack);
            Assert.Equal("Grouped", manager.UndoDescription);
        }

        [Fact]
        public void BeginTransaction_NestedTransaction_ReturnsNoOpDisposable()
        {
            var manager = new UndoManager();

            using (manager.BeginTransaction("Outer"))
            {
                manager.RecordAction(new DelegateAction("Test1", () => { }, () => { }));

                using (manager.BeginTransaction("Inner"))
                {
                    manager.RecordAction(new DelegateAction("Test2", () => { }, () => { }));
                }

                manager.RecordAction(new DelegateAction("Test3", () => { }, () => { }));
            }

            Assert.Single(manager.UndoStack);
            Assert.Equal("Outer", manager.UndoDescription);
        }

        [Fact]
        public void CommitTransaction_WithNoActions_DoesNotAddToUndoStack()
        {
            var manager = new UndoManager();

            using (manager.BeginTransaction("Empty"))
            {
                // No actions
            }

            Assert.Empty(manager.UndoStack);
        }

        [Fact]
        public void RollbackTransaction_UndoesAllActionsInReverseOrder()
        {
            var manager = new UndoManager();
            var undoOrder = new List<int>();

            manager.BeginTransaction("Test");
            manager.ExecuteAction(new DelegateAction("Test1", () => { }, () => undoOrder.Add(1)));
            manager.ExecuteAction(new DelegateAction("Test2", () => { }, () => undoOrder.Add(2)));
            manager.ExecuteAction(new DelegateAction("Test3", () => { }, () => undoOrder.Add(3)));

            manager.RollbackTransaction();

            Assert.Empty(manager.UndoStack);
            Assert.Equal(new[] { 3, 2, 1 }, undoOrder);
        }

        [Fact]
        public void RollbackTransaction_WithNoTransaction_DoesNothing()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);

            var exception = Record.Exception(() => manager.RollbackTransaction());

            Assert.Null(exception);
            Assert.Single(manager.UndoStack);
        }

        #endregion

        #region MaxUndoLevels Tests

        [Fact]
        public void MaxUndoLevels_TrimsUndoStack()
        {
            var manager = new UndoManager();
            manager.MaxUndoLevels = 3;

            for (int i = 0; i < 5; i++)
            {
                manager.ExecuteAction(new DelegateAction($"Test{i}", () => { }, () => { }));
            }

            Assert.Equal(3, manager.UndoStack.Count);
        }

        [Fact]
        public void MaxUndoLevels_SetToLessThanOne_SetsToOne()
        {
            var manager = new UndoManager();

            manager.MaxUndoLevels = 0;

            Assert.Equal(1, manager.MaxUndoLevels);
        }

        [Fact]
        public void MaxUndoLevels_SetToNegative_SetsToOne()
        {
            var manager = new UndoManager();

            manager.MaxUndoLevels = -5;

            Assert.Equal(1, manager.MaxUndoLevels);
        }

        #endregion

        #region PropertyChanged Tests

        [Fact]
        public void ExecuteAction_RaisesPropertyChangedForCanUndo()
        {
            var manager = new UndoManager();
            var changedProperties = new List<string>();
            manager.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName!);
            var action = new DelegateAction("Test", () => { }, () => { });

            manager.ExecuteAction(action);

            Assert.Contains(nameof(IUndoManager.CanUndo), changedProperties);
        }

        [Fact]
        public void Undo_RaisesPropertyChangedForCanRedo()
        {
            var manager = new UndoManager();
            var action = new DelegateAction("Test", () => { }, () => { });
            manager.ExecuteAction(action);
            var changedProperties = new List<string>();
            manager.PropertyChanged += (s, e) => changedProperties.Add(e.PropertyName!);

            manager.Undo();

            Assert.Contains(nameof(IUndoManager.CanRedo), changedProperties);
        }

        #endregion
    }
}
