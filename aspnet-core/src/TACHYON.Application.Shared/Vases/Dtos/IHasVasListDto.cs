using System.Collections.Generic;

namespace TACHYON.Vases.Dtos
{
    public interface IHasVasListDto
    {
        List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }
    }
}