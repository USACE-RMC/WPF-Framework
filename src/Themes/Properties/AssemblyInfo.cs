// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Themes")]
[assembly: AssemblyDescription("A WPF theme library providing Visual Studio 2013 inspired themes with thread-safe theme switching.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("USACE Risk Management Center")]
[assembly: AssemblyProduct("Themes")]
[assembly: AssemblyCopyright("Copyright © USACE 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// WPF Theme Library specific attributes
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            // where theme specific resource dictionaries are located
                                                // (used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   // where the generic resource dictionary is located
                                                // (used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]

// XML namespace definitions for XAML
[assembly: XmlnsDefinition("http://schemas.themes.com/wpf", "Themes")]
[assembly: XmlnsPrefix("http://schemas.themes.com/wpf", "themes")]
