using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Common
{
  public  class AppResult
    {
        public bool Status { get; set; }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> Notifications { get; set; }
    }
}
