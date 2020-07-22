using Abp;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Dto;

namespace TACHYON.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}