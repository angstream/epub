using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpubEditor.Model
{
    public static class StringHelper
    {
        public static bool EndsWith(this string str, string[] arr)
        {
            // Loop through and test each string
            foreach (string s in arr)
            {
                if (str.EndsWith(s, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

    }

}
