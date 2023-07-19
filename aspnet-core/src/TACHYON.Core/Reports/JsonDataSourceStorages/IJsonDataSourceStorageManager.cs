using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Reports.JsonDataSourceStorages;

namespace TACHYON.Reports.ReportDataSources
{
    public interface IJsonDataSourceStorageManager : IDomainService
    {
        Task SaveDataSource(string connectionName, string connectionValue);

        Task<Dictionary<string, string>> GetDataSources();

        Task<bool> IsDataSourceExist(string connectionName);
        
        Task<JsonDataSourceStorage> GetDataSource(string connectionName);
    }
}