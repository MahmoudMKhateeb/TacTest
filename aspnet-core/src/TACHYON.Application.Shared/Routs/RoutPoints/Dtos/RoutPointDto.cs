using Abp.Application.Services.Dto;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.AddressBook.Dtos;
using TACHYON.Routs.RoutSteps;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class RoutPointDto: EntityDto<long>
    {
        public string DisplayName { get; set; }

        public int? PickingTypeId { get; set; }
        public long FacilityId { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public virtual int ShippingRequestTripId { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public int? Rating { get; set; }
        [CanBeNull] public string ReceiverFullName { get; set; }
        [DataType(DataType.PhoneNumber)] [CanBeNull] public string ReceiverPhoneNumber { get; set; }
        [CanBeNull] public string ReceiverEmailAddress { get; set; }
        [CanBeNull] public string ReceiverCardIdNumber { get; set; }
        //to do receiver attribute
    }
}
