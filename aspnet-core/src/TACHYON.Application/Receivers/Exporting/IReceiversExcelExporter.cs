using System.Collections.Generic;
using TACHYON.Receivers.Dtos;
using TACHYON.Dto;

namespace TACHYON.Receivers.Exporting
{
    public interface IReceiversExcelExporter
    {
        FileDto ExportToFile(List<GetReceiverForViewDto> receivers);
    }
}