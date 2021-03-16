using Abp.Application.Services.Dto;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Goods.GoodsDetails.Dtos;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class CreateOrEditRoutPointDto: EntityDto<long?>
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
        [CanBeNull] public string ReceiverFullName { get; set; }
        [DataType(DataType.PhoneNumber)] [CanBeNull] public string ReceiverPhoneNumber { get; set; }
        [CanBeNull] public string ReceiverEmailAddress { get; set; }
        [CanBeNull] public string ReceiverCardIdNumber { get; set; }
        public List<CreateOrEditGoodsDetailDto> GoodsDetailListDto { get; set; }

    }
}
