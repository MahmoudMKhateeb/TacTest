using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
using TACHYON.PricePackages.Dto.PricePackageAppendices;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    public interface IPricePackageAppendixManager : IDomainService
    {
        Task<BinaryObject> GenerateAppendixFile(int appendixId);

        Task CreateAppendix(PricePackageAppendix createdAppendix, List<string> pricePackages, string emailAddress);

        Task UpdateAppendix(CreateOrEditAppendixDto updatedAppendix, List<string> pricePackages, string emailAddress);
    }
}