using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Dedicated
{
    public enum AttendaceStatus: byte
    {
        Present = 1,
        Absent = 2,
        OverTime = 3
    }
}
