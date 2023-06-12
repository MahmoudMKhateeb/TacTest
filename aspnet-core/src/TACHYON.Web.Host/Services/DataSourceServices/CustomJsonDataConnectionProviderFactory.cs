using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace TACHYON.Web.Services.DataSourceServices
{
    public class CustomJsonDataConnectionProviderFactory : IJsonDataConnectionProviderFactory
    {

        private readonly CustomDataSourceWizardJsonDataConnectionStorage _connectionStorage;

        public CustomJsonDataConnectionProviderFactory(IHttpContextAccessor contextAccessor)
        {
            _connectionStorage = new CustomDataSourceWizardJsonDataConnectionStorage(contextAccessor);
        }

        public IJsonDataConnectionProviderService Create()
        {
            var storedDataSource = _connectionStorage.GetConnections();
            return new WebDocumentViewerJsonDataConnectionProvider(storedDataSource,_connectionStorage.GetAuthorizationHeaderValue());
        }
    }

    public class WebDocumentViewerJsonDataConnectionProvider : IJsonDataConnectionProviderService
    {
        private readonly Dictionary<string, string> _jsonDataConnections;
        private readonly string _authorizationHeader;
        public WebDocumentViewerJsonDataConnectionProvider(Dictionary<string, string> jsonDataConnections, string authorizationHeader)
        {
            _jsonDataConnections = jsonDataConnections;
            _authorizationHeader = authorizationHeader;
        }
        public JsonDataConnection GetJsonDataConnection(string name)
        {
            if (_jsonDataConnections == null)
                return null;
            return CustomDataSourceWizardJsonDataConnectionStorage.CreateJsonDataConnectionFromString(
                name, _jsonDataConnections[name],_authorizationHeader);
        }
    }
}