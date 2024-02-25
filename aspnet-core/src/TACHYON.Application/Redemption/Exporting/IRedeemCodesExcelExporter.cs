using System.Collections.Generic;
using TACHYON.Redemption.Dtos;
using TACHYON.Dto;

namespace TACHYON.Redemption.Exporting
{
    public interface IRedeemCodesExcelExporter
    {
        FileDto ExportToFile(List<GetRedeemCodeForViewDto> redeemCodes);
    }
}