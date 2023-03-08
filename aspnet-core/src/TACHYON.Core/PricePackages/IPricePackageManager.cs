using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.PricePackages.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages
{
    public interface IPricePackageManager : IDomainService
    {

        Task<List<PricePackageSelectItemDto>> GetPricePackagesForCarrierAppendix(int carrierId, int? appendixId = null);

        Task<decimal?> GetItemPriceByMatchedPricePackage(long shippingRequestId, long directRequestId);

        Task<bool> IsHaveMatchedPricePackage(long shippingRequestId, long? directRequestId);
        
        string GeneratePricePackageReferenceNumber(PricePackage pricePackage);

        Task SendNotificationToCarriersWithTheSameTrucks(ShippingRequest shippingRequest);

        Task<long?> GetMatchingPricePackageId(long? truckType, int? originCityId, int? destinationCityId, int? carrierId);
    }
}