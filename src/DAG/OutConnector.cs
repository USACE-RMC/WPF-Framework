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

using System.ComponentModel;

namespace DAG
{
    /// <summary>
    /// Represents an output connector on a node in a directed acyclic graph (DAG).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Output connectors are the sending end of connections between nodes. Each output connector
    /// can connect to multiple input connectors on other nodes, allowing one output to feed into
    /// multiple downstream nodes.
    /// </para>
    /// <para>
    /// Output connectors have an optional <see cref="Unit"/> property that can be used to validate
    /// connection compatibility. When units are specified, connections are typically only allowed
    /// between connectors with matching units.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create an output connector for a node
    /// var output = new OutConnector("Calculated Flow", "m3/s", myNode);
    /// myNode.Outputs.Add(output);
    /// </code>
    /// </example>
    public class OutConnector : INotifyPropertyChanged
    {
        #region Fields

        private string _name = "";
        private string _unit = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the display name of the output connector.
        /// </summary>
        /// <value>The name shown next to the connector in the node control.</value>
        public string Name
        {
            get => _name;
            set => Utilities.SetString(value, ref _name, PropertyChanged, this);
        }

        /// <summary>
        /// Gets or sets the unit of measurement for this output connector.
        /// </summary>
        /// <value>
        /// A string representing the unit (e.g., "kg/s", "m", "PSI").
        /// An empty string indicates no unit restriction.
        /// </value>
        /// <remarks>
        /// Units can be used to validate connection compatibility. If units do not match between
        /// an input and output connector, the connection may be considered invalid by the application.
        /// </remarks>
        public string Unit
        {
            get => _unit;
            set => Utilities.SetString(value, ref _unit, PropertyChanged, this);
        }

        /// <summary>
        /// Gets the parent node that owns this connector.
        /// </summary>
        /// <value>The <see cref="NodeBase"/> instance that this connector belongs to.</value>
        /// <remarks>
        /// The parent reference is set during construction and cannot be changed.
        /// This ensures connectors maintain a stable relationship with their owning node.
        /// </remarks>
        public NodeBase Parent { get; } = null;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutConnector"/> class.
        /// </summary>
        /// <param name="name">The display name of the connector.</param>
        /// <param name="unit">The unit of measurement for connection validation. Use empty string for no unit.</param>
        /// <param name="parent">The node that owns this connector.</param>
        /// <example>
        /// <code>
        /// var flowOutput = new OutConnector("Flow Rate", "m3/s", parentNode);
        /// var signalOutput = new OutConnector("Complete", "", parentNode); // No unit
        /// </code>
        /// </example>
        public OutConnector(string name, string unit, NodeBase parent)
        {
            _name = name;
            _unit = unit;
            Parent = parent;
        }

        #endregion
    }
}
