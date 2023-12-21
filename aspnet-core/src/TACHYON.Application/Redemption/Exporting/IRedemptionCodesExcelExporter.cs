using System.Collections.Generic;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;

namespace TACHYON.Redemption.Exporting
{
    public interface IRedemptionCodesExcelExporter
    {
        FileDto ExportToFile(List<GetRedemptionCodeForViewDto> redemptionCodes);
    }
}