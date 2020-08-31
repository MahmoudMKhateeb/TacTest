using TACHYON.Countries;
using TACHYON.Cities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.AddressBook
{
    [Table("Facilities")]
    public class Facility : AddressBaseFullAuditedEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        //todo remove country from here
        public virtual int CountyId { get; set; }

        [ForeignKey("CountyId")]
        public County CountyFk { get; set; }


    }
}