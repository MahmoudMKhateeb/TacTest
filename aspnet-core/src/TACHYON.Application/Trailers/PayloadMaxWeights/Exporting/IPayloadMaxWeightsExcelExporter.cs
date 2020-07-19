using System.Collections.Generic;
using TACHYON.Dto;
using TACHYON.Trailers.PayloadMaxWeights.Dtos;

namespace TACHYON.Trailers.PayloadMaxWeights.Exporting
{
    public interface IPayloadMaxWeightsExcelExporter
    {
        FileDto ExportToFile(List<GetPayloadMaxWeightForViewDto> payloadMaxWeights);
    }
}