using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Shipping.Notes.Dto;

namespace TACHYON.Shipping.Notes
{
    public interface IShippingRequestAndTripNotes : IApplicationService
    {
        Task<GetAllShippingRequestAndTripNotesDto> GetShippingRequestNotes(GetAllNotesInput Input);
        Task<GetAllShippingRequestAndTripNotesDto> GetTripNotes(GetAllNotesInput Input);
        Task<CreateOrEditShippingRequestAndTripNotesDto> GetForEdit(EntityDto input);
        Task CreateOrEdit(CreateOrEditShippingRequestAndTripNotesDto input);
        Task Delete(EntityDto<long> input);
    }
}