using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.GroupsGroups.Dto
{
   public class GroupShippingRequestDto: IHasCreationTime
    {
        public decimal Price { get; set; }
        public string TruckType { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
