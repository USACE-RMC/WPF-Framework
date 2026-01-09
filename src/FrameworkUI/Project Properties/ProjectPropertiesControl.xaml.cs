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

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FrameworkInterfaces;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for ProjectPropertiesControl.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class ProjectPropertiesControl : UserControl
    {
        /// <summary>
        /// Initialize an empty control.
        /// </summary>
        public ProjectPropertiesControl()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Stores the previous name value before editing begins.
        /// </summary>
        private string _previousName;

        /// <summary>
        /// Dependency property for the IProject property.
        /// </summary>
        public static DependencyProperty ProjectProperty = DependencyProperty.Register(nameof(Project), typeof(IProject), typeof(ProjectPropertiesControl), new PropertyMetadata(null, ProjectCallback));

        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        public IProject Project
        {
            get { return (IProject)GetValue(ProjectProperty); }
            set { SetValue(ProjectProperty, value); }
        }

        /// <summary>
        /// Set up the class attributes when the project class is set.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The dependency property changed event arguments.</param>
        private static void ProjectCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(ProjectPropertiesControl)) return;
            ProjectPropertiesControl thisControl = (ProjectPropertiesControl)d;

            if (e.NewValue == null) return;
            IProject newProject = e.NewValue as IProject;
            if (newProject == null) return;
            thisControl.PropertyAttributes.GetClassAttributes(newProject);
        }

        /// <summary>
        /// Dependency property for the existing names property.
        /// </summary>
        public static DependencyProperty ExistingNamesProperty = DependencyProperty.Register(nameof(ExistingNames), typeof(string[]), typeof(ProjectPropertiesControl), new FrameworkPropertyMetadata(new string[] { }));

        /// <summary>
        /// Gets or sets the existing names.
        /// </summary>
        public string[] ExistingNames
        {
            get { return (string[])GetValue(ExistingNamesProperty); }
            set { SetValue(ExistingNamesProperty, value); }
        }

        /// <summary>
        /// On left click, get property attributes.
        /// </summary>
        private void Name_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Project.Name), Project);
        }

        /// <summary>
        /// On left click, get property attributes.
        /// </summary>
        private void Description_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Project.Description), Project);
        }

        /// <summary>
        /// On left click, get property attributes.
        /// </summary>
        private void CreationDate_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Project.CreationDate), Project);
        }

        /// <summary>
        /// On left click, get property attributes.
        /// </summary>
        private void LastModified_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Project.LastModified), Project);
        }

        /// <summary>
        /// On left click, get property attributes.
        /// </summary>
        private void FileDirectory_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Project.FileDirectory), Project);
        }

        /// <summary>
        /// On left click, get property attributes.
        /// </summary>
        private void SoftwareVersion_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Project.SoftwareVersion), Project);
        }

        /// <summary>
        /// When the name gets focus, load existing names.
        /// </summary>
        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _previousName = Project.Name;
            var fullfilepaths = Directory.GetFiles(Project.FileDirectory, "*" + ShellPublicVariables.SoftwareExtension);
            var files = new List<string>();
            for (int i = 0; i < fullfilepaths.Count(); i++)
            {
                if (fullfilepaths[i] != Project.FullFileName)
                {
                    files.Add(Path.GetFileNameWithoutExtension(fullfilepaths[i]));
                }
            }
            ExistingNames = files.ToArray();
        }

        /// <summary>
        /// On lost focus, if the name is not unique, revert the name, and show an error message.
        /// </summary>
        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.NameTextBox.IsValid == true) return;
            Project.Name = _previousName;
        }

    }
}
