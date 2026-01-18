using FrameworkInterfaces;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace FrameworkInterfaces.Tests.Undo.Actions
{
    /// <summary>
    /// Mock implementation of IElement for testing.
    /// </summary>
    internal class MockElement : IElement
    {
        public string Name { get; set; } = "TestElement";
        public string DisplayName { get; set; } = "Test Element";
        public IElementCollection ParentCollection { get; set; } = null!;
        public Bitmap ElementImage => null!;
        public bool CanCopyFromExternal => false;
        public bool IsValid => true;
        public string? Description { get; set; }
        public string FileName { get; set; } = "test.xml";
        public bool IsDirty { get; set; }

        public string NameOnDisk => throw new NotImplementedException();

        public DateTime CreationDate => throw new NotImplementedException();

        public DateTime LastModified => throw new NotImplementedException();

        public event PreviewDeletedEventHandler? PreviewDeleted;
        public event DeletedEventHandler? Deleted;
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;
        public event ObjectSavedEventHandler ObjectSaved;
        public event PropertyChangedEventHandler? PropertyChanged;

        public IElement Copy(string? newName = null)
        {
            return new MockElement { Name = newName ?? Name, DisplayName = DisplayName };
        }

        public IElement CopyFromExternal(string itemName, string fullFileName)
        {
            return new MockElement { Name = itemName };
        }

        public void Delete()
        {
            bool cancel = false;
            PreviewDeleted?.Invoke(this, ref cancel);
            if (!cancel)
            {
                Deleted?.Invoke(this);
            }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Save() { }
    }

    /// <summary>
    /// Mock implementation of IElementCollection for testing.
    /// </summary>
    internal class MockElementCollection : IElementCollection
    {
        private readonly List<IElement> _elements = new();

        public string Name { get; set; } = "TestCollection";
        public IProject ParentProject { get; set; } = null!;
        public ReadOnlyCollection<IElementCollection> ElementCollections { get; set; } = new ReadOnlyCollection<IElementCollection>(Array.Empty<IElementCollection>());
        public string? Description { get; set; }
        public string FileName { get; set; } = "collection.xml";
        public bool IsDirty { get; set; }

        public event ElementAddedEventHandler? ElementAdded;
        public event ElementRemovedEventHandler? ElementRemoved;
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;
        public event ObjectSavedEventHandler ObjectSaved;
        public event PropertyChangedEventHandler? PropertyChanged;

        public int Count => _elements.Count;
        public bool IsReadOnly => false;

        public string NameOnDisk => throw new NotImplementedException();

        public IElement this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        public void Add(IElement item)
        {
            _elements.Add(item);
            ElementAdded?.Invoke(item);
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(IElement item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(IElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(IElement item)
        {
            return _elements.IndexOf(item);
        }

        public void Insert(int index, IElement item)
        {
            _elements.Insert(index, item);
            ElementAdded?.Invoke(item);
        }

        public bool Remove(IElement item)
        {
            var result = _elements.Remove(item);
            if (result)
            {
                ElementRemoved?.Invoke(item);
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            var item = _elements[index];
            _elements.RemoveAt(index);
            ElementRemoved?.Invoke(item);
        }

        public void Sort(IComparer<IElement> sortAction)
        {
            _elements.Sort(sortAction);
        }

        public void MoveElement(IElement element, int startIndex, int endIndex)
        {
            if (startIndex < 0 || startIndex >= _elements.Count)
                return;

            _elements.RemoveAt(startIndex);
            _elements.Insert(endIndex, element);
        }

        public void Delete() { }

        public void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName)
        {
            var element = new MockElement { Name = elementName };
            Insert(index, element);
        }

        public void Save() { }

        public void Open()
        {
            throw new NotImplementedException();
        }
    }
}
