using System.Collections.Generic;
using TACHYON.Trailers.PayloadMaxWeight.Dtos;
using TACHYON.Dto;

namespace TACHYON.Trailers.PayloadMaxWeight.Exporting
{
    public interface IPayloadMaxWeightsExcelExporter
    {
        FileDto ExportToFile(List<GetPayloadMaxWeightForViewDto> payloadMaxWeights);
    }
}