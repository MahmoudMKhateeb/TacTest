using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Threading.Tasks;
using TACHYON.DriverLicenseTypes.Dtos;
using TACHYON.Dto;


namespace TACHYON.DriverLicenseTypes
{
    public interface IDriverLicenseTypesAppService : IApplicationService
    {
        Task<LoadResult> GetAll(GetAllDriverLicenseTypesInput input);

        Task<GetDriverLicenseTypeForEditOutput> GetDriverLicenseTypeForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditDriverLicenseTypeDto input);

        Task Delete(EntityDto input);


    }
}