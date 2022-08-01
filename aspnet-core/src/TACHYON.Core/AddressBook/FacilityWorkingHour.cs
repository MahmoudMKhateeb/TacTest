using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.AddressBook
{
    //[Table("FacilityWorkingHours")]
    public class FacilityWorkingHour : FullAuditedEntity<int>
    {
        public long FacilityId { get; set; }
        public Facility FacilityFk { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
