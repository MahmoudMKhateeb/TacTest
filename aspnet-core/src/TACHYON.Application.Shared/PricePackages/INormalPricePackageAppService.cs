using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.PricePackages.Dto.NormalPricePackage;

namespace TACHYON.PricePackages
{
    public interface INormalPricePackageAppService : IApplicationService
    {
        Task<PagedResultDto<NormalPricePackageDto>> GetAll(GetAllNormalPricePackagesInput input);
        Task<NormalPricePackageDto> GetNormalPricePackage(int id);
        Task CreateOrEdit(CreateOrEditNormalPricePackageDto input);
        Task Delete(EntityDto input);
        Task<CreateOrEditNormalPricePackageDto> GetNormalPricePackageForEdit(int id);
        Task<List<SelectItemDto>> GetAllCitiesForTableDropdown();
        Task<List<SelectItemDto>> GetAllTranspotTypesForTableDropdown();
        Task<List<SelectItemDto>> GetAllTruckTypesForTableDropdown(int transpotTypeId);
        Task<PagedResultDto<PricePackageForRequestDto>> GetMatchingPricePackagesForRequest(GetAllPricePackagesForRequestInput input);
        Task<PricePackageOfferDto> GetPricePackageOffer(int pricePackageOfferId, long shippingRequestId);
        Task HandlePricePackageOfferToCarrier(int pricePackageId, long shippingRequestId);
        Task<List<SelectItemDto>> GetCarriers();
    }
}