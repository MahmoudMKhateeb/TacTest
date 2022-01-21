using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TACHYON.PriceOffers.Dto
{
    public class RejectPriceOfferInput : EntityDto<long>
    {
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Reason { get; set; }

        [JsonIgnore] public string RejectBy { get; set; }
    }
}