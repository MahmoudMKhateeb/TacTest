using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    public interface IPricePackageAppendixManager : IDomainService
    {
        Task<BinaryObject> GenerateAppendixFile(int appendixId);

        Task CreateAppendix(PricePackageAppendix createdAppendix, List<int> tmsPricePackages, string emailAddress);

        Task UpdateAppendix(PricePackageAppendix updatedAppendix, List<int> tmsPricePackages, string emailAddress);
    }
}