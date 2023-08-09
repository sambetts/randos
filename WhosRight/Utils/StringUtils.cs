using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class StringUtils
    {

        public static string GetNameFromWebSafeString(string webSafeString)
        {
            if (string.IsNullOrEmpty(webSafeString))
            {
                throw new ArgumentNullException(nameof(webSafeString));
            }

            return webSafeString.Replace("_", " ");
        }

        public static string GetWebSafeString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return name.Replace(" ", "_");
        }
    }
}
