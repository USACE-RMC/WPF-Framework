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

using FrameworkInterfaces.Undo;
using FrameworkInterfaces.Undo.Actions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace FrameworkInterfaces
{
    /// <summary>
    /// Provides a base class for projects that can be stored, modified, and validated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class provides common functionality for all projects including:
    /// <list type="bullet">
    /// <item><description>Name and description management</description></item>
    /// <item><description>Creation and modification date tracking</description></item>
    /// <item><description>Dirty state tracking for unsaved changes</description></item>
    /// <item><description>Validation support with error messaging</description></item>
    /// <item><description>Copy, save, open, and clear operations</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class ProjectBase : IProject
    {
        #region Fields

        /// <summary>
        /// The name of the project.
        /// </summary>
        protected string _name;

        /// <summary>
        /// The description of the project.
        /// </summary>
        protected string _description;

        /// <summary>
        /// The date when the project was created.
        /// </summary>
        protected DateTime _creationDate;

        /// <summary>
        /// The date when the project was last modified.
        /// </summary>
        protected DateTime _lastModified;

        /// <summary>
        /// Indicates whether the project is in a valid state.
        /// </summary>
        protected bool _isValid;

        /// <summary>
        /// The full project file name, including the directory path.
        /// </summary>
        protected string _fullFileName;

        /// <summary>
        /// The AvalonDock layout string.
        /// </summary>
        protected string _avalonDockLayout;

        /// <summary>
        /// The Project Explorer layout string.
        /// </summary>
        protected string _projectExplorerLayout;

        /// <summary>
        /// The undo manager for this project. Lazily initialized when first accessed.
        /// </summary>
        protected IUndoManager _undoManager;

        /// <summary>
        /// Indicates whether undo recording is enabled for this project.
        /// </summary>
        protected bool _isUndoEnabled = true;

        /// <summary>
        /// Indicates whether a project is currently being opened.
        /// </summary>
        protected bool _openingProject = false;

        /// <summary>
        /// The read-only collection of element collections.
        /// </summary>
        protected ReadOnlyCollection<IElementCollection> _readOnlyElementCollections;

        #endregion

        #region Abstract Properties

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The project name.</value>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        /// <value>The project description.</value>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets the version of the software the project was last edited with.
        /// </summary>
        /// <value>
        /// The version number of software.
        /// </value>
        public abstract string SoftwareVersion { get; }

        /// <summary>
        /// Gets the image representation of the project as a Bitmap.
        /// </summary>
        /// <value>The project's icon or image.</value>
        public abstract Bitmap ProjectImage { get; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the project as stored on disk.
        /// </summary>
        /// <value>The on-disk name of the project.</value>
        public string NameOnDisk { get; protected set; }

        /// <inheritdoc/>
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// Gets the date and time when the project was created.
        /// </summary>
        /// <value>The creation date and time.</value>
        [Category("Meta Data"), DisplayName("Creation Date"), Description("The date and time when the project was first created."), Browsable(true)]
        public DateTime CreationDate 
        {
            get { return _creationDate; }
            protected set 
            {
                _creationDate = value;
            } 
        }

        /// <summary>
        /// Gets the date and time when the project was last modified.
        /// </summary>
        /// <value>The last modification date and time.</value>
        [Category("Meta Data"), DisplayName("Last Modified"), Description("The date and time when the project was last modified."), Browsable(true)]
        public DateTime LastModified
        {
            get { return _lastModified; }
            protected set
            {
                _lastModified = value;
            }
        }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Full File Name"), Description("The full project file name, including the directory path."), Browsable(true)]
        public string FullFileName
        {
            get => _fullFileName;
            set
            {
                if (_fullFileName != value)
                {
                    _fullFileName = value;
                    RaisePropertyChange(nameof(FullFileName));
                }
            }
        }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("File Directory"), Description("The directory where the project file is located."), Browsable(true)]
        public string FileDirectory => FullFileName != null ? Path.GetDirectoryName(FullFileName) : null;

        /// <inheritdoc/>
        public string AvalonDockLayout
        {
            get => _avalonDockLayout;
            set
            {
                if (_avalonDockLayout != value)
                {
                    _avalonDockLayout = value;
                    RaisePropertyChange(nameof(AvalonDockLayout));
                }
            }
        }

        /// <inheritdoc/>
        public string ProjectExplorerLayout
        {
            get => _projectExplorerLayout;
            set
            {
                if (_projectExplorerLayout != value)
                {
                    _projectExplorerLayout = value;
                    RaisePropertyChange(nameof(ProjectExplorerLayout));
                }
            }
        }


        /// <summary>
        /// Gets or sets the read-only collection of element collections.
        /// </summary>
        /// <value>
        /// A read-only collection containing all element collections in the project.
        /// </value>
        public ReadOnlyCollection<IElementCollection> ElementCollections
        {
            get => _readOnlyElementCollections;
            set => _readOnlyElementCollections = value;
        }

        /// <summary>
        /// Gets the undo manager for this project.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The undo manager is lazily initialized when first accessed. Each project
        /// has its own undo manager, enabling Visual Studio-style per-document undo
        /// where Ctrl+Z operates on the active document.
        /// </para>
        /// </remarks>
        public virtual IUndoManager UndoManager
        {
            get
            {
                if (_undoManager == null)
                {
                    _undoManager = new UndoManager();
                }
                return _undoManager;
            }
        }

        /// <summary>
        /// Gets or sets whether undo recording is enabled for this project.
        /// </summary>
        /// <remarks>
        /// Set to <c>false</c> to temporarily disable undo recording, for example
        /// during bulk operations, loading from disk, or undo/redo operations.
        /// Default is <c>true</c>.
        /// </remarks>
        public bool IsUndoEnabled
        {
            get { return _isUndoEnabled; }
            set { _isUndoEnabled = value; }
        }

        #endregion


        #region Events

        /// <summary>
        /// Occurs before the project is saved, allowing cancellation.
        /// </summary>
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;

        /// <summary>
        /// Occurs after the project has been saved.
        /// </summary>
        public event ObjectSavedEventHandler ObjectSaved;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Abstract Methods

        /// <inheritdoc/>
        public abstract bool IsValid();

        /// <inheritdoc/>
        public abstract void CreateNew(string newFullFileName);

        /// <inheritdoc/>
        public abstract void Open();

        /// <inheritdoc/>
        public abstract void Save();

        /// <inheritdoc/>
        public abstract void Close();

        /// <inheritdoc/>
        public abstract void Compact();

        /// <inheritdoc/>
        public abstract void Optimize();

        #endregion


        #region Protected Methods

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event and optionally sets the dirty state.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="isDirty">
        /// <c>true</c> to mark the element as dirty after the property change;
        /// <c>false</c> to leave the dirty state unchanged. Default is <c>true</c>.
        /// </param>
        /// <remarks>
        /// This method is provided for backwards compatibility. For undo support,
        /// use <see cref="RecordPropertyChange"/> instead.
        /// </remarks>
        protected void RaisePropertyChange(string propertyName, bool isDirty = true)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SetIsDirty(isDirty);
        }

        /// <summary>
        /// Sets the IsDirty property value.
        /// </summary>
        /// <param name="value">The new dirty state.</param>
        protected void SetIsDirty(bool value)
        {
            if (IsDirty != value)
            {
                IsDirty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
            }
        }

        /// <summary>
        /// Records a property change for undo support and raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="isDirty">
        /// <c>true</c> to mark the element as dirty after the property change;
        /// <c>false</c> to leave the dirty state unchanged. Default is <c>true</c>.
        /// </param>
        /// <remarks>
        /// <para>
        /// Call this method from property setters to enable undo/redo support.
        /// If <see cref="IsUndoEnabled"/> is <c>false</c> or the undo manager is currently
        /// executing an action (during undo/redo), the change will not be recorded.
        /// </para>
        /// <para>
        /// Example usage in a derived class:
        /// <code>
        /// public override string Name
        /// {
        ///     get => _name;
        ///     set
        ///     {
        ///         if (_name != value)
        ///         {
        ///             var oldValue = _name;
        ///             _name = value;
        ///             ValidateName();
        ///             RecordPropertyChange(nameof(Name), oldValue, value);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </para>
        /// </remarks>
        protected void RecordPropertyChange(string propertyName, object oldValue, object newValue, bool isDirty = true)
        {
            // Record the action for undo if enabled and not currently executing an undo/redo
            if (IsUndoEnabled && !UndoManager.IsExecutingAction && !Equals(oldValue, newValue))
            {
                var action = new PropertyChangeAction(this, propertyName, oldValue, newValue);
                UndoManager.RecordAction(action);
            }

            // Always raise the property changed event and update dirty state
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SetIsDirty(isDirty);
        }

        /// <summary>
        /// Clears the undo history for this element.
        /// </summary>
        /// <remarks>
        /// Call this after loading data from disk or when you want to establish
        /// a clean state with no undo history.
        /// </remarks>
        protected void ClearUndoHistory()
        {
            _undoManager?.Clear();
        }

        /// <summary>
        /// Marks the current state as the saved state in the undo manager.
        /// </summary>
        /// <remarks>
        /// Call this after successfully saving the element. This updates the
        /// <see cref="IUndoManager.HasChangedSinceSave"/> property.
        /// </remarks>
        protected void MarkUndoSavePoint()
        {
            _undoManager?.MarkSavePoint();
        }

        /// <summary>
        /// Raises the <see cref="PreviewObjectSaved"/> event.
        /// </summary>
        /// <param name="sender">The object to be saved.</param>
        /// <param name="cancel">
        /// When this method returns, contains <c>true</c> if the save operation should be cancelled;
        /// otherwise, <c>false</c>.
        /// </param>
        protected void RaisePreviewObjectSaved(ISave sender, ref bool cancel)
        {
            PreviewObjectSaved?.Invoke(sender, ref cancel);
        }

        /// <summary>
        /// Raises the <see cref="ObjectSaved"/> event.
        /// </summary>
        /// <param name="sender">The object that was saved.</param>
        protected void RaiseObjectSaved(ISave sender)
        {
            ObjectSaved?.Invoke(sender);
        }

        /// <summary>
        /// When the element collections are saved, updates the Last Modified timestamp.
        /// </summary>
        /// <param name="sender">The object that was saved.</param>
        protected void ElementCollection_Saved(ISave sender)
        {
            LastModified = DateTime.Now;
        }

        /// <inheritdoc/>
        public void SaveAs(string newFullFileName)
        {
            // Create a copy of the current project and rename it
            File.Copy(FullFileName, newFullFileName, true);
        }

        /// <inheritdoc/>
        public void ZipProject(string zipFileName)
        {
            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }

            using (var archive = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(FullFileName, Path.GetFileName(FullFileName));
            }
        }

        #endregion

    }
}
