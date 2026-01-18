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

using System.Reflection;

namespace FrameworkUI
{
    /// <summary>
    /// A utility class for application attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Woody Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public sealed class ApplicationAttributes
    {

        /// <summary>
        /// Construct empty application attributes.
        /// </summary>
        private ApplicationAttributes() { }

        /// <summary>
        /// Initializes the application attributes.
        /// </summary>
        static ApplicationAttributes()
        {
            try
            {
                Assembly = Assembly.GetEntryAssembly();
                if (Assembly != null)
                {
                    var attributes = Assembly.GetCustomAttributes(false);
                    foreach (object attribute in attributes)
                    {
                        var type = attribute.GetType();
                        if (type == typeof(AssemblyTitleAttribute))
                        {
                            _Title = (AssemblyTitleAttribute)attribute;
                        }
                        if (type == typeof(AssemblyCompanyAttribute))
                        {
                            _Company = (AssemblyCompanyAttribute)attribute;
                        }
                        if (type == typeof(AssemblyCopyrightAttribute))
                        {
                            _Copyright = (AssemblyCopyrightAttribute)attribute;
                        }
                        if (type == typeof(AssemblyProductAttribute))
                        {
                            _Product = (AssemblyProductAttribute)attribute;
                        }
                    }
                    _Version = Assembly.GetName().Version;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The assembly title attribute.
        /// </summary>
        private static readonly AssemblyTitleAttribute _Title = null;
        /// <summary>
        /// The assembly company attribute.
        /// </summary>
        private static readonly AssemblyCompanyAttribute _Company = null;
        /// <summary>
        /// The assembly copyright attribute.
        /// </summary>
        private static readonly AssemblyCopyrightAttribute _Copyright = null;
        /// <summary>
        /// The assembly product attribute.
        /// </summary>
        private static readonly AssemblyProductAttribute _Product = null;
        /// <summary>
        /// The assembly version.
        /// </summary>
        private static readonly Version _Version = null;

        /// <summary>
        /// Gets the application assembly.
        /// </summary>
        public static readonly Assembly Assembly;

        /// <summary>
        /// Gets the assembly title.
        /// </summary>
        public static string Title
        {
            get { return _Title?.Title ?? string.Empty; }
        }

        /// <summary>
        /// Gets the application company name.
        /// </summary>
        public static string CompanyName
        {
            get { return _Company?.Company ?? string.Empty; }
        }

        /// <summary>
        /// Gets the application copyright.
        /// </summary>
        public static string Copyright
        {
            get { return _Copyright?.Copyright ?? string.Empty; }
        }

        /// <summary>
        /// Gets the application product name.
        /// </summary>
        public static string ProductName
        {
            get { return _Product?.Product ?? string.Empty; }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public static string Version
        {
            get { return _Version?.ToString() ?? string.Empty; }
        }

    }
}
