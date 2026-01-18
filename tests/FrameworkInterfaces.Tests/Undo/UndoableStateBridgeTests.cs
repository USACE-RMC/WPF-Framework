using Xunit;
using FrameworkInterfaces.Undo;
using System.ComponentModel;

namespace FrameworkInterfaces.Tests.Undo
{
    public class UndoableStateBridgeTests
    {
        #region Test Helpers

        private class TestNotifyObject : INotifyPropertyChanged
        {
            private string _name = string.Empty;
            private int _value;
            private double _amount;

            public string Name
            {
                get => _name;
                set
                {
                    if (_name != value)
                    {
                        _name = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                    }
                }
            }

            public int Value
            {
                get => _value;
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    }
                }
            }

            public double Amount
            {
                get => _amount;
                set
                {
                    if (_amount != value)
                    {
                        _amount = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
                    }
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_SetsSource()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            Assert.Same(source, bridge.Source);
        }

        [Fact]
        public void Constructor_SetsSourceDescription()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager, "test source");

            Assert.Equal("test source", bridge.SourceDescription);
        }

        [Fact]
        public void Constructor_NullSource_ThrowsArgumentNullException()
        {
            var undoManager = new UndoManager();

            Assert.Throws<ArgumentNullException>(() =>
                new UndoableStateBridge(null!, () => undoManager));
        }

        [Fact]
        public void Constructor_NullGetUndoManager_ThrowsArgumentNullException()
        {
            var source = new TestNotifyObject();

            Assert.Throws<ArgumentNullException>(() =>
                new UndoableStateBridge(source, null!));
        }

        [Fact]
        public void Constructor_BothIncludedAndExcluded_ThrowsArgumentException()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();

            Assert.Throws<ArgumentException>(() =>
                new UndoableStateBridge(
                    source,
                    () => undoManager,
                    "settings",
                    null,
                    includedProperties: new[] { "Name" },
                    excludedProperties: new[] { "Value" }));
        }

        [Fact]
        public void Constructor_SubscribesToPropertyChanged()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            source.Name = "Test";

            Assert.True(undoManager.CanUndo);
        }

        #endregion

        #region Property Change Recording Tests

        [Fact]
        public void PropertyChange_RecordsUndoableAction()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            source.Name = "Changed";

            Assert.True(undoManager.CanUndo);
            Assert.Single(undoManager.UndoStack);
        }

        [Fact]
        public void PropertyChange_UndoRestoresOldValue()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            source.Name = "Changed";
            undoManager.Undo();

            Assert.Equal("Initial", source.Name);
        }

        [Fact]
        public void MultiplePropertyChanges_AllCanBeUndone()
        {
            var source = new TestNotifyObject { Name = "A", Value = 1 };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            source.Name = "B";
            source.Value = 2;
            source.Name = "C";

            Assert.Equal(3, undoManager.UndoStack.Count);

            undoManager.Undo();
            Assert.Equal("B", source.Name);

            undoManager.Undo();
            Assert.Equal(1, source.Value);

            undoManager.Undo();
            Assert.Equal("A", source.Name);
        }

        [Fact]
        public void PropertyChange_Redo_ReappliesValue()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            source.Name = "Changed";
            undoManager.Undo();
            undoManager.Redo();

            Assert.Equal("Changed", source.Name);
        }

        [Fact]
        public void PropertyChange_WhenUndoManagerReturnsNull_DoesNotRecord()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var bridge = new UndoableStateBridge(source, () => null);

            var exception = Record.Exception(() => source.Name = "Changed");

            Assert.Null(exception);
        }

        #endregion

        #region SuspendRecording Tests

        [Fact]
        public void SuspendRecording_PropertyChangesNotRecorded()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            using (bridge.SuspendRecording())
            {
                source.Name = "Changed";
            }

            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void SuspendRecording_AfterDispose_RecordingResumes()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            using (bridge.SuspendRecording())
            {
                source.Name = "DuringSuspension";
            }

            source.Name = "AfterSuspension";

            Assert.True(undoManager.CanUndo);
            Assert.Single(undoManager.UndoStack);
        }

        [Fact]
        public void SuspendRecording_UpdatesShadowValues()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            using (bridge.SuspendRecording())
            {
                source.Name = "NewBaseline";
            }

            source.Name = "Changed";
            undoManager.Undo();

            // Should undo to "NewBaseline" not "Initial"
            Assert.Equal("NewBaseline", source.Name);
        }

        #endregion

        #region Property Filter Tests - Included

        [Fact]
        public void IncludedProperties_OnlyMonitorsSpecifiedProperties()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(
                source,
                () => undoManager,
                "settings",
                null,
                includedProperties: new[] { nameof(TestNotifyObject.Name) });

            source.Name = "Test1";
            source.Value = 42;

            Assert.Single(undoManager.UndoStack);
        }

        [Fact]
        public void IncludedProperties_IgnoresNonIncludedProperties()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(
                source,
                () => undoManager,
                "settings",
                null,
                includedProperties: new[] { nameof(TestNotifyObject.Value) });

            source.Name = "Test";

            Assert.False(undoManager.CanUndo);
        }

        #endregion

        #region Property Filter Tests - Excluded

        [Fact]
        public void ExcludedProperties_ExcludesSpecifiedProperties()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(
                source,
                () => undoManager,
                "settings",
                null,
                null,
                excludedProperties: new[] { nameof(TestNotifyObject.Value) });

            source.Name = "Test1";
            source.Value = 42;

            Assert.Single(undoManager.UndoStack);
        }

        [Fact]
        public void ExcludeProperty_AddsToExclusionList()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            bridge.ExcludeProperty(nameof(TestNotifyObject.Value));
            source.Value = 42;

            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void ExcludeProperty_WhenUsingInclusionList_ThrowsInvalidOperationException()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(
                source,
                () => undoManager,
                "settings",
                null,
                includedProperties: new[] { nameof(TestNotifyObject.Name) });

            Assert.Throws<InvalidOperationException>(() =>
                bridge.ExcludeProperty(nameof(TestNotifyObject.Value)));
        }

        [Fact]
        public void IncludeProperty_RemovesFromExclusionList()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(
                source,
                () => undoManager,
                "settings",
                null,
                null,
                excludedProperties: new[] { nameof(TestNotifyObject.Value) });

            bridge.IncludeProperty(nameof(TestNotifyObject.Value));
            source.Value = 42;

            Assert.True(undoManager.CanUndo);
        }

        [Fact]
        public void IncludeProperty_WhenUsingInclusionList_ThrowsInvalidOperationException()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(
                source,
                () => undoManager,
                "settings",
                null,
                includedProperties: new[] { nameof(TestNotifyObject.Name) });

            Assert.Throws<InvalidOperationException>(() =>
                bridge.IncludeProperty(nameof(TestNotifyObject.Value)));
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_UnsubscribesFromPropertyChanged()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            bridge.Dispose();
            source.Name = "Changed";

            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void Dispose_SetsIsDisposedToTrue()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            bridge.Dispose();

            Assert.True(bridge.IsDisposed);
        }

        [Fact]
        public void Dispose_MultipleCalls_DoNotThrow()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            var exception = Record.Exception(() =>
            {
                bridge.Dispose();
                bridge.Dispose();
            });

            Assert.Null(exception);
        }

        [Fact]
        public void IsDisposed_InitiallyFalse()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            Assert.False(bridge.IsDisposed);
        }

        #endregion

        #region UseDetailedDescriptions Tests

        [Fact]
        public void UseDetailedDescriptions_DefaultIsTrue()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            Assert.True(bridge.UseDetailedDescriptions);
        }

        [Fact]
        public void UseDetailedDescriptions_CanBeSetToFalse()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            bridge.UseDetailedDescriptions = false;

            Assert.False(bridge.UseDetailedDescriptions);
        }

        [Fact]
        public void UseDetailedDescriptions_WhenTrue_IncludesValuesInDescription()
        {
            var source = new TestNotifyObject { Name = "Before" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager)
            {
                UseDetailedDescriptions = true
            };

            source.Name = "After";

            var description = undoManager.UndoDescription;
            Assert.NotNull(description);
            // Description should mention property name at least
            Assert.Contains("Name", description, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UseDetailedDescriptions_WhenFalse_UsesSimpleDescription()
        {
            var source = new TestNotifyObject { Name = "Before" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager, "test settings")
            {
                UseDetailedDescriptions = false
            };

            source.Name = "After";

            var description = undoManager.UndoDescription;
            Assert.NotNull(description);
            Assert.Contains("test settings", description);
        }

        #endregion

        #region RefreshShadowValues Tests

        [Fact]
        public void RefreshShadowValues_UpdatesShadowToCurrentValues()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            // Change without recording (simulate external modification)
            using (bridge.SuspendRecording())
            {
                source.Name = "External";
            }

            bridge.RefreshShadowValues();
            source.Name = "Changed";
            undoManager.Undo();

            Assert.Equal("External", source.Name);
        }

        #endregion

        #region Same Value Tests

        [Fact]
        public void PropertyChange_SameValue_DoesNotRecordAction()
        {
            var source = new TestNotifyObject { Name = "Same" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            // This won't trigger PropertyChanged since the TestNotifyObject checks for equality
            source.Name = "Same";

            Assert.False(undoManager.CanUndo);
        }

        #endregion

        #region IsExecutingAction Tests

        [Fact]
        public void WhenUndoManagerIsExecutingAction_DoesNotRecordAction()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, () => undoManager);

            source.Name = "Changed";
            var initialCount = undoManager.UndoStack.Count;

            // During undo, property changes should not create new actions
            undoManager.Undo();

            // Should still only have the original action count (minus the undone one)
            Assert.Equal(initialCount - 1, undoManager.UndoStack.Count);
        }

        #endregion
    }
}
