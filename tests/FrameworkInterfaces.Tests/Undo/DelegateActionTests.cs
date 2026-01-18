using Xunit;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces.Tests.Undo
{
    public class DelegateActionTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsDescription()
        {
            var action = new DelegateAction("Test Description", () => { }, () => { });

            Assert.Equal("Test Description", action.Description);
        }

        [Fact]
        public void Constructor_SetsTarget()
        {
            var target = new object();
            var action = new DelegateAction("Test", () => { }, () => { }, target);

            Assert.Same(target, action.Target);
        }

        [Fact]
        public void Constructor_WithNullTarget_TargetIsNull()
        {
            var action = new DelegateAction("Test", () => { }, () => { });

            Assert.Null(action.Target);
        }

        [Fact]
        public void Constructor_SetsTimestamp()
        {
            var before = DateTime.Now;
            var action = new DelegateAction("Test", () => { }, () => { });
            var after = DateTime.Now;

            Assert.True(action.Timestamp >= before);
            Assert.True(action.Timestamp <= after);
        }

        #endregion

        #region Execute Tests

        [Fact]
        public void Execute_CallsExecuteDelegate()
        {
            var executed = false;
            var action = new DelegateAction("Test", () => executed = true, () => { });

            action.Execute();

            Assert.True(executed);
        }

        [Fact]
        public void Execute_WithParameters_PassesParameters()
        {
            var capturedValue = 0;
            var action = new DelegateAction("Test",
                () => capturedValue = 42,
                () => capturedValue = 0);

            action.Execute();

            Assert.Equal(42, capturedValue);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_CallsUndoDelegate()
        {
            var undone = false;
            var action = new DelegateAction("Test", () => { }, () => undone = true);

            action.Undo();

            Assert.True(undone);
        }

        [Fact]
        public void Execute_ThenUndo_WorksCorrectly()
        {
            var value = "initial";
            var action = new DelegateAction("Test",
                () => value = "executed",
                () => value = "undone");

            action.Execute();
            Assert.Equal("executed", value);

            action.Undo();
            Assert.Equal("undone", value);
        }

        #endregion

        #region CanMergeWith Tests

        [Fact]
        public void CanMergeWith_ByDefault_ReturnsFalse()
        {
            var action1 = new DelegateAction("Test1", () => { }, () => { });
            var action2 = new DelegateAction("Test2", () => { }, () => { });

            Assert.False(action1.CanMergeWith(action2));
        }

        [Fact]
        public void CanMergeWith_Null_ReturnsFalse()
        {
            var action = new DelegateAction("Test", () => { }, () => { });

            Assert.False(action.CanMergeWith(null!));
        }

        #endregion

        #region MergeWith Tests

        [Fact]
        public void MergeWith_ByDefault_ReturnsSelf()
        {
            var action1 = new DelegateAction("Test1", () => { }, () => { });
            var action2 = new DelegateAction("Test2", () => { }, () => { });

            var result = action1.MergeWith(action2);

            Assert.Same(action1, result);
        }

        #endregion
    }
}
