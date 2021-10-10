using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Extension
{
    public static class CustomExtensions
    {
        public static bool ToLowerContains(this String str, string s)
        {
            return str.ToLower().Contains(s.ToLower());
        }
    }
}