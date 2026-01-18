using Xunit;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces.Tests.Undo
{
    public class CompositeActionTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsDescription()
        {
            var action = new CompositeAction("Composite Test");

            Assert.Equal("Composite Test", action.Description);
        }

        [Fact]
        public void Constructor_InitializesEmptyActionsList()
        {
            var action = new CompositeAction("Test");

            Assert.NotNull(action.Actions);
            Assert.Empty(action.Actions);
        }

        [Fact]
        public void Constructor_SetsTimestamp()
        {
            var before = DateTime.Now;
            var action = new CompositeAction("Test");
            var after = DateTime.Now;

            Assert.True(action.Timestamp >= before);
            Assert.True(action.Timestamp <= after);
        }

        #endregion

        #region AddAction Tests

        [Fact]
        public void AddAction_AddsToActionsList()
        {
            var composite = new CompositeAction("Test");
            var childAction = new DelegateAction("Child", () => { }, () => { });

            composite.AddAction(childAction);

            Assert.Single(composite.Actions);
            Assert.Same(childAction, composite.Actions[0]);
        }

        [Fact]
        public void AddAction_MultipleActions_AddsInOrder()
        {
            var composite = new CompositeAction("Test");
            var action1 = new DelegateAction("Action1", () => { }, () => { });
            var action2 = new DelegateAction("Action2", () => { }, () => { });
            var action3 = new DelegateAction("Action3", () => { }, () => { });

            composite.AddAction(action1);
            composite.AddAction(action2);
            composite.AddAction(action3);

            Assert.Equal(3, composite.Actions.Count);
            Assert.Same(action1, composite.Actions[0]);
            Assert.Same(action2, composite.Actions[1]);
            Assert.Same(action3, composite.Actions[2]);
        }

        #endregion

        #region Execute Tests

        [Fact]
        public void Execute_ExecutesAllActionsInOrder()
        {
            var composite = new CompositeAction("Test");
            var executionOrder = new List<int>();

            composite.AddAction(new DelegateAction("A1", () => executionOrder.Add(1), () => { }));
            composite.AddAction(new DelegateAction("A2", () => executionOrder.Add(2), () => { }));
            composite.AddAction(new DelegateAction("A3", () => executionOrder.Add(3), () => { }));

            composite.Execute();

            Assert.Equal(new[] { 1, 2, 3 }, executionOrder);
        }

        [Fact]
        public void Execute_WithNoActions_DoesNotThrow()
        {
            var composite = new CompositeAction("Empty");

            var exception = Record.Exception(() => composite.Execute());

            Assert.Null(exception);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_UndoesAllActionsInReverseOrder()
        {
            var composite = new CompositeAction("Test");
            var undoOrder = new List<int>();

            composite.AddAction(new DelegateAction("A1", () => { }, () => undoOrder.Add(1)));
            composite.AddAction(new DelegateAction("A2", () => { }, () => undoOrder.Add(2)));
            composite.AddAction(new DelegateAction("A3", () => { }, () => undoOrder.Add(3)));

            composite.Undo();

            Assert.Equal(new[] { 3, 2, 1 }, undoOrder);
        }

        [Fact]
        public void Undo_WithNoActions_DoesNotThrow()
        {
            var composite = new CompositeAction("Empty");

            var exception = Record.Exception(() => composite.Undo());

            Assert.Null(exception);
        }

        #endregion

        #region Execute and Undo Integration Tests

        [Fact]
        public void ExecuteThenUndo_RestoresState()
        {
            var composite = new CompositeAction("Test");
            var value1 = 0;
            var value2 = 0;

            composite.AddAction(new DelegateAction("A1", () => value1 = 10, () => value1 = 0));
            composite.AddAction(new DelegateAction("A2", () => value2 = 20, () => value2 = 0));

            composite.Execute();
            Assert.Equal(10, value1);
            Assert.Equal(20, value2);

            composite.Undo();
            Assert.Equal(0, value1);
            Assert.Equal(0, value2);
        }

        #endregion

        #region CanMergeWith Tests

        [Fact]
        public void CanMergeWith_ReturnsFalse()
        {
            var composite1 = new CompositeAction("Test1");
            var composite2 = new CompositeAction("Test2");

            Assert.False(composite1.CanMergeWith(composite2));
        }

        [Fact]
        public void CanMergeWith_NonCompositeAction_ReturnsFalse()
        {
            var composite = new CompositeAction("Test");
            var delegateAction = new DelegateAction("Other", () => { }, () => { });

            Assert.False(composite.CanMergeWith(delegateAction));
        }

        #endregion

        #region MergeWith Tests

        [Fact]
        public void MergeWith_ReturnsSelf()
        {
            var composite1 = new CompositeAction("Test1");
            var composite2 = new CompositeAction("Test2");

            var result = composite1.MergeWith(composite2);

            Assert.Same(composite1, result);
        }

        #endregion

        #region Target Property Tests

        [Fact]
        public void Target_DefaultIsNull()
        {
            var composite = new CompositeAction("Test");

            Assert.Null(composite.Target);
        }

        #endregion
    }
}
