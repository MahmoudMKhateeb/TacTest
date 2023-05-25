using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;
using System.Collections.Generic;

namespace TACHYON.Web.Services
{
    public class CustomJsonDataConnectionProviderFactory : IJsonDataConnectionProviderFactory {

        Dictionary<string, string> connectionStrings;

        public CustomJsonDataConnectionProviderFactory()
        {

            connectionStrings = CustomDataSourceWizardJsonDataConnectionStorage.Connections;
        }

        public IJsonDataConnectionProviderService Create()
        {
            return new WebDocumentViewerJsonDataConnectionProvider(connectionStrings);
        }
    }

    public class WebDocumentViewerJsonDataConnectionProvider : IJsonDataConnectionProviderService
    {
        readonly Dictionary<string, string> jsonDataConnections;
        public WebDocumentViewerJsonDataConnectionProvider(Dictionary<string, string> jsonDataConnections)
        {
            this.jsonDataConnections = jsonDataConnections;
        }
        public JsonDataConnection GetJsonDataConnection(string name)
        {
            if (jsonDataConnections == null)
                return null;
            return CustomDataSourceWizardJsonDataConnectionStorage.CreateJsonDataConnectionFromString(
                name, jsonDataConnections[name]);
        }
    }
}