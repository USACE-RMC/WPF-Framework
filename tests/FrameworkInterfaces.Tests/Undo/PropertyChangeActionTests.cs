using Xunit;
using FrameworkInterfaces.Undo.Actions;
using System.ComponentModel;

namespace FrameworkInterfaces.Tests.Undo
{
    public class PropertyChangeActionTests
    {
        #region Test Helpers

        private class TestObject : INotifyPropertyChanged
        {
            private string _name = string.Empty;
            private int _value;

            public string Name
            {
                get => _name;
                set
                {
                    _name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                }
            }

            public int Value
            {
                get => _value;
                set
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_SetsProperties()
        {
            var target = new TestObject { Name = "Test" };
            var action = new PropertyChangeAction(target, nameof(TestObject.Name), "Old", "New");

            Assert.Same(target, action.Target);
            Assert.Equal(nameof(TestObject.Name), action.PropertyName);
            Assert.Equal("Old", action.OldValue);
            Assert.Equal("New", action.NewValue);
        }

        [Fact]
        public void Constructor_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new PropertyChangeAction(null!, "Name", "Old", "New"));
        }

        [Fact]
        public void Constructor_NullPropertyName_ThrowsArgumentNullException()
        {
            var target = new TestObject();

            Assert.Throws<ArgumentNullException>(() =>
                new PropertyChangeAction(target, null!, "Old", "New"));
        }

        [Fact]
        public void Constructor_InvalidPropertyName_ThrowsArgumentException()
        {
            var target = new TestObject();

            Assert.Throws<ArgumentException>(() =>
                new PropertyChangeAction(target, "NonExistentProperty", "Old", "New"));
        }

        [Fact]
        public void Constructor_SetsTimestamp()
        {
            var before = DateTime.Now;
            var target = new TestObject();
            var action = new PropertyChangeAction(target, nameof(TestObject.Name), "Old", "New");
            var after = DateTime.Now;

            Assert.True(action.Timestamp >= before);
            Assert.True(action.Timestamp <= after);
        }

        #endregion

        #region Description Tests

        [Fact]
        public void Description_ReturnsPropertyChangeName()
        {
            var target = new TestObject();
            var action = new PropertyChangeAction(target, nameof(TestObject.Name), "Old", "New");

            Assert.Contains("Name", action.Description);
            Assert.Contains("Change", action.Description);
        }

        #endregion

        #region Execute Tests

        [Fact]
        public void Execute_SetsNewValue()
        {
            var target = new TestObject { Name = "Old" };
            var action = new PropertyChangeAction(target, nameof(TestObject.Name), "Old", "New");

            action.Execute();

            Assert.Equal("New", target.Name);
        }

        [Fact]
        public void Execute_WithIntProperty_SetsNewValue()
        {
            var target = new TestObject { Value = 10 };
            var action = new PropertyChangeAction(target, nameof(TestObject.Value), 10, 20);

            action.Execute();

            Assert.Equal(20, target.Value);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_RestoresOldValue()
        {
            var target = new TestObject { Name = "New" };
            var action = new PropertyChangeAction(target, nameof(TestObject.Name), "Old", "New");

            action.Undo();

            Assert.Equal("Old", target.Name);
        }

        [Fact]
        public void Undo_WithIntProperty_RestoresOldValue()
        {
            var target = new TestObject { Value = 20 };
            var action = new PropertyChangeAction(target, nameof(TestObject.Value), 10, 20);

            action.Undo();

            Assert.Equal(10, target.Value);
        }

        [Fact]
        public void Execute_ThenUndo_RestoresOriginalState()
        {
            var target = new TestObject { Name = "Original" };
            var action = new PropertyChangeAction(target, nameof(TestObject.Name), "Original", "Changed");

            action.Execute();
            Assert.Equal("Changed", target.Name);

            action.Undo();
            Assert.Equal("Original", target.Name);
        }

        #endregion

        #region Merge Tests

        [Fact]
        public void CanMergeWith_SameTargetAndProperty_WithinWindow_ReturnsTrue()
        {
            var target = new TestObject();
            var action1 = new PropertyChangeAction(target, nameof(TestObject.Name), "A", "B");
            var action2 = new PropertyChangeAction(target, nameof(TestObject.Name), "B", "C");

            Assert.True(action1.CanMergeWith(action2));
        }

        [Fact]
        public void CanMergeWith_DifferentTarget_ReturnsFalse()
        {
            var target1 = new TestObject();
            var target2 = new TestObject();
            var action1 = new PropertyChangeAction(target1, nameof(TestObject.Name), "A", "B");
            var action2 = new PropertyChangeAction(target2, nameof(TestObject.Name), "B", "C");

            Assert.False(action1.CanMergeWith(action2));
        }

        [Fact]
        public void CanMergeWith_DifferentProperty_ReturnsFalse()
        {
            var target = new TestObject();
            var action1 = new PropertyChangeAction(target, nameof(TestObject.Name), "A", "B");
            var action2 = new PropertyChangeAction(target, nameof(TestObject.Value), 1, 2);

            Assert.False(action1.CanMergeWith(action2));
        }

        [Fact]
        public void CanMergeWith_NonPropertyChangeAction_ReturnsFalse()
        {
            var target = new TestObject();
            var action1 = new PropertyChangeAction(target, nameof(TestObject.Name), "A", "B");
            var action2 = new DelegateAction("Test", () => { }, () => { });

            Assert.False(action1.CanMergeWith(action2));
        }

        [Fact]
        public void MergeWith_ReturnsActionWithOldValueFromFirstAndNewValueFromSecond()
        {
            var target = new TestObject();
            var action1 = new PropertyChangeAction(target, nameof(TestObject.Name), "A", "B");
            var action2 = new PropertyChangeAction(target, nameof(TestObject.Name), "B", "C");

            var merged = action1.MergeWith(action2) as PropertyChangeAction;

            Assert.NotNull(merged);
            Assert.Equal("A", merged.OldValue);
            Assert.Equal("C", merged.NewValue);
        }

        [Fact]
        public void MergeWith_NonPropertyChangeAction_ReturnsOriginal()
        {
            var target = new TestObject();
            var action1 = new PropertyChangeAction(target, nameof(TestObject.Name), "A", "B");
            var action2 = new DelegateAction("Test", () => { }, () => { });

            var result = action1.MergeWith(action2);

            Assert.Same(action1, result);
        }

        [Fact]
        public void MergeWith_UsesLaterTimestamp()
        {
            var target = new TestObject();
            var action1 = new PropertyChangeAction(target, nameof(TestObject.Name), "A", "B");
            System.Threading.Thread.Sleep(10);
            var action2 = new PropertyChangeAction(target, nameof(TestObject.Name), "B", "C");

            var merged = action1.MergeWith(action2) as PropertyChangeAction;

            Assert.NotNull(merged);
            Assert.Equal(action2.Timestamp, merged.Timestamp);
        }

        #endregion

        #region Static Configuration Tests

        [Fact]
        public void MergeWindowMilliseconds_DefaultValue()
        {
            Assert.Equal(500, PropertyChangeAction.MergeWindowMilliseconds);
        }

        [Fact]
        public void MergeWindowMilliseconds_CanBeChanged()
        {
            var original = PropertyChangeAction.MergeWindowMilliseconds;
            try
            {
                PropertyChangeAction.MergeWindowMilliseconds = 1000;
                Assert.Equal(1000, PropertyChangeAction.MergeWindowMilliseconds);
            }
            finally
            {
                PropertyChangeAction.MergeWindowMilliseconds = original;
            }
        }

        #endregion
    }
}
