using Abp.Dependency;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Dto;

namespace TACHYON.Storage
{
    public interface ITempFileCacheManager : ITransientDependency
    {
        void SetFile(string token, byte[] content);
        byte[] GetFile(string token);
        void ClearCache(string token);
    }
}