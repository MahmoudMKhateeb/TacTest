using Abp.Application.Services.Dto;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.AddressBook.Dtos;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.RoutSteps;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class RoutPointDto : EntityDto<long>
    {
        public long? WaybillNumber { get; set; }
        public string DisplayName { get; set; }

        public PickingType PickingType { get; set; }
        public long FacilityId { get; set; }
        public string Facility { get; set; }
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

        public int? ReceiverId { get; set; }

        //receiver or sender full name, binded to sender or receiver cantact name in waybills
        public string SenderOrReceiverContactName { get; set; }

        [CanBeNull] public string ReceiverFullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [CanBeNull]
        public string ReceiverPhoneNumber { get; set; }

        [CanBeNull] public string ReceiverEmailAddress { get; set; }
        [CanBeNull] public string ReceiverCardIdNumber { get; set; }

        public string ReceiverAddress { get; set; }
        public List<GoodsDetailDto> GoodsDetailListDto { get; set; }
        [CanBeNull] public string Note { get; set; }

        //to do receiver attribute
    }
}