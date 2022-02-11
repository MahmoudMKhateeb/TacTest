using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Integration.WaslIntegration
{
    public interface IWaslIntegrated
    {
        public bool IsWaslIntegrated { get; set; }
        public string WaslIntegrationErrorMsg { get; set; }

    }
}
