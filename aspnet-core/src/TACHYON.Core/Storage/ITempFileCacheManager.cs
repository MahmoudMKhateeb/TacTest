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
        void SetPods(string key, List<FileDto> files);
        List<FileDto> GetPods(string key);
        List<GetAllFileByteDto> GetFiles(List<string> tokens);
    }
}