using Abp.Domain.Services;
using System.Threading.Tasks;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public interface ITmsPricePackageManager : IDomainService
    {
        Task SendOfferByPricePackage(int pricePackageId, long srId);

        Task AcceptOfferByPricePackage(TmsPricePackage pricePackage);
    }
}