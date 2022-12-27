using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.Countries;

namespace TACHYON.ServiceAreas
{
    public class ServiceArea : CreationAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public int CityId { get; set; }

        [ForeignKey(nameof(CityId))]
        public City City { get; set; }
    }
}