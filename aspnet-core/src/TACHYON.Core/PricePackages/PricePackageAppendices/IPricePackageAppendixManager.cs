using Abp.Domain.Services;
using System.Threading.Tasks;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    public interface IPricePackageAppendixManager : IDomainService
    {
        Task<BinaryObject> GenerateAppendixFile(int appendixId);
    }
}