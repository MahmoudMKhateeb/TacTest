using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;

namespace TACHYON.Extension
{
    public static class CustomExtensions
    {

        #region String

        public static bool ToLowerContains(this String str, string s)
        {
            return str.ToLower().Contains(s.ToLower());

        }

        #endregion




    }


}