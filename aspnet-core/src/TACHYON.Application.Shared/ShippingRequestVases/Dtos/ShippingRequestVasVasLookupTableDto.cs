using Abp.Application.Services.Dto;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class ShippingRequestVasVasLookupTableDto
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public bool IsOther { get; set; }
    }
}