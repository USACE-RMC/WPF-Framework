using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkInterfaces
{
    /// <summary>
    /// A class for extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get the names of each element in the IElementCollection.
        /// </summary>
        /// <param name="elementCollection"></param>
        /// <param name="elementToIgnore">Optional element that will be ignored in the result.</param>
        /// <returns>List of element names.</returns>
        public static List<string> GetElementNames(this IElementCollection elementCollection, IElement elementToIgnore = null)
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
        public static T ElementCollection<T>(this IProject project)
        {
            foreach (var collection in project.ElementCollections)
            {
                if (collection.GetType() == typeof(T)) { return (T)collection; }
            }
            return default;
        }


    }
}
