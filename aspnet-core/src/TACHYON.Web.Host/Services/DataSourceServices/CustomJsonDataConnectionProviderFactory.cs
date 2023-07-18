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
            return new WebDocumentViewerJsonDataConnectionProvider(storedDataSource,
                _connectionStorage.GetAuthorizationHeaderValue(), _connectionStorage.GetServerBaseUrl());
        }
    }
}