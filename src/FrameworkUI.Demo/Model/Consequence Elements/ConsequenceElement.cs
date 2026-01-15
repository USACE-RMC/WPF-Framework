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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkInterfaces;

namespace FrameworkUI.Demo
{
    /// <summary>
    /// Represents a consequence element in the demo project.
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
    internal class ConsequenceElement : ElementBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsequenceElement"/> class.
        /// </summary>
        /// <param name="name">The name of the consequence element.</param>
        /// <param name="parentCollection">The parent collection that contains this element.</param>
        public ConsequenceElement(string name, IElementCollection parentCollection) : base(name, parentCollection)
        {
            Name = name;
            SetIsDirty(false);
        }

        /// <summary>
        /// Gets or sets the name of the consequence element.
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the consequence element.
        /// </summary>
        public override string Description { get => "Consequence Element"; set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the creation date of the consequence element.
        /// </summary>
        public override DateTime CreationDate => DateTime.Now;

        /// <summary>
        /// Gets the last modified date of the consequence element.
        /// </summary>
        public override DateTime LastModified => DateTime.Now;

        /// <summary>
        /// Gets the name used when saving the element to disk.
        /// </summary>
        public override string NameOnDisk => "Consequence Element";

        /// <summary>
        /// Gets the image icon representing the element.
        /// </summary>
        public override Bitmap ElementImage => Properties.Resources.Add;

        /// <summary>
        /// Gets a value indicating whether this element can be copied from an external project.
        /// </summary>
        public override bool CanCopyFromExternal => true;

        /// <summary>
        /// Gets a value indicating whether the element is valid.
        /// </summary>
        public override bool IsValid { get => true; }

        /// <summary>
        /// Creates a copy of the consequence element.
        /// </summary>
        /// <param name="newName">The name for the copied element. If null, a default name will be used.</param>
        /// <returns>A copy of the element, or null if copying is not implemented.</returns>
        public override IElement Copy(string newName = null)
        {
            //throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Copies an element from an external project file.
        /// </summary>
        /// <param name="itemName">The name of the item to copy.</param>
        /// <param name="fullFileName">The full path to the external file.</param>
        /// <returns>A copy of the external element, or null if not implemented.</returns>
        public override IElement CopyFromExternal(string itemName, string fullFileName)
        {
            //throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Deletes the consequence element.
        /// </summary>
        public override void Delete()
        {
            //throw new NotImplementedException();
        }


        /// <summary>
        /// Opens the consequence element for editing.
        /// </summary>
        public override void Open()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the consequence element to disk.
        /// </summary>
        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
