using Xunit;
using FrameworkInterfaces;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces.Tests.Undo.Actions
{
    public class RemoveElementActionTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_NullCollection_ThrowsArgumentNullException()
        {
            var element = new MockElement();

            Assert.Throws<ArgumentNullException>(() =>
                new RemoveElementAction(null!, element));
        }

        [Fact]
        public void Constructor_NullElement_ThrowsArgumentNullException()
        {
            var collection = new MockElementCollection();

            Assert.Throws<ArgumentNullException>(() =>
                new RemoveElementAction(collection, null!));
        }

        [Fact]
        public void Constructor_ValidArgs_SetsProperties()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();
            collection.Add(element);

            var action = new RemoveElementAction(collection, element);

            Assert.Same(collection, action.Collection);
            Assert.Same(element, action.Element);
            Assert.Same(collection, action.Target);
        }

        [Fact]
        public void Constructor_SetsTimestamp()
        {
            var before = DateTime.Now;
            var collection = new MockElementCollection();
            var element = new MockElement();

            var action = new RemoveElementAction(collection, element);
            var after = DateTime.Now;

            Assert.True(action.Timestamp >= before);
            Assert.True(action.Timestamp <= after);
        }

        [Fact]
        public void Constructor_CapturesElementIndex()
        {
            var collection = new MockElementCollection();
            var element1 = new MockElement { Name = "First" };
            var element2 = new MockElement { Name = "Second" };
            var element3 = new MockElement { Name = "Third" };
            collection.Add(element1);
            collection.Add(element2);
            collection.Add(element3);

            var action = new RemoveElementAction(collection, element2);

            Assert.Equal(1, action.Index);
        }

        [Fact]
        public void Constructor_WithExplicitIndex_UsesProvidedIndex()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();

            var action = new RemoveElementAction(collection, element, 5);

            Assert.Equal(5, action.Index);
        }

        #endregion

        #region Description Tests

        [Fact]
        public void Description_UsesDisplayName()
        {
            var collection = new MockElementCollection();
            var element = new MockElement { DisplayName = "My Element" };

            var action = new RemoveElementAction(collection, element);

            Assert.Equal("Delete My Element", action.Description);
        }

        [Fact]
        public void Description_FallsBackToName()
        {
            var collection = new MockElementCollection();
            var element = new MockElement { DisplayName = null!, Name = "ElementName" };

            var action = new RemoveElementAction(collection, element);

            Assert.Equal("Delete ElementName", action.Description);
        }

        [Fact]
        public void Description_FallsBackToGeneric()
        {
            var collection = new MockElementCollection();
            var element = new MockElement { DisplayName = null!, Name = null! };

            var action = new RemoveElementAction(collection, element);

            Assert.Equal("Delete element", action.Description);
        }

        #endregion

        #region Execute Tests

        [Fact]
        public void Execute_RemovesElementFromCollection()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();
            collection.Add(element);
            var action = new RemoveElementAction(collection, element);

            action.Execute();

            Assert.DoesNotContain(element, collection);
        }

        [Fact]
        public void Execute_CollectionSizeDecreases()
        {
            var collection = new MockElementCollection();
            var element1 = new MockElement { Name = "First" };
            var element2 = new MockElement { Name = "Second" };
            collection.Add(element1);
            collection.Add(element2);
            var action = new RemoveElementAction(collection, element1);

            action.Execute();

            Assert.Single(collection);
            Assert.Contains(element2, collection);
        }

        #endregion

        #region Undo Tests

        [Fact]
        public void Undo_ReAddsElementToCollection()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();
            collection.Add(element);
            var action = new RemoveElementAction(collection, element);
            collection.Remove(element);

            action.Undo();

            Assert.Contains(element, collection);
        }

        [Fact]
        public void Undo_ReAddsElementAtOriginalIndex()
        {
            var collection = new MockElementCollection();
            var element1 = new MockElement { Name = "First" };
            var element2 = new MockElement { Name = "Second" };
            var element3 = new MockElement { Name = "Third" };
            collection.Add(element1);
            collection.Add(element2);
            collection.Add(element3);

            var action = new RemoveElementAction(collection, element2);
            collection.Remove(element2);

            action.Undo();

            Assert.Equal(1, collection.IndexOf(element2));
            Assert.Equal(3, collection.Count);
        }

        [Fact]
        public void Undo_NegativeIndex_AddsToEnd()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();

            var action = new RemoveElementAction(collection, element, -1);

            action.Undo();

            Assert.Contains(element, collection);
            Assert.Equal(0, collection.IndexOf(element));
        }

        [Fact]
        public void Undo_IndexOutOfRange_AddsToEnd()
        {
            var collection = new MockElementCollection();
            var existing = new MockElement { Name = "Existing" };
            collection.Add(existing);

            var element = new MockElement { Name = "Removed" };
            var action = new RemoveElementAction(collection, element, 100);

            action.Undo();

            Assert.Equal(1, collection.IndexOf(element));
        }

        [Fact]
        public void Undo_ExecuteRoundTrip_RestoresState()
        {
            var collection = new MockElementCollection();
            var element1 = new MockElement { Name = "First" };
            var element2 = new MockElement { Name = "Second" };
            collection.Add(element1);
            collection.Add(element2);

            var action = new RemoveElementAction(collection, element2);

            // Execute removes element
            action.Execute();
            Assert.Single(collection);
            Assert.DoesNotContain(element2, collection);

            // Undo re-adds element
            action.Undo();
            Assert.Equal(2, collection.Count);
            Assert.Equal(1, collection.IndexOf(element2));

            // Execute removes again
            action.Execute();
            Assert.Single(collection);
            Assert.DoesNotContain(element2, collection);
        }

        #endregion

        #region CanMergeWith Tests

        [Fact]
        public void CanMergeWith_ReturnsFalse()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();
            var action = new RemoveElementAction(collection, element);

            var other = new RemoveElementAction(collection, new MockElement());

            Assert.False(action.CanMergeWith(other));
        }

        #endregion

        #region MergeWith Tests

        [Fact]
        public void MergeWith_ReturnsSelf()
        {
            var collection = new MockElementCollection();
            var element = new MockElement();
            var action = new RemoveElementAction(collection, element);

            var other = new RemoveElementAction(collection, new MockElement());

            var result = action.MergeWith(other);

            Assert.Same(action, result);
        }

        #endregion
    }
}
