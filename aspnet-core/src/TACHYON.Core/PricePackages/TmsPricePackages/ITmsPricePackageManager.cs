using Abp.Domain.Services;
using System.Threading.Tasks;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public interface ITmsPricePackageManager : IDomainService
    {
        Task<TmsPricePackage> GetMatchingPricePackage(long shippingRequestId);
    }
}