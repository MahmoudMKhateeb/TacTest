using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.UnitOfMeasures.Dtos;

namespace TACHYON.UnitOfMeasures
{
    public interface IUnitOfMeasuresAppService : IApplicationService
    {
        Task<LoadResult> GetAll(GetAllUnitOfMeasuresInput input);

        Task<GetUnitOfMeasureForViewDto> GetUnitOfMeasureForView(int id);

        Task<GetUnitOfMeasureForEditOutput> GetUnitOfMeasureForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditUnitOfMeasureDto input);

        Task Delete(EntityDto input);


    }
}