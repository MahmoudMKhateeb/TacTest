using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Threading;
using Abp.UI;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using TACHYON.Reports;
using TACHYON.Reports.ReportDataSources;
using TACHYON.Url;

namespace TACHYON.Web.Services.DataSourceServices
{
    public class CustomDataSourceWizardJsonDataConnectionStorage : IDataSourceWizardJsonConnectionStorage
    {
        private readonly IJsonDataSourceStorageManager _jsonDataSourceStorageManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebUrlService _webUrlService;

        public CustomDataSourceWizardJsonDataConnectionStorage(IHttpContextAccessor contextAccessor)
        {
            _jsonDataSourceStorageManager = IocManager.Instance.Resolve<IJsonDataSourceStorageManager>();
            _webUrlService = IocManager.Instance.Resolve<IWebUrlService>();
            _contextAccessor = contextAccessor;
        }

        public Dictionary<string, string> GetConnections()
        {
            return AsyncHelper.RunSync(() => _jsonDataSourceStorageManager.GetDataSources());
        }

        bool IJsonConnectionStorageService.CanSaveConnection
        {
            get => true;
        }
        bool IJsonConnectionStorageService.ContainsConnection(string connectionName)
        {
            return AsyncHelper.RunSync(() => _jsonDataSourceStorageManager.IsDataSourceExist(connectionName));
        }

        IEnumerable<JsonDataConnection> IJsonConnectionStorageService.GetConnections()
        {
            var connections = GetConnections();
            string authorizationHeaderValue = GetAuthorizationHeaderValue();
            var connectionsResult = connections.IsNullOrEmpty() ? new List<JsonDataConnection>()
                : connections.Select(x => CreateJsonDataConnectionFromString(x.Key, x.Value,authorizationHeaderValue,GetServerBaseUrl()));
            return connectionsResult;
        }

        JsonDataConnection IJsonDataConnectionProviderService.GetJsonDataConnection(string name)
        {
            var dataSourceStorage = AsyncHelper.RunSync(() => _jsonDataSourceStorageManager.GetDataSource(name));
            if (dataSourceStorage is null)
                throw new UserFriendlyException("Data source not found");
            
            return CreateJsonDataConnectionFromString(dataSourceStorage.ConnectionName, dataSourceStorage.ConnectionValue,GetAuthorizationHeaderValue(),GetServerBaseUrl());
        }

        public string GetAuthorizationHeaderValue()
        {
            string authorizationHeader = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            return authorizationHeader;
        }

        void IJsonConnectionStorageService.SaveConnection(string connectionName,
            JsonDataConnection dataConnection, bool saveCredentials)
        {
            string connectionString = dataConnection.CreateConnectionString();

            AsyncHelper.RunSync(() => _jsonDataSourceStorageManager.SaveDataSource(connectionName, connectionString));
        }



        public static JsonDataConnection CreateJsonDataConnectionFromString(string connectionName,
            string connectionString,string authorizationHeader, string baseUrl)
        {
            string connectionUrl = connectionString.Replace("Uri=", "");
            var uri = new Uri(connectionUrl);
            string actionUrlPath = uri.AbsolutePath.Split(';')[0];
            string dataSourceUrl = baseUrl.TrimEnd('/') + actionUrlPath;
            UriJsonSource jsonSource = new UriJsonSource(new Uri(dataSourceUrl));
            
            jsonSource.HeaderParameters.Add(new HeaderParameter("Authorization",authorizationHeader));
            return new JsonDataConnection(jsonSource) { StoreConnectionNameOnly = false, Name = connectionName };
        }

        public UriJsonSource CreateDefaultDataSourceByReportType(ReportType reportType)
        {
            string dataSourceUrlPath = reportType switch
            {
                ReportType.TripDetailsReport => ReportDataSourcePaths.TripDetailsDataSourcePath,
                ReportType.PodPerformanceReport => ReportDataSourcePaths.PodPerformanceDataSourcePath,
                ReportType.FinancialReport => ReportDataSourcePaths.FinancialDataSourcePath,
                _ => throw new UserFriendlyException("Not Supported Report Type")
            };
            string dataSourceUrl = GetServerBaseUrl().TrimEnd('/') + dataSourceUrlPath;
            UriJsonSource jsonSource = new UriJsonSource(new Uri(dataSourceUrl));

            jsonSource.HeaderParameters.Add(new HeaderParameter("Authorization",GetAuthorizationHeaderValue()));
            return jsonSource;
        }
        
        public string GetServerBaseUrl()
            => _webUrlService.ServerRootAddressFormat;
    }
}