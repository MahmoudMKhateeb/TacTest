using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;

namespace TACHYON.Trailers.PayloadMaxWeights.Dtos
{
    public class CreateOrEditPayloadMaxWeightDto : EntityDto<int?>
    {

		[Required]
		[StringLength(PayloadMaxWeightConsts.MaxDisplayNameLength, MinimumLength = PayloadMaxWeightConsts.MinDisplayNameLength)]
		public string DisplayName { get; set; }
		
		
		public int MaxWeight { get; set; }
		
		

    }
}