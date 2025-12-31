using GenericControls;
using FrameworkInterfaces.Utilities;
using FrameworkUI.ProjectExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrameworkUI.ProjectExplorer
{
    public class SimpleNode : Node
    {
        private string _nodeName = "Node Name";
        public SimpleNode(string name, Node parentNode, ExplorerTreeView parentTreeView) : base(parentNode, parentTreeView)
        {
            AllowDrop = true;
            IsReadOnly = false;
            _nodeName = name;

            // Node Header Appearance
            //NodeHeader.ShowToolTip = true;
            NodeHeader.StaticImage = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.MoveDown);
            NodeHeader.ExpandedImage = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.MoveUp);

            // Event Handlers

            // Set bindings           
            NodeHeader.SetBinding(NodeHeader.HeaderTextProperty, new Binding(nameof(NodeName)) { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
        }

        public override bool CanMultiSelect => true;

        public string NodeName
        {
            get { return _nodeName; }
            set
            {
                if (value == _nodeName) { return; }
                _nodeName = value;
                RaisePropertyChange(nameof(NodeName));
            }
        }
    }
}
