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

using System.Windows;

namespace FrameworkUI.ProjectExplorer
{

    /// <summary>
    /// Project Explorer TreeView
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </para>
    /// </remarks>
    public class ProjectExplorerTreeView : ExplorerTreeView
    {

        /// <summary>
        /// Construct new project explorer tree view.
        /// </summary>
        public ProjectExplorerTreeView()
        {
        }

        /// <summary>
        /// Dependency property for the project node.
        /// </summary>
        public static DependencyProperty ProjectNodeProperty = DependencyProperty.Register(nameof(ProjectNode), typeof(ProjectNode), typeof(ProjectExplorerTreeView), new FrameworkPropertyMetadata(null, ProjectNode_PropertyChangedCallback));

        /// <summary>
        /// Gets or sets the project node.
        /// </summary>
        public ProjectNode ProjectNode
        {
            get { return (ProjectNode)GetValue(ProjectNodeProperty); }
            set { SetValue(ProjectNodeProperty, value); }
        }

        /// <summary>
        /// When the project node is changed, update the tree view items.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event data.</param>
        private static void ProjectNode_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(ProjectExplorerTreeView)) return;
            ProjectExplorerTreeView thisControl = (ProjectExplorerTreeView)d;

            // Get the new value
            ProjectNode? newValue = e.NewValue as ProjectNode;
            if (newValue == null) return;

            newValue.ParentTreeView = thisControl;
            thisControl.Items.Clear();
            thisControl.Items.Add(newValue);
            thisControl.Items.Refresh();
            thisControl.UpdateLayout();

        }

    }
}
