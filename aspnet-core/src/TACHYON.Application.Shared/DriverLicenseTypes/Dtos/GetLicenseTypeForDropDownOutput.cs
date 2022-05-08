using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.DriverLicenseTypes.Dtos
{
    public class GetLicenseTypeForDropDownOutput
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOther { get; set; }
    }
}