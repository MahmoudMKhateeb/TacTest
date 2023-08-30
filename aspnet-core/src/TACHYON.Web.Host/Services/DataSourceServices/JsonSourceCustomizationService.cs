using DevExpress.DataAccess.Json;
using System;

namespace TACHYON.Web.Services.DataSourceServices
{
    public class JsonSourceCustomizationService : IJsonSourceCustomizationService
    {
        public JsonSourceBase CustomizeJsonSource(JsonDataSource jsonDataSource)
        {
            Console.WriteLine(jsonDataSource);
            return new UriJsonSource(new Uri("https://northwind.netcore.io/customers.json"));
        }
    }
}
