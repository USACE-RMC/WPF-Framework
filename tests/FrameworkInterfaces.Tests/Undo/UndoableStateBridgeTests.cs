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
            var bridge = new UndoableStateBridge(source, undoManager);

            Assert.Same(source, bridge.Source);
        }

        [Fact]
        public void Constructor_SetsUndoManager()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            Assert.Same(undoManager, bridge.UndoManager);
        }

        [Fact]
        public void Constructor_NullSource_ThrowsArgumentNullException()
        {
            var undoManager = new UndoManager();

            Assert.Throws<ArgumentNullException>(() =>
                new UndoableStateBridge(null!, undoManager));
        }

        [Fact]
        public void Constructor_NullUndoManager_ThrowsArgumentNullException()
        {
            var source = new TestNotifyObject();

            Assert.Throws<ArgumentNullException>(() =>
                new UndoableStateBridge(source, null!));
        }

        [Fact]
        public void Constructor_SubscribesToPropertyChanged()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

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
            var bridge = new UndoableStateBridge(source, undoManager);

            source.Name = "Changed";

            Assert.True(undoManager.CanUndo);
            Assert.Single(undoManager.UndoStack);
        }

        [Fact]
        public void PropertyChange_UndoRestoresOldValue()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            source.Name = "Changed";
            undoManager.Undo();

            Assert.Equal("Initial", source.Name);
        }

        [Fact]
        public void MultiplePropertyChanges_AllCanBeUndone()
        {
            var source = new TestNotifyObject { Name = "A", Value = 1 };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

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
            var bridge = new UndoableStateBridge(source, undoManager);

            source.Name = "Changed";
            undoManager.Undo();
            undoManager.Redo();

            Assert.Equal("Changed", source.Name);
        }

        #endregion

        #region Paused Tests

        [Fact]
        public void IsPaused_InitiallyFalse()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            Assert.False(bridge.IsPaused);
        }

        [Fact]
        public void Pause_SetsIsPausedToTrue()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            bridge.Pause();

            Assert.True(bridge.IsPaused);
        }

        [Fact]
        public void Resume_SetsIsPausedToFalse()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);
            bridge.Pause();

            bridge.Resume();

            Assert.False(bridge.IsPaused);
        }

        [Fact]
        public void WhenPaused_PropertyChangesNotRecorded()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);
            bridge.Pause();

            source.Name = "Changed";

            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void AfterResume_PropertyChangesRecorded()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);
            bridge.Pause();
            source.Name = "DuringPause";
            bridge.Resume();

            source.Name = "AfterResume";

            Assert.True(undoManager.CanUndo);
        }

        #endregion

        #region Property Filter Tests

        [Fact]
        public void MonitoredProperties_WhenSetToNull_MonitorsAllProperties()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager)
            {
                MonitoredProperties = null
            };

            source.Name = "Test1";
            source.Value = 42;

            Assert.Equal(2, undoManager.UndoStack.Count);
        }

        [Fact]
        public void MonitoredProperties_OnlyMonitorsSpecifiedProperties()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager)
            {
                MonitoredProperties = new HashSet<string> { nameof(TestNotifyObject.Name) }
            };

            source.Name = "Test1";
            source.Value = 42;

            Assert.Single(undoManager.UndoStack);
        }

        [Fact]
        public void IgnoredProperties_ExcludesSpecifiedProperties()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager)
            {
                IgnoredProperties = new HashSet<string> { nameof(TestNotifyObject.Value) }
            };

            source.Name = "Test1";
            source.Value = 42;

            Assert.Single(undoManager.UndoStack);
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_UnsubscribesFromPropertyChanged()
        {
            var source = new TestNotifyObject { Name = "Initial" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            bridge.Dispose();
            source.Name = "Changed";

            Assert.False(undoManager.CanUndo);
        }

        [Fact]
        public void Dispose_MultipleCalls_DoNotThrow()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            var exception = Record.Exception(() =>
            {
                bridge.Dispose();
                bridge.Dispose();
            });

            Assert.Null(exception);
        }

        #endregion

        #region UseDetailedDescriptions Tests

        [Fact]
        public void UseDetailedDescriptions_DefaultIsFalse()
        {
            var source = new TestNotifyObject();
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager);

            Assert.False(bridge.UseDetailedDescriptions);
        }

        [Fact]
        public void UseDetailedDescriptions_WhenTrue_IncludesValuesInDescription()
        {
            var source = new TestNotifyObject { Name = "Before" };
            var undoManager = new UndoManager();
            var bridge = new UndoableStateBridge(source, undoManager)
            {
                UseDetailedDescriptions = true
            };

            source.Name = "After";

            var description = undoManager.UndoDescription;
            Assert.NotNull(description);
            Assert.Contains("Before", description);
            Assert.Contains("After", description);
        }

        #endregion
    }
}
