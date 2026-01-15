/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using FrameworkInterfaces;
using FrameworkInterfaces.Undo;
using System;
using System.Drawing;

namespace FrameworkUI.Demo
{
    /// <summary>
    /// A demo element that showcases the undo/redo functionality. This element demonstrates how to use RecordPropertyChange for undo support.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UndoDemoElement : ElementBase, IUndoableElement
    {
        private string _customValue = "Initial Value";
        private int _numericValue = 0;
        private bool _booleanValue = false;
        private DateTime _dateValue = DateTime.Today;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoDemoElement"/> class.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="parentCollection">The parent collection that contains this element.</param>
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

        /// <summary>
        /// Gets the creation date of the element.
        /// </summary>
        public override DateTime CreationDate => _creationDate;

        /// <summary>
        /// Gets the last modified date of the element.
        /// </summary>
        public override DateTime LastModified => _lastModified;

        /// <summary>
        /// Gets the name used when saving the element to disk.
        /// </summary>
        public override string NameOnDisk => Name;

        /// <summary>
        /// Gets the image icon representing the element.
        /// </summary>
        public override Bitmap ElementImage => Properties.Resources.Hazard_Icon;

        /// <summary>
        /// Gets a value indicating whether this element can be copied from an external project.
        /// </summary>
        public override bool CanCopyFromExternal => false;

        /// <summary>
        /// Gets a value indicating whether the element is valid.
        /// </summary>
        public override bool IsValid => true;

        #endregion

        #region Required Abstract Methods

        /// <summary>
        /// Creates a copy of the element.
        /// </summary>
        /// <param name="newName">The name for the copied element. If null, a default name will be generated.</param>
        /// <returns>A copy of the element.</returns>
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

        /// <summary>
        /// Copies an element from an external project file.
        /// </summary>
        /// <param name="itemName">The name of the item to copy.</param>
        /// <param name="fullFileName">The full path to the external file.</param>
        /// <returns>Null, as external copying is not supported for this element type.</returns>
        public override IElement CopyFromExternal(string itemName, string fullFileName)
        {
            return null;
        }

        /// <summary>
        /// Deletes the element and raises the appropriate events.
        /// </summary>
        public override void Delete()
        {
            bool cancel = false;
            RaisePreviewDeleted(this, ref cancel);
            if (!cancel)
            {
                RaiseDeleted(this);
            }
        }

        /// <summary>
        /// Opens the element and loads its data from disk. Clears the undo history after loading.
        /// </summary>
        public override void Open()
        {
            // Simulate loading from disk
            IsUndoEnabled = false;
            // ... load data ...
            IsUndoEnabled = true;
            ClearUndoHistory();
            SetIsDirty(false);
        }

        /// <summary>
        /// Saves the element to disk and marks the current state as the undo save point.
        /// </summary>
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
        /// Performs a batch update using a transaction. All changes within the transaction can be undone with a single Undo.
        /// </summary>
        /// <param name="newCustomValue">The new custom string value.</param>
        /// <param name="newNumericValue">The new numeric value.</param>
        /// <param name="newBooleanValue">The new boolean value.</param>
        public void PerformBatchUpdate(string newCustomValue, int newNumericValue, bool newBooleanValue)
        {
            // Using statement ensures transaction is committed when disposed
            using (UndoManager.BeginTransaction("Batch Update"))
            {
                CustomValue = newCustomValue;
                NumericValue = newNumericValue;
                BooleanValue = newBooleanValue;
            }
        }
    }
}
