using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Integration.BayanIntegration
{
    public interface ICanBeExcludedFromBayanIntegration
    {
         bool ExcludeFromBayanIntegration { get; set; }
    }
}
