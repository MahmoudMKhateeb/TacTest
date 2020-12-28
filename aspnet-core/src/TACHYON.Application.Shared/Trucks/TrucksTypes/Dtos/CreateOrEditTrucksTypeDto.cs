
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class CreateOrEditTrucksTypeDto : EntityDto<long?>
    {

        [Required]
        [StringLength(TrucksTypeConsts.MaxDisplayNameLength, MinimumLength = TrucksTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        public int? TransportTypeId { get; set; }

    }
}