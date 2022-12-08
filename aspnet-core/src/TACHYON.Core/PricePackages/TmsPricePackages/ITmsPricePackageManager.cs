using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.PricePackages.Dto.TmsPricePackages;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public interface ITmsPricePackageManager : IDomainService
    {

        Task<List<PricePackageSelectItemDto>> GetPricePackagesForCarrierAppendix(int carrierId, int? appendixId = null);

        Task<decimal?> GetItemPriceByMatchedPricePackage(long shippingRequestId, decimal quantity, int carrierId);

        Task<bool> IsHaveMatchedPricePackage(long shippingRequestId, int carrierId);
    }
}