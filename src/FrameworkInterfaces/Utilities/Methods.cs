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
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkInterfaces
{
    /// <summary>
    /// Provides utility methods for property change notification and value comparison.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public static class Methods
    {
        /// <summary>
        /// Sets a string property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="target">The target field to update.</param>
        /// <param name="propertyChanged">The PropertyChanged event handler.</param>
        /// <param name="source">The source object raising the event.</param>
        /// <param name="action">Optional action to execute after setting the value.</param>
        /// <param name="memberName">The name of the calling property (auto-populated).</param>
        public static void SetString(string value, ref string target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        /// <summary>
        /// Sets a boolean property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="target">The target field to update.</param>
        /// <param name="propertyChanged">The PropertyChanged event handler.</param>
        /// <param name="source">The source object raising the event.</param>
        /// <param name="action">Optional action to execute after setting the value.</param>
        /// <param name="memberName">The name of the calling property (auto-populated).</param>
        public static void SetBoolean(bool value, ref bool target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        /// <summary>
        /// Sets an integer property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="target">The target field to update.</param>
        /// <param name="propertyChanged">The PropertyChanged event handler.</param>
        /// <param name="source">The source object raising the event.</param>
        /// <param name="action">Optional action to execute after setting the value.</param>
        /// <param name="memberName">The name of the calling property (auto-populated).</param>
        public static void SetInteger(int value, ref int target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        /// <summary>
        /// Sets a short property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="target">The target field to update.</param>
        /// <param name="propertyChanged">The PropertyChanged event handler.</param>
        /// <param name="source">The source object raising the event.</param>
        /// <param name="action">Optional action to execute after setting the value.</param>
        /// <param name="memberName">The name of the calling property (auto-populated).</param>
        public static void SetShort(short value, ref short target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        /// <summary>
        /// Sets a byte property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="target">The target field to update.</param>
        /// <param name="propertyChanged">The PropertyChanged event handler.</param>
        /// <param name="source">The source object raising the event.</param>
        /// <param name="action">Optional action to execute after setting the value.</param>
        /// <param name="memberName">The name of the calling property (auto-populated).</param>
        public static void SetByte(byte value, ref byte target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        /// <summary>
        /// Sets a DateTime property value and raises the PropertyChanged event if the value has changed.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        /// <param name="target">The target field to update.</param>
        /// <param name="propertyChanged">The PropertyChanged event handler.</param>
        /// <param name="source">The source object raising the event.</param>
        /// <param name="action">Optional action to execute after setting the value.</param>
        /// <param name="memberName">The name of the calling property (auto-populated).</param>
        public static void SetDateTime(DateTime value, ref DateTime target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (DateTime.Equals(value, target) == true) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

    }
}

