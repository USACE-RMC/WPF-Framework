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

namespace FrameworkInterfaces
{
    /// <summary>
    /// Provides extension methods for framework interfaces.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the names of each element in the IElementCollection.
        /// </summary>
        /// <param name="elementCollection">The element collection to query.</param>
        /// <param name="elementToIgnore">Optional element that will be ignored in the result.</param>
        /// <returns>List of element names.</returns>
        public static List<string> GetElementNames(this IElementCollection elementCollection, IElement? elementToIgnore = null)
        {
            var elementNames = new List<string>();
            if (elementToIgnore == null)
            {
                foreach (var element in elementCollection) { elementNames.Add(element.Name); }
            }
            else
            {
                foreach (var element in elementCollection)
                {
                    if (element.Equals(elementToIgnore)) { continue; }
                    elementNames.Add(element.Name);
                }
            }
            return elementNames;
        }

        /// <summary>
        /// Gets an element collection based on the supplied type (T).
        /// </summary>
        /// <typeparam name="T">Type of element collection.</typeparam>
        /// <param name="project">Project to search.</param>
        /// <returns>T or Nothing</returns>
        public static T? ElementCollection<T>(this IProject project)
        {
            if (project.ElementCollections == null) return default;
            foreach (var collection in project.ElementCollections)
            {
                if (collection.GetType() == typeof(T)) { return (T)collection; }
            }
            return default;
        }


    }
}
