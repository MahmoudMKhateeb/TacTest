using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using TACHYON.Packing.PackingTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.Packing.PackingTypes
{
    public interface IPackingTypesAppService : IApplicationService
    {
        Task<LoadResult> GetAll(GetAllPackingTypesInput input);

        Task<GetPackingTypeForViewDto> GetPackingTypeForView(int id);

        Task<GetPackingTypeForEditOutput> GetPackingTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditPackingTypeDto input);

        Task Delete(EntityDto input);
    }
}