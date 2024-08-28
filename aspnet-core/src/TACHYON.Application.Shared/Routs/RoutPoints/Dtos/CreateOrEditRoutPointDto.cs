using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Castle.Core.Internal;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class CreateOrEditRoutPointDto : EntityDto<long?>, ICreateOrEditRoutPointDtoBase, ICustomValidate
    {
        public string DisplayName { get; set; }
        public PickingType PickingType { get; set; }
        [Required] public long FacilityId { get; set; }

        /// <summary>
        /// is for UI helpping only and will be ignored in mapping
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// is for UI helpping only and will be ignored in mapping
        /// </summary>
        public double? Latitude { get; set; }

        public int? ReceiverId { get; set; }
        [JsonIgnore] public string Code { get; set; } = (new Random().Next(100000, 999999)).ToString();
        [CanBeNull] public string ReceiverFullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [CanBeNull]
        public string ReceiverPhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [CanBeNull]
        public string ReceiverEmailAddress { get; set; }

        [CanBeNull] public string ReceiverCardIdNumber { get; set; }

        [CanBeNull] public string Note { get; set; }

        #region HomeDelivery
        public DropPaymentMethod? DropPaymentMethod { get; set; }
        public bool? NeedsReceiverCode { get; set; }
        public bool? NeedsPOD { get; set; }
        #region Port Movements
        public int? PointOrder { get; set; }

        public bool DropNeedsAppointment { get; set; }
        public bool DropNeedsClearance { get; set; }

        public TripAppointmentDataDto AppointmentDataDto { get; set; }
        public TripClearancePricesDto TripClearancePricesDto {get; set;}
        #endregion

        #endregion

        public long? TruckId { set; get; }
        public long? DriverUserId { set; get; }

        public int? StorageDays { get; set; }
        public decimal? StoragePricePerDay { get; set; }

        public List<CreateOrEditGoodsDetailDto> GoodsDetailListDto { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (PickingType == PickingType.Pickup) return;
            

            //there is additional receiver
            if (ReceiverPhoneNumber != null)
            {
                if (string.IsNullOrEmpty(ReceiverFullName))
                    context.Results.Add(new ValidationResult("PleaseAddAdditionalReceiverFullName"));
                if (string.IsNullOrEmpty(ReceiverPhoneNumber))
                    context.Results.Add(new ValidationResult("PleaseAddAdditionalReceiverPhoneNumber"));
                else if (!isValidMobileNumber(ReceiverPhoneNumber))
                    context.Results.Add(new ValidationResult("InvalidAdditionalReceiverMobileNumber"));
            }
        }

        private static bool isValidMobileNumber(string inputMobileNumber)
        {
            string strRegex = @"^(5){1}\d{8}$";

            Regex re = new Regex(strRegex);

            return re.IsMatch(inputMobileNumber);
        }
    }
}