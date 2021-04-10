using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Goods.GoodsDetails.Dtos;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class CreateOrEditRoutPointDto : EntityDto<long?>, ICustomValidate
    {
        public string DisplayName { get; set; }
        public PickingType PickingType { get; set; }
        [Required]
        public long FacilityId { get; set; }

        /// <summary>
        /// is for UI helpping only and will be ignored in mapping
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// is for UI helpping only and will be ignored in mapping
        /// </summary>
        public double Latitude { get; set; }
        public int? ReceiverId { get; set; }

        [CanBeNull] public string ReceiverFullName { get; set; }
        [DataType(DataType.PhoneNumber)] [CanBeNull] public string ReceiverPhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)] [CanBeNull] public string ReceiverEmailAddress { get; set; }
        [CanBeNull] public string ReceiverCardIdNumber { get; set; }

        [CanBeNull] public string Note { get; set; }

        public List<CreateOrEditGoodsDetailDto> GoodsDetailListDto { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (PickingType == PickingType.Pickup) return;
            if (!ReceiverId.HasValue)
            {
                if (string.IsNullOrEmpty(ReceiverFullName)) context.Results.Add(new ValidationResult("PleaseAddReceiverFullName"));
                if (string.IsNullOrEmpty(ReceiverPhoneNumber)) context.Results.Add(new ValidationResult("PleaseAddReceiverPhoneNumber"));
                //if (string.IsNullOrEmpty(ReceiverEmailAddress)) context.Results.Add(new ValidationResult("PleaseAddReceiverEmailAddress"));
                if (string.IsNullOrEmpty(ReceiverCardIdNumber)) context.Results.Add(new ValidationResult("PleaseAddReceiverCardIdNumber"));

            }
        }
    }
}
