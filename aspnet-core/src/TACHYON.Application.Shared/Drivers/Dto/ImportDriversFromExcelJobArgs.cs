using System;
using Abp;

namespace TACHYON.Drivers.Dto
{
    public class ImportDriversFromExcelJobArgs
    {
        public int? TenantId { get; set; }

        public Guid BinaryObjectId { get; set; }

        public UserIdentifier User { get; set; }
    }
}