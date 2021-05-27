using System;

namespace TACHYON.Terminologies
{
    [Flags]
    public enum TerminologyAppVersion
    {
        Angular = 1,
        Mobile = 2,
        Backend =3,
        BothAngularANDMobile = 4,
        All=5
        
    }
}
