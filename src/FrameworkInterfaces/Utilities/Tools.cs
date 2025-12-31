using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkInterfaces.Utilities
{
    public static class Tools
    {
        public static DateTime DateFromString(string dateString)
        {
            DateTime result;
            if (DateTime.TryParse(dateString, out result) == false)
                result = DateTime.Now;
            return result.ToLocalTime();
        }

        public static string DateToUniversalString(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString();
        }

    }
}
