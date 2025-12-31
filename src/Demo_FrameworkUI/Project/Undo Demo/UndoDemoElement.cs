using FrameworkInterfaces;
using FrameworkInterfaces.Undo;
using System;
using System.Drawing;

namespace Demo_ProjectUI.Project.Undo_Demo
{
    /// <summary>
    /// A demo element that showcases the undo/redo functionality.
    /// This element demonstrates how to use RecordPropertyChange for undo support.
    /// </summary>
    public class UndoDemoElement : ElementBase, IUndoableElement
    {
        private string _customValue = "Initial Value";
        private int _numericValue = 0;
        private bool _booleanValue = false;
        private DateTime _dateValue = DateTime.Today;

        public UndoDemoElement(string name, IElementCollection parentCollection) : base(name, parentCollection)
        {
            Name = name;
            SetIsDirty(false);
        }

        #region Properties with Undo Support

        /// <summary>
        /// Gets or sets the name of the element with undo support.
        /// </summary>
        public override string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    var oldValue = _name;
                    _name = value;
                    RecordPropertyChange(nameof(Name), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the description with undo support.
        /// </summary>
        public override string Description
        {
            get => _description ?? "Undo Demo Element";
            set
            {
                if (_description != value)
                {
                    var oldValue = _description;
                    _description = value;
                    RecordPropertyChange(nameof(Description), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a custom string value with undo support.
        /// </summary>
        public string CustomValue
        {
            get => _customValue;
            set
            {
                if (_customValue != value)
                {
                    var oldValue = _customValue;
                    _customValue = value;
                    RecordPropertyChange(nameof(CustomValue), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a numeric value with undo support.
        /// </summary>
        public int NumericValue
        {
            get => _numericValue;
            set
            {
                if (_numericValue != value)
                {
                    var oldValue = _numericValue;
                    _numericValue = value;
                    RecordPropertyChange(nameof(NumericValue), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value with undo support.
        /// </summary>
        public bool BooleanValue
        {
            get => _booleanValue;
            set
            {
                if (_booleanValue != value)
                {
                    var oldValue = _booleanValue;
                    _booleanValue = value;
                    RecordPropertyChange(nameof(BooleanValue), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a date value with undo support.
        /// </summary>
        public DateTime DateValue
        {
            get => _dateValue;
            set
            {
                if (_dateValue != value)
                {
                    var oldValue = _dateValue;
                    _dateValue = value;
                    RecordPropertyChange(nameof(DateValue), oldValue, value);
                }
            }
        }

        #endregion

        #region Required Abstract Properties

        public override DateTime CreationDate => _creationDate;

        public override DateTime LastModified => _lastModified;

        public override string NameOnDisk => Name;

        public override Bitmap ElementImage => Properties.Resources.Hazard_Icon;

        public override bool CanCopyFromExternal => false;

        public override bool IsValid => true;

        #endregion

        #region Required Abstract Methods

        public override IElement Copy(string newName = null)
        {
            var copy = new UndoDemoElement(newName ?? $"{Name}_Copy", ParentCollection);
            copy.CustomValue = this.CustomValue;
            copy.NumericValue = this.NumericValue;
            copy.BooleanValue = this.BooleanValue;
            copy.DateValue = this.DateValue;
            copy.Description = this.Description;
            return copy;
        }

        public override IElement CopyFromExternal(string itemName, string fullFileName)
        {
            return null;
        }

        public override void Delete()
        {
            bool cancel = false;
            RaisePreviewDeleted(this, ref cancel);
            if (!cancel)
            {
                RaiseDeleted(this);
            }
        }

        public override void Open()
        {
            // Simulate loading from disk
            IsUndoEnabled = false;
            // ... load data ...
            IsUndoEnabled = true;
            ClearUndoHistory();
            SetIsDirty(false);
        }

        public override void Save()
        {
            bool cancel = false;
            RaisePreviewObjectSaved(this, ref cancel);
            if (!cancel)
            {
                // Simulate saving to disk
                // ... save data ...
                MarkUndoSavePoint();
                SetIsDirty(false);
                RaiseObjectSaved(this);
            }
        }

        #endregion

        /// <summary>
        /// Performs a batch update using a transaction.
        /// All changes within the transaction can be undone with a single Undo.
        /// </summary>
        public void PerformBatchUpdate(string newCustomValue, int newNumericValue, bool newBooleanValue)
        {
            UndoManager.BeginTransaction("Batch Update");
            try
            {
                CustomValue = newCustomValue;
                NumericValue = newNumericValue;
                BooleanValue = newBooleanValue;
                UndoManager.CommitTransaction();
            }
            catch
            {
                UndoManager.RollbackTransaction();
                throw;
            }
        }
    }
}
