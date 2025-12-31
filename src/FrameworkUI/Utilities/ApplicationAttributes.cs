using System;
using System.Reflection;

namespace FrameworkUI
{
    /// <summary>
    /// A utility class for application attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Woody Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static readonly AssemblyTitleAttribute _Title = null;
        private static readonly AssemblyCompanyAttribute _Company = null;
        private static readonly AssemblyCopyrightAttribute _Copyright = null;
        private static readonly AssemblyProductAttribute _Product = null;
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
            get { return _Title.Title; }
        }

        /// <summary>
        /// Gets the application company name.
        /// </summary>
        public static string CompanyName
        {
            get { return _Company.Company; }
        }

        /// <summary>
        /// Gets the application copyright.
        /// </summary>
        public static string Copyright
        {
            get { return _Copyright.Copyright; }
        }

        /// <summary>
        /// Gets the application product name.
        /// </summary>
        public static string ProductName
        {
            get { return _Product.Product; }
        }

        /// <summary>
        /// Gets the application version.
        /// </summary>
        public static string Version
        {
            get { return _Version.ToString(); }
        }

    }
}
