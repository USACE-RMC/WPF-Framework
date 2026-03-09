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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;

namespace DAG
{
    /// <summary>
    /// Abstract base class for all nodes in a directed acyclic graph (DAG).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nodes are the fundamental building blocks of a DAG. Each node has a unique identifier,
    /// a collection of input connectors, a collection of output connectors, and a position
    /// for visual display on a canvas.
    /// </para>
    /// <para>
    /// Derived classes must implement <see cref="Name"/>, <see cref="Clone"/>, and
    /// <see cref="AddToBaseElement"/> to provide custom node behavior and serialization.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyCustomNode : NodeBase
    /// {
    ///     public override string Name => "Custom Node";
    ///
    ///     public MyCustomNode()
    ///     {
    ///         Inputs.Add(new InConnector("Input 1", "data", this));
    ///         Outputs.Add(new OutConnector("Output 1", "data", this));
    ///     }
    ///
    ///     public override NodeBase Clone() { /* implementation */ }
    ///     protected override void AddToBaseElement(XElement baseElement) { /* implementation */ }
    /// }
    /// </code>
    /// </example>
    public abstract class NodeBase : INotifyPropertyChanged
    {
        #region Fields

        private Guid _nodeGuid = Guid.NewGuid();
        private double _leftPosition = 10;
        private double _topPosition = 10;

        /// <summary>
        /// The collection of output connectors for this node.
        /// </summary>
        protected readonly ObservableCollection<OutConnector> _outputs = new ObservableCollection<OutConnector>();

        /// <summary>
        /// The collection of input connectors for this node.
        /// </summary>
        protected readonly ObservableCollection<InConnector> _inputs = new ObservableCollection<InConnector>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the display name of the node.
        /// </summary>
        /// <value>A string representing the name shown in the node's header.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the unique identifier for this node.
        /// </summary>
        /// <value>A <see cref="Guid"/> that uniquely identifies this node within the graph.</value>
        /// <remarks>
        /// The GUID is automatically generated when the node is created and is preserved
        /// during serialization/deserialization to maintain node identity.
        /// </remarks>
        public Guid NodeGuid => _nodeGuid;

        /// <summary>
        /// Gets the collection of output connectors for this node.
        /// </summary>
        /// <value>An observable collection of <see cref="OutConnector"/> objects.</value>
        /// <remarks>
        /// Output connectors can be connected to input connectors of other nodes to form edges in the graph.
        /// </remarks>
        public ObservableCollection<OutConnector> Outputs => _outputs;

        /// <summary>
        /// Gets the collection of input connectors for this node.
        /// </summary>
        /// <value>An observable collection of <see cref="InConnector"/> objects.</value>
        /// <remarks>
        /// Input connectors receive connections from output connectors of other nodes.
        /// Each input connector can only have one incoming connection.
        /// </remarks>
        public ObservableCollection<InConnector> Inputs => _inputs;

        /// <summary>
        /// Gets or sets the horizontal position of the node on the canvas.
        /// </summary>
        /// <value>The left position in canvas coordinates. Default is 10.</value>
        public double LeftPosition
        {
            get => _leftPosition;
            set => Utilities.SetDouble(value, ref _leftPosition, PropertyChanged, this);
        }

        /// <summary>
        /// Gets or sets the vertical position of the node on the canvas.
        /// </summary>
        /// <value>The top position in canvas coordinates. Default is 10.</value>
        public double TopPosition
        {
            get => _topPosition;
            set => Utilities.SetDouble(value, ref _topPosition, PropertyChanged, this);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeBase"/> class.
        /// </summary>
        public NodeBase() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeBase"/> class from an XML element.
        /// </summary>
        /// <param name="nodeElement">The XML element containing the serialized node data.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="nodeElement"/> is null.</exception>
        /// <exception cref="FormatException">Thrown when the XML element is not named "Node".</exception>
        /// <remarks>
        /// The XML element must contain attributes for NodeGuid, LeftPosition, and TopPosition.
        /// Derived classes should call this constructor and then parse their own custom attributes.
        /// </remarks>
        public NodeBase(XElement nodeElement)
        {
            if (nodeElement == null) { throw new NullReferenceException("XElement Node can't be null."); }
            if (nodeElement.Name != "Node") { throw new FormatException("XElement must be named Node. It is currently " + nodeElement.Name); }

            _ = Guid.TryParse(nodeElement.Attribute(nameof(NodeGuid))?.Value, out _nodeGuid);
            _ = double.TryParse(nodeElement.Attribute(nameof(LeftPosition))?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _leftPosition);
            _ = double.TryParse(nodeElement.Attribute(nameof(TopPosition))?.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out _topPosition);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// When overridden in a derived class, adds custom data to the XML element during serialization.
        /// </summary>
        /// <param name="baseElement">The base XML element to add custom data to.</param>
        /// <remarks>
        /// This method is called by <see cref="ToXElement"/> to allow derived classes to serialize
        /// their custom properties. The base element already contains the standard node properties.
        /// </remarks>
        protected abstract void AddToBaseElement(XElement baseElement);

        /// <summary>
        /// When overridden in a derived class, creates a deep copy of this node.
        /// </summary>
        /// <returns>A new <see cref="NodeBase"/> instance that is a copy of this node.</returns>
        /// <remarks>
        /// <para>
        /// The cloned node should have a new <see cref="NodeGuid"/> and copies of all connectors.
        /// The connector copies must reference the new cloned node as their parent.
        /// </para>
        /// <para>
        /// Important: When copying connectors, ensure the parent reference points to the cloned node,
        /// not the original node.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// public override NodeBase Clone()
        /// {
        ///     var clone = new MyNode
        ///     {
        ///         LeftPosition = this.LeftPosition,
        ///         TopPosition = this.TopPosition
        ///     };
        ///
        ///     foreach (var output in _outputs)
        ///     {
        ///         clone.Outputs.Add(new OutConnector(output.Name, output.Unit, clone)); // Note: parent is 'clone'
        ///     }
        ///
        ///     return clone;
        /// }
        /// </code>
        /// </example>
        public abstract NodeBase Clone();

        /// <summary>
        /// Serializes the node to an XML element.
        /// </summary>
        /// <returns>An <see cref="XElement"/> containing the serialized node data.</returns>
        /// <remarks>
        /// The XML element includes the node's name, GUID, and position. Custom data is added
        /// through the <see cref="AddToBaseElement"/> method.
        /// </remarks>
        public XElement ToXElement()
        {
            XElement nodeElement = new XElement("Node");
            nodeElement.SetAttributeValue(nameof(Name), Name);
            nodeElement.SetAttributeValue(nameof(NodeGuid), _nodeGuid);
            nodeElement.SetAttributeValue(nameof(LeftPosition), _leftPosition.ToString("G17", CultureInfo.InvariantCulture));
            nodeElement.SetAttributeValue(nameof(TopPosition), _topPosition.ToString("G17", CultureInfo.InvariantCulture));
            AddToBaseElement(nodeElement);

            return nodeElement;
        }

        #endregion
    }
}
