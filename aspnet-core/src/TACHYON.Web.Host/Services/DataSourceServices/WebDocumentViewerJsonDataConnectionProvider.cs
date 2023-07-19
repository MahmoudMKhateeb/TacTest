using DevExpress.DataAccess.Json;
using System.Collections.Generic;

namespace TACHYON.Web.Services.DataSourceServices
{
    public class WebDocumentViewerJsonDataConnectionProvider : IJsonDataConnectionProviderService
    {
        private readonly Dictionary<string, string> _jsonDataConnections;
        private readonly string _authorizationHeader;
        private readonly string _baseUrl;
        public WebDocumentViewerJsonDataConnectionProvider(Dictionary<string, string> jsonDataConnections, string authorizationHeader,string baseUrl)
        {
            _jsonDataConnections = jsonDataConnections;
            _authorizationHeader = authorizationHeader;
            _baseUrl = baseUrl;
        }
        public JsonDataConnection GetJsonDataConnection(string name)
        {
            if (_jsonDataConnections == null)
                return null;
            return CustomDataSourceWizardJsonDataConnectionStorage.CreateJsonDataConnectionFromString(
                name, _jsonDataConnections[name],_authorizationHeader,_baseUrl);
        }
    }
}