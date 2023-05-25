using Abp.Dependency;
using Abp.Runtime.Session;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TACHYON.Web.Services
{
public class CustomDataSourceWizardJsonDataConnectionStorage : IDataSourceWizardJsonConnectionStorage
    {
        public const string JsonDataConnectionsKey = "dxJsonDataConnections";
        public static Dictionary<string, string> Connections ;

        static CustomDataSourceWizardJsonDataConnectionStorage()
        {
            Connections = new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetConnections()
        {
            
            return Connections;
        }

        bool IJsonConnectionStorageService.CanSaveConnection
        {
            get => true;
        }
        bool IJsonConnectionStorageService.ContainsConnection(string connectionName)
        {
            var connections = GetConnections();
            return connections?.ContainsKey(connectionName) ?? false;
        }

        IEnumerable<JsonDataConnection> IJsonConnectionStorageService.GetConnections()
        {
            var connections = GetConnections();
            if (connections == null)
            {
                return new List<JsonDataConnection>();
            }
            return connections.Select(x => CreateJsonDataConnectionFromString(x.Key, x.Value));

        }

        JsonDataConnection IJsonDataConnectionProviderService.GetJsonDataConnection(string name)
        {
            var connections = GetConnections();
            if (connections == null || !connections.ContainsKey(name))
                throw new InvalidOperationException();
            return CreateJsonDataConnectionFromString(name, connections[name]);
        }

        void IJsonConnectionStorageService.SaveConnection(string connectionName,
            JsonDataConnection dataConnection, bool saveCredentials)
        {
            var connections = GetConnections();
            if (connections == null)
            {
                return;
            }
            var connectionString = dataConnection.CreateConnectionString();
            if (connections.ContainsKey(connectionName))
            {
                connections[connectionName] = connectionString;
            }
            else
            {
                connections.Add(connectionName, connectionString);
            }
            UpdateSessionState(connections);
        }

        void UpdateSessionState(Dictionary<string, string> connectionStrings)
        {
            Connections = connectionStrings;
        }

        public static JsonDataConnection CreateJsonDataConnectionFromString(string connectionName,
            string connectionString)
        {
            return new JsonDataConnection(connectionString) 
            { 
                StoreConnectionNameOnly = true,
                Name = connectionName
            };
        }
    }
public static class SessionExtensions {
    public static void SetObjectAsJson(this ISession session, string key, object value) {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T GetObjectFromJson<T>(this ISession session, string key) {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
}