using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace FrameworkUI
{

    /// <summary>
    /// User settings.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class UserSettings
    {

        /// <summary>
        /// Gets and sets whether the user agreed to the terms and conditions for use.
        /// </summary>
        public static bool UserAgreedToTCU { get; set; } = false;

        /// <summary>
        /// Gets and sets the color theme.
        /// </summary>
        public static string ColorTheme { get; set; } = "Light";

        /// <summary>
        /// Gets and sets whether to save window layout.
        /// </summary>
        public static bool SaveWindowLayout { get; set; } = true;

        /// <summary>
        /// Gets and sets the maximum number of window menu items to show.
        /// </summary>
        public static int MaxWindowMenuItems { get; set; } = 10;

        /// <summary>
        /// Gets and sets the maximum number of recent file items to show.
        /// </summary>
        public static int MaxRecentFileItems { get; set; } = 10;

        /// <summary>
        /// Gets and sets whether to show the Undo/Redo buttons on the toolbar.
        /// </summary>
        public static bool ShowUndoRedoButtons { get; set; } = true;

        /// <summary>
        /// Gets and sets whether to compress the project file on close.
        /// </summary>
        public static bool CompressProjectFileOnClose { get; set; } = false;

        /// <summary>
        /// Gets and sets whether to create an AutoRecover backup file.
        /// </summary>
        public static bool CreateAutoRecoverBackup { get; set; } = true;

        /// <summary>
        /// Gets and sets the AutoRecover interval.
        /// </summary>
        public static int AutoRecoverInterval { get; set; } = 30;

        /// <summary>
        /// Gets and sets whether to keep the last backup version if the file were to unexpectedly close.
        /// </summary>
        public static bool KeepLastBackupVersion { get; set; } = true;

        /// <summary>
        /// Gets and sets whether error messages beep.
        /// </summary>
        public static bool ErrorBeep { get; set; } = false;

        /// <summary>
        /// Gets and sets whether warning messages beep.
        /// </summary>
        public static bool WarningBeep { get; set; } = false;

        /// <summary>
        /// Gets and sets whether messages beep.
        /// </summary>
        public static bool MessageBeep { get; set; } = false;

        /// <summary>
        /// Gets and sets whether event messages beep.
        /// </summary>
        public static bool EventBeep { get; set; } = false;

        /// <summary>
        /// Gets and sets the error message color.
        /// </summary>
        public static System.Drawing.Color ErrorColor { get; set; } = System.Drawing.Color.Red;

        /// <summary>
        /// Gets and sets the warning message color.
        /// </summary>
        public static System.Drawing.Color WarningColor { get; set; } = System.Drawing.Color.DarkOrange;

        /// <summary>
        /// Gets and sets the message color.
        /// </summary>
        public static System.Drawing.Color MessageColor { get; set; } = System.Drawing.Color.Blue;

        /// <summary>
        /// Gets and sets the event message color.
        /// </summary>
        public static System.Drawing.Color EventColor { get; set; } = System.Drawing.Color.Black;

        /// <summary>
        /// Gets and sets the default folder location.
        /// </summary>
        public static string DefaultLocation { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        /// <summary>
        /// Gets and sets the default output value decimal digits.
        /// </summary>
        public static int DefaultValueDigits
        {
            get { return _defaultValueDigits; }
            set
            {
                _defaultValueDigits = value;
                SetValueStringFormat(value);
            }
        }

        private static void SetValueStringFormat(int digits)
        {
            string hashString = "";
            for (int i = 0; i < digits; i++)
                hashString += "#";
            ValueStringFormat = "#,##0." + hashString;
        }

        /// <summary>
        /// Gets and sets the value string format
        /// </summary>
        /// <returns></returns>
        public static string ValueStringFormat
        {
            get { return _valueStringFormat; }
            set
            {
                if (_valueStringFormat != value)
                {
                    _valueStringFormat = value;
                    OnGlobalPropertyChanged(nameof(ValueStringFormat));
                }
            }
        }

        private static int _defaultValueDigits = 2;
        private static string _valueStringFormat;

        public event PropertyChangedEventHandler PropertyChanged;
        private static event PropertyChangedEventHandler GlobalPropertyChanged;

        private static void OnGlobalPropertyChanged(string propertyName)
        {
            GlobalPropertyChanged?.Invoke(typeof(UserSettings), new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Restore the default user settings.
        /// </summary>
        /// <remarks>
        /// Make sure to update this sub routine for each application.
        /// </remarks>
        public static void RestoreDefaults()
        {
            // General
            ColorTheme = "Light";
            SaveWindowLayout = true;
            MaxWindowMenuItems = 10;
            MaxRecentFileItems = 10;
            ShowUndoRedoButtons = true;
            // File Management
            CompressProjectFileOnClose = false;
            CreateAutoRecoverBackup = true;
            AutoRecoverInterval = 30;
            KeepLastBackupVersion = true;
            // Message window
            ErrorBeep = false;
            WarningBeep = false;
            MessageBeep = false;
            EventBeep = false;
            ErrorColor = System.Drawing.Color.Red;
            WarningColor = System.Drawing.Color.DarkOrange;
            MessageColor = System.Drawing.Color.Blue;
            EventColor = System.Drawing.Color.Black;
            // Defaults
            DefaultLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DefaultValueDigits = 2;
        }

        /// <summary>
        /// Loads user settings from an XML file.
        /// </summary>
        /// <param name="xmlFilePath">The full path to the XML file containing user settings.</param>
        /// <remarks>
        /// <para>
        /// If the settings file does not exist, default settings are restored.
        /// Invalid or corrupted values in the XML file are silently ignored and
        /// defaults are used instead.
        /// </para>
        /// </remarks>
        public static void Load(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                RestoreDefaults();
                return;
            }

            try
            {
                using (XmlReader xmlReader = XmlReader.Create(xmlFilePath))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType != XmlNodeType.Element) continue;

                        string elementName = xmlReader.Name;
                        string innerXml = null;

                        // TCU
                        if (elementName == nameof(UserAgreedToTCU))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool agreed))
                                UserAgreedToTCU = agreed;
                        }
                        // General
                        else if (elementName == nameof(ColorTheme))
                        {
                            ColorTheme = xmlReader.ReadInnerXml();
                        }
                        else if (elementName == nameof(SaveWindowLayout))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool save))
                                SaveWindowLayout = save;
                        }
                        else if (elementName == nameof(MaxWindowMenuItems))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int maxItems))
                                MaxWindowMenuItems = maxItems;
                        }
                        else if (elementName == nameof(MaxRecentFileItems))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int maxRecent))
                                MaxRecentFileItems = maxRecent;
                        }
                        else if (elementName == nameof(ShowUndoRedoButtons))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool showButtons))
                                ShowUndoRedoButtons = showButtons;
                        }
                        // File Management
                        else if (elementName == nameof(CompressProjectFileOnClose))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool compress))
                                CompressProjectFileOnClose = compress;
                        }
                        else if (elementName == nameof(CreateAutoRecoverBackup))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool autoRecover))
                                CreateAutoRecoverBackup = autoRecover;
                        }
                        else if (elementName == nameof(AutoRecoverInterval))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int interval))
                                AutoRecoverInterval = interval;
                        }
                        else if (elementName == nameof(KeepLastBackupVersion))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool keepBackup))
                                KeepLastBackupVersion = keepBackup;
                        }
                        // Message Window
                        else if (elementName == nameof(ErrorBeep))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool beep))
                                ErrorBeep = beep;
                        }
                        else if (elementName == nameof(WarningBeep))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool beep))
                                WarningBeep = beep;
                        }
                        else if (elementName == nameof(MessageBeep))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool beep))
                                MessageBeep = beep;
                        }
                        else if (elementName == nameof(EventBeep))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (bool.TryParse(innerXml, out bool beep))
                                EventBeep = beep;
                        }
                        else if (elementName == nameof(ErrorColor))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int argb))
                                ErrorColor = UtilityFunctions.IntegerToColor(ref argb);
                        }
                        else if (elementName == nameof(WarningColor))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int argb))
                                WarningColor = UtilityFunctions.IntegerToColor(ref argb);
                        }
                        else if (elementName == nameof(MessageColor))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int argb))
                                MessageColor = UtilityFunctions.IntegerToColor(ref argb);
                        }
                        else if (elementName == nameof(EventColor))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int argb))
                                EventColor = UtilityFunctions.IntegerToColor(ref argb);
                        }
                        // Defaults
                        else if (elementName == nameof(DefaultLocation))
                        {
                            DefaultLocation = xmlReader.ReadInnerXml();
                        }
                        else if (elementName == nameof(DefaultValueDigits))
                        {
                            innerXml = xmlReader.ReadInnerXml();
                            if (int.TryParse(innerXml, out int digits))
                                DefaultValueDigits = digits;
                        }
                    }
                }
            }
            catch (XmlException)
            {
                // If the XML file is corrupted, restore defaults
                RestoreDefaults();
            }
        }

        /// <summary>
        /// Save the user settings to XML.
        /// </summary>
        /// <param name="xmlFilePath">The XML file path for storing the user settings.</param>
        public static void Save(string xmlFilePath)
        {
            // Create the settings directory if it doesn't already exist.
            if (Directory.Exists(Path.GetDirectoryName(xmlFilePath)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(xmlFilePath));
            }
            // Check if the settings file exists. If it does, then delete it. 
            if (File.Exists(xmlFilePath) == true)
            {
                File.Delete(xmlFilePath);
            }
            // Now, save settings.
            using (var xmlWriter = XmlWriter.Create(xmlFilePath, new XmlWriterSettings() { Indent = true }))
            {
                // Write the XML declaration.
                xmlWriter.WriteStartDocument();
                // Write a comment.
                xmlWriter.WriteComment("User Settings");
                // Write settings.
                xmlWriter.WriteStartElement("Settings");
                // 
                // TCU
                // 
                xmlWriter.WriteStartElement(nameof(UserAgreedToTCU));
                xmlWriter.WriteString(UserAgreedToTCU.ToString());
                xmlWriter.WriteEndElement();
                // 
                // General
                // 
                xmlWriter.WriteStartElement(nameof(ColorTheme));
                xmlWriter.WriteString(ColorTheme.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(SaveWindowLayout));
                xmlWriter.WriteString(SaveWindowLayout.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(MaxWindowMenuItems));
                xmlWriter.WriteString(MaxWindowMenuItems.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(MaxRecentFileItems));
                xmlWriter.WriteString(MaxRecentFileItems.ToString());
                xmlWriter.WriteEndElement();
                //
                xmlWriter.WriteStartElement(nameof(ShowUndoRedoButtons));
                xmlWriter.WriteString(ShowUndoRedoButtons.ToString());
                xmlWriter.WriteEndElement();
                //
                // File Management
                // 
                xmlWriter.WriteStartElement(nameof(CompressProjectFileOnClose));
                xmlWriter.WriteString(CompressProjectFileOnClose.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(CreateAutoRecoverBackup));
                xmlWriter.WriteString(CreateAutoRecoverBackup.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(AutoRecoverInterval));
                xmlWriter.WriteString(AutoRecoverInterval.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(KeepLastBackupVersion));
                xmlWriter.WriteString(KeepLastBackupVersion.ToString());
                xmlWriter.WriteEndElement();
                // 
                // Message Window
                // 
                xmlWriter.WriteStartElement(nameof(ErrorBeep));
                xmlWriter.WriteString(ErrorBeep.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(WarningBeep));
                xmlWriter.WriteString(WarningBeep.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(MessageBeep));
                xmlWriter.WriteString(MessageBeep.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(EventBeep));
                xmlWriter.WriteString(EventBeep.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(ErrorColor));
                xmlWriter.WriteString(ErrorColor.ToArgb().ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(WarningColor));
                xmlWriter.WriteString(WarningColor.ToArgb().ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(MessageColor));
                xmlWriter.WriteString(MessageColor.ToArgb().ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(EventColor));
                xmlWriter.WriteString(EventColor.ToArgb().ToString());
                xmlWriter.WriteEndElement();
                // 
                // Defaults
                // 
                xmlWriter.WriteStartElement(nameof(DefaultLocation));
                xmlWriter.WriteString(DefaultLocation.ToString());
                xmlWriter.WriteEndElement();
                // 
                xmlWriter.WriteStartElement(nameof(DefaultValueDigits));
                xmlWriter.WriteString(DefaultValueDigits.ToString());
                xmlWriter.WriteEndElement();
                // 
                // The end of settings.
                xmlWriter.WriteEndElement();
                // Flush and Close the XmlTextWriter.
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }

    }
}
