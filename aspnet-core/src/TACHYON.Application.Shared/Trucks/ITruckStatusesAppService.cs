using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;


namespace TACHYON.Trucks
{
    public interface ITruckStatusesAppService : IApplicationService
    {
        Task<LoadResult> GetAll(GetAllTruckStatusesInput input);

        Task<GetTruckStatusForViewDto> GetTruckStatusForView(long id);

        Task<GetTruckStatusForEditOutput> GetTruckStatusForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditTruckStatusDto input);

        Task Delete(EntityDto<long> input);

        Task<LoadResult> GetAllTranslations(GetAllTruckStatusesTranslationsInput input);

        Task CreateOrEditTranslation(CreateOrEditTruckStatusesTranslationDto input);

        Task DeleteTranslation(EntityDto input);

    }
}