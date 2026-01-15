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

using DatabaseManager;
using DocumentFormat.OpenXml.Vml.Office;
using FrameworkInterfaces;
using FrameworkInterfaces.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Media;


namespace FrameworkUI.Demo
{
    /// <summary>
    /// Represents a demo project that showcases the FrameworkUI features.
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
    [Category("Project")]
    [DisplayName("Demo Project")]
    [Description("The Demo project includes hazard, response, and consequence input functions.")]
    [Browsable(true)]
    public class DemoProject : ProjectBase
    {
        #region Construction

        /// <summary>
        /// Lazy initialization for thread-safe singleton pattern.
        /// </summary>
        private static readonly Lazy<DemoProject> _lazyInstance = new Lazy<DemoProject>(() => new DemoProject());

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        private DemoProject()
        {
            InitializeMessages();

            _hazardFunctions.ObjectSaved += ElementCollection_Saved;
            _responseFunctions.ObjectSaved += ElementCollection_Saved;
            _consequenceFunctions.ObjectSaved += ElementCollection_Saved;

            _readOnlyElementCollections = new ReadOnlyCollection<IElementCollection>(new IElementCollection[] 
            { _hazardFunctions, _responseFunctions, _consequenceFunctions});
        }

        /// <summary>
        /// Returns the singleton instance of the Project.
        /// </summary>
        /// <returns>The singleton Project instance.</returns>
        public static DemoProject GetInstance()
        {
            return _lazyInstance.Value;
        }

        /// <summary>
        /// Initializes the message items. Called after construction since they reference instance properties.
        /// </summary>
        private void InitializeMessages()
        {
            _descriptionMsg = new BasicMessageItem(MessageType.Message, "The project does not have a description.",
                this, "Project", Name, nameof(Description), "P-MSG-001");

            _noNameMsg = new BasicMessageItem(MessageType.Error, "The name of the project cannot be blank.",
                this, "Project", Name, nameof(Name), "P-ERR-001");

            _longNameMsg = new BasicMessageItem(MessageType.Error, "The name of the project cannot exceed 50 characters.",
                this, "Project", Name, nameof(Name), "P-ERR-002");

            _dupNameMsg = new BasicMessageItem(MessageType.Error, $"A project with the name '{Name}' already exists in this directory and must be unique.",
                this, "Project", Name, nameof(Name), "P-ERR-003");

            _badCharMsg = new BasicMessageItem(MessageType.Error, "Invalid character in project name.",
                this, "Project", Name, nameof(Name), "P-ERR-004");

            _messages.Clear();
            _messages.AddRange(new[] { _descriptionMsg, _noNameMsg, _longNameMsg, _dupNameMsg, _badCharMsg });
        }

        #endregion

        #region Members

        private static readonly char[] _InvalidNameCharacters = new List<char>(Path.GetInvalidFileNameChars()) { '\'', '[', ']' }.ToArray();
        private HazardElementCollection _hazardFunctions;
        private ResponseElementCollection _responseFunctions;
        private ConsequenceElementCollection _consequenceFunctions;
        private bool _nameValid = false;
        private List<BasicMessageItem> _messages = new List<BasicMessageItem>();
        private Messenger _messenger = Messenger.GetInstance();
        private BasicMessageItem _descriptionMsg;
        private BasicMessageItem _noNameMsg;
        private BasicMessageItem _longNameMsg;
        private BasicMessageItem _dupNameMsg;
        private BasicMessageItem _badCharMsg;


        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Name"), Description("The name of the project."), Browsable(true)]
        public override string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    var oldValue = _name;
                    _name = value;

                    // Reset messages with new name
                    foreach (var item in _messages)
                    {
                        item.SourceName = value;
                    }

                    // Check if name is nothing.
                    if (string.IsNullOrEmpty(Name))
                    {
                        _nameValid = false;
                        _messenger.Add(_noNameMsg);
                    }
                    else
                    {
                        _messenger.Remove(_noNameMsg);
                    }

                    // Check the length of the name.
                    if (Name.Length > 50)
                    {
                        _nameValid = false;
                        _messenger.Add(_longNameMsg);
                    }
                    else
                    {
                        _messenger.Remove(_longNameMsg);
                    }

                    // Check if there are bad characters.
                    _messenger.Remove(_badCharMsg);
                    foreach (char badChar in DemoProject.InvalidNameCharacters)
                    {
                        if (Name.Contains(badChar))
                        {
                            _nameValid = false;
                            string badCharacters = "<>:" + (char)34 + "/\\|?*";
                            _badCharMsg.Description = $"Invalid character in project name: '{badChar}'. Invalid characters are: {badCharacters}";
                            _messenger.Add(_badCharMsg);
                            break;
                        }
                    }

                    RecordPropertyChange(nameof(Name), oldValue, value);
                }
            }
        }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Description"), Description("The description for the project."), Browsable(true)]
        public override string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    var oldValue = _description;
                    _description = value;

                    // Validate description message
                    if (string.IsNullOrEmpty(_description))
                    {
                        _messenger.Add(_descriptionMsg);
                    }
                    else
                    {
                        _messenger.Remove(_descriptionMsg);
                    }
                    RecordPropertyChange(nameof(Description), oldValue, value);
                }
            }
        }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Software Version"), Description("The version of RMC-TotalRisk used to last modify the project file."), Browsable(true)]
        public override string SoftwareVersion => "1.0";

        /// <inheritdoc/>
        public override Bitmap ProjectImage => Properties.Resources.Hazard_Icon;

        /// <summary>
        /// Gets the array of invalid characters for project names.
        /// </summary>
        /// <value>
        /// An array of characters that are not allowed in project names,
        /// including invalid file name characters, apostrophe, and brackets.
        /// </value>
        public static char[] InvalidNameCharacters => _InvalidNameCharacters;


        #endregion

        #region Methods

        /// <inheritdoc/>
        public override void CreateNew(string fullFileName)
        {
            // Set project meta data & properties
            FullFileName = fullFileName;
            Name = Path.GetFileNameWithoutExtension(fullFileName);
            Description = "";
            CreationDate = DateTime.Now;
            LastModified = DateTime.Now;

            // Load element collections.
            _readOnlyElementCollections = new ReadOnlyCollection<IElementCollection>(new IElementCollection[]
            { _hazardFunctions, _responseFunctions, _consequenceFunctions});

            Save();
        }

        /// <summary>
        /// Creates a new dummy project.
        /// </summary>
        public void CreateNewDummyProject()
        {
            // Set project meta data and properties
            FullFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".demo";
            Name = "Blank Project";
            Description = "This is a blank project file.";
            CreationDate = DateTime.Now;
            LastModified = DateTime.Now;

            // Load element collections.
            _readOnlyElementCollections = new ReadOnlyCollection<IElementCollection>(new IElementCollection[]
            { _hazardFunctions, _responseFunctions, _consequenceFunctions});

            Save();
        }

        /// <inheritdoc/>
        public override void Open()
        {
            _openingProject = true;
            SetIsDirty(false);

            //
            // Load from disk or database
            //

            // Open element collections.
            _hazardFunctions.Open();
            _responseFunctions.Open();
            _consequenceFunctions.Open();

            // Load element collections.
            _readOnlyElementCollections = new ReadOnlyCollection<IElementCollection>(new IElementCollection[]
            { _hazardFunctions, _responseFunctions, _consequenceFunctions});

            SetIsDirty(false);
            NameOnDisk = Name;
            _openingProject = false;
        }

        /// <inheritdoc/>
        public override void Close()
        {
            // Close all project element collections.. 
            _hazardFunctions.Clear();
            _responseFunctions.Clear();
            _consequenceFunctions.Clear();
        }

        /// <inheritdoc/>
        public override void Save()
        {
            bool cancel = false;
            RaisePreviewObjectSaved(this, ref cancel);
            if (cancel) return;

            // Must have a file name to save.
            if (FullFileName == null) return;

            // Check if the file name needs to be renamed
            string winExpName = Path.GetFileNameWithoutExtension(FullFileName);
            string winExpFullFileName = FullFileName;

            if (winExpName != Name && Name != "Blank Project")
            {
                FullFileName = Path.Combine(FileDirectory, Name + ".tra");
                File.Move(winExpFullFileName, FullFileName);
            }

            // Update last edited
            _lastModified = DateTime.Now;

            // Save project meta data

            // Next, save element collections
            for (int i = 0; i < ElementCollections.Count; i++)
            {
                ElementCollections[i].Save();
            }

            SetIsDirty(false);
            RaiseObjectSaved(this);
        }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            if (!_nameValid) return false;
            return true;
        }

        #region Compact and Optimize Project File

        /// <inheritdoc/>
        public override void Compact()
        {
            // Compact or vacuum SQLite database to reduce file size.
        }

        /// <inheritdoc/>
        public override void Optimize()
        {
            // Optimize the SQLite database for better performance.
        }

        #endregion

        #endregion
    }
}
