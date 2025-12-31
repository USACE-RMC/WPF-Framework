using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkInterfaces.Messaging
{
    public static class Extensions
    {
        public static string ToText(this IMessageItem message)
        {
            string line = "Type: " + message.Type.ToString() + "; ";
            line += "Time: " + message.TimeStamp + "; ";
            line += "Description: " + message.Description + "; ";
            line += "Source: " + message.SourceCollectionName + "; ";
            line += "Name: " + message.SourceName + "; ";
            line += "Parameter: " + message.ParameterName + ".";
            return line;
        }
    }
}
