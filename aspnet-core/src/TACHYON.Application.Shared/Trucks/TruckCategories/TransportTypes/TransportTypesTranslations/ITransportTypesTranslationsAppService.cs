using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos;
using TACHYON.Dto;
using System.Collections.Generic;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations
{
    public interface ITransportTypesTranslationsAppService : IApplicationService
    {
        Task<PagedResultDto<GetTransportTypesTranslationForViewDto>> GetAll(GetAllTransportTypesTranslationsInput input);

        Task<GetTransportTypesTranslationForViewDto> GetTransportTypesTranslationForView(int id);

        Task<GetTransportTypesTranslationForEditOutput> GetTransportTypesTranslationForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTransportTypesTranslationDto input);

        Task Delete(EntityDto input);

        Task<List<TransportTypesTranslationTransportTypeLookupTableDto>> GetAllTransportTypeForTableDropdown();

    }
}