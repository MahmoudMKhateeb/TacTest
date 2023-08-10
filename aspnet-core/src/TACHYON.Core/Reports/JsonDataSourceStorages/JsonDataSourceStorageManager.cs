using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Reports.JsonDataSourceStorages;

namespace TACHYON.Reports.ReportDataSources
{
    public class JsonDataSourceStorageManager : TACHYONDomainServiceBase, IJsonDataSourceStorageManager
    {
        private readonly IRepository<JsonDataSourceStorage, long> _jsonDataSourceStorage;

        public JsonDataSourceStorageManager(IRepository<JsonDataSourceStorage, long> jsonDataSourceStorage)
        {
            _jsonDataSourceStorage = jsonDataSourceStorage;
        }


        public async Task SaveDataSource(string connectionName, string connectionValue)
        {
            var createdDataSource = new JsonDataSourceStorage { ConnectionName = connectionName, ConnectionValue = connectionValue };

            await _jsonDataSourceStorage.InsertAsync(createdDataSource);
        }

        public async Task<Dictionary<string,string>> GetDataSources()
        {
            var dataSources = await _jsonDataSourceStorage.GetAll()
                .ToDictionaryAsync(x => x.ConnectionName, x => x.ConnectionValue);
            return dataSources;
        }

        public async Task<bool> IsDataSourceExist(string connectionName)
        {
            return await _jsonDataSourceStorage.GetAll().AnyAsync(x => x.ConnectionName.Trim().Equals(connectionName.Trim()));
        }

        public async Task<JsonDataSourceStorage> GetDataSource(string connectionName)
        {
            return await _jsonDataSourceStorage.GetAll()
                .FirstOrDefaultAsync(x => x.ConnectionName.Trim().Equals(connectionName.Trim()));
        }
    }
}