using TACHYON.Cities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.AddressBook.Ports
{
    [Table("Ports")]
    public class Port : AddressBaseFullAuditedEntity
    {
    }
}