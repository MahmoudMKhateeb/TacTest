using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Offers.Dtos;

namespace TACHYON.Offers.Exporting
{
    public interface IOffersExcelExporter
    {
        FileDto ExportToFile(List<GetOfferForViewDto> offers);
    }
}