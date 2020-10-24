using System;
using Abp;

namespace TACHYON.Trucks.Dtos
{
    public class ImportTrucksFromExcelJobArgs
    {
        public int? TenantId { get; set; }

        public Guid BinaryObjectId { get; set; }

        public UserIdentifier User { get; set; }
    }
}
