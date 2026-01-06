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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkInterfaces;

namespace Demo_FrameworkUI.Project.Response_Elements
{
    /// <summary>
    /// Collection of response function elements for the demo project.
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
    internal class ResponseElementCollection : ElementCollectionBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseElementCollection"/> class.
        /// </summary>
        /// <param name="parentProject">The parent project that contains this collection.</param>
        public ResponseElementCollection(IProject parentProject) : base(parentProject)
        {
            Add(new ResponseElement("Response Function_1", this));
            Add(new ResponseElement("Response Function_2", this));
            Add(new ResponseElement("Response Function_3", this));
            Add(new ResponseElement("Response Function_4", this));
            Add(new ResponseElement("Response Function_5", this));
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Response Functions";
            }
        }

        /// <summary>
        /// Opens the collection and loads elements from disk.
        /// </summary>
        public override void Open()
        {
            _opening = true;
            // read from disk
            _opening = false;
        }

        /// <summary>
        /// Adds a response element to the collection.
        /// </summary>
        /// <param name="item">The element to add to the collection.</param>
        public override void Add(IElement item)
        {
            item.Deleted += ElementDeleted;
            //item.AddMessage += RaiseAddMessage;
            //item.RemoveMessage += RaiseRemoveMessage;
            ElementList.Add((ResponseElement)item);
            // Save to disk
            if (_opening == false)
            {
                //item.IsValid();
                SetIsDirty(true);
            }
            RaiseElementAddedEvent(item);
        }

        /// <summary>
        /// Inserts a response element at the specified index in the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="item">The element to insert.</param>
        public override void Insert(int index, IElement item)
        {
            item.Deleted += ElementDeleted;
            //item.AddMessage += RaiseAddMessage;
            //item.RemoveMessage += RaiseRemoveMessage;
            ElementList.Insert(index, (ResponseElement)item);
            // save to disk
            SetIsDirty(true);
            RaiseElementAddedEvent(item);
        }

        /// <summary>
        /// Deletes the collection from disk.
        /// </summary>
        public override void Delete()
        {
            // Delete from disk
        }

        /// <summary>
        /// Inserts an element from an external project at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="elementName">The name of the element to insert.</param>
        /// <param name="elementType">The type of the element.</param>
        /// <param name="fullFileName">The full path to the external project file.</param>
        public override void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName)
        {
            throw new System.NotImplementedException();
        }
    }
}
