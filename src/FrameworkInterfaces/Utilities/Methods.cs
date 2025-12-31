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

    public static class Methods
    {

        public static void SetString(string value, ref string target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        public static void SetBoolean(bool value, ref bool target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        public static void SetInteger(int value, ref int target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        public static void SetShort(short value, ref short target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        public static void SetByte(byte value, ref byte target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (value == target) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

        public static void SetDateTime(DateTime value, ref DateTime target, PropertyChangedEventHandler propertyChanged = null, object source = null, Action<string> action = null, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            if (DateTime.Equals(value, target) == true) return;
            target = value;
            action?.Invoke(memberName);
            propertyChanged?.Invoke(source, new PropertyChangedEventArgs(memberName));
        }

    }
}

