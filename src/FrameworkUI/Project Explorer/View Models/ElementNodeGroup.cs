using GenericControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrameworkUI.ProjectExplorer
{

    /// <summary>
    /// Element node group class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class ElementNodeGroup : NodeGroup
    {
        /// <summary>
        /// Construct a new element node group.
        /// </summary>
        /// <param name="element">The project IElement.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="parentTreeView">The parent tree view.</param>
        public ElementNodeGroup(Node parentNode, ProjectExplorerTreeView parentTreeView) : base(parentNode, parentTreeView)
        {

        }

    }
}
