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

using System.Xml.Linq;
using DAG;

namespace DAG.Tests
{
    /// <summary>
    /// A simple node implementation for unit testing purposes.
    /// </summary>
    public class SimpleTestNode : NodeBase
    {
        private string _name;

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public override string Name => _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTestNode"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        public SimpleTestNode(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTestNode"/> class with a default name.
        /// </summary>
        public SimpleTestNode() : this("TestNode")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTestNode"/> class from an XML element.
        /// </summary>
        /// <param name="nodeElement">The XML element containing the node data.</param>
        public SimpleTestNode(XElement nodeElement) : base(nodeElement)
        {
            _name = nodeElement.Attribute(nameof(Name))?.Value ?? "TestNode";
            // Recreate connectors — NodeBase serialization stores connections by index,
            // so the deserialized node must have the same connector layout.
            AddInput("In", "");
            AddOutput("Out", "");
        }

        /// <summary>
        /// Adds an input connector to this node.
        /// </summary>
        /// <param name="name">The name of the input connector.</param>
        /// <param name="unit">The unit of the input connector.</param>
        /// <returns>The created input connector.</returns>
        public InConnector AddInput(string name, string unit = "")
        {
            var connector = new InConnector(name, unit, this);
            Inputs.Add(connector);
            return connector;
        }

        /// <summary>
        /// Adds an output connector to this node.
        /// </summary>
        /// <param name="name">The name of the output connector.</param>
        /// <param name="unit">The unit of the output connector.</param>
        /// <returns>The created output connector.</returns>
        public OutConnector AddOutput(string name, string unit = "")
        {
            var connector = new OutConnector(name, unit, this);
            Outputs.Add(connector);
            return connector;
        }

        /// <summary>
        /// Creates a deep copy of this node.
        /// </summary>
        /// <returns>A new instance that is a copy of this node.</returns>
        public override NodeBase Clone()
        {
            var result = new SimpleTestNode(_name)
            {
                LeftPosition = LeftPosition,
                TopPosition = TopPosition
            };

            foreach (var output in _outputs)
            {
                result.Outputs.Add(new OutConnector(output.Name, output.Unit, result));
            }

            foreach (var input in _inputs)
            {
                result.Inputs.Add(new InConnector(input.Name, input.Unit, result));
            }

            return result;
        }

        /// <summary>
        /// Adds custom data to the XML element during serialization.
        /// </summary>
        /// <param name="baseElement">The base XML element to add data to.</param>
        protected override void AddToBaseElement(XElement baseElement)
        {
            // No additional data to serialize for simple test node
        }
    }
}
