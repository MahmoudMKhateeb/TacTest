using System.Collections.Generic;
using TACHYON.Offers.Dtos;
using TACHYON.Dto;

namespace TACHYON.Offers.Exporting
{
    public interface IOffersExcelExporter
    {
        FileDto ExportToFile(List<GetOfferForViewDto> offers);
    }
}