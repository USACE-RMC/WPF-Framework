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

#nullable enable
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace FrameworkUI.Demo;

/// <summary>
/// Provides assembly resolution from a "Libraries" subfolder beside the application executable.
/// This allows the application to keep the exe directory clean by placing all supporting DLLs
/// in a dedicated Libraries folder.
/// </summary>
/// <remarks>
/// <para>
/// The resolver must be registered before any WPF types or external assemblies are loaded.
/// Call <see cref="Register"/> at the very beginning of application startup.
/// </para>
/// <para>
/// The Libraries folder is expected at: {exe directory}\Libraries\
/// </para>
/// </remarks>
internal static class AssemblyResolver
{
    /// <summary>
    /// The path to the Libraries folder beside the application executable.
    /// </summary>
    private static readonly string LibrariesPath =
        Path.Combine(AppContext.BaseDirectory, "libraries");

    /// <summary>
    /// Registers the assembly resolver with the default <see cref="AssemblyLoadContext"/>.
    /// When the runtime cannot find an assembly in the default locations, this resolver
    /// checks the Libraries subfolder.
    /// </summary>
    /// <remarks>
    /// This method must be called before any external assemblies are referenced,
    /// typically as the first line in the application constructor or entry point.
    /// </remarks>
    public static void Register()
    {
        AssemblyLoadContext.Default.Resolving += ResolveFromLibraries;
    }

    /// <summary>
    /// Attempts to resolve an assembly from the Libraries subfolder.
    /// </summary>
    /// <param name="context">The assembly load context requesting resolution.</param>
    /// <param name="assemblyName">The name of the assembly to resolve.</param>
    /// <returns>The loaded assembly if found in the Libraries folder; otherwise, <c>null</c>.</returns>
    private static Assembly? ResolveFromLibraries(AssemblyLoadContext context, AssemblyName assemblyName)
    {
        string dllPath = Path.Combine(LibrariesPath, $"{assemblyName.Name}.dll");
        if (File.Exists(dllPath))
        {
            return context.LoadFromAssemblyPath(dllPath);
        }
        return null;
    }
}
