using Abp.Dependency;
using Abp.UI;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using TACHYON.Reports;

namespace TACHYON.Web.Services
{
    public class ReportsConnectionProviderService : IConnectionProviderService, ITransientDependency
    {


        public SqlDataConnection LoadConnection(string connectionName)
        {
            switch (connectionName)
            {
                case ReportConnectionNames.ReportDbConnectionName:
                    // todo get parameters from settings
                    var parameters = new MsSqlConnectionParameters("dev.tachyonhub.com",
                        "ReportsDB", "tachyontest", "tachyontest!@#", MsSqlAuthorizationType.SqlServer);
                    return new SqlDataConnection(ReportConnectionNames.ReportDbConnectionName, parameters);
                default: throw new UserFriendlyException($"The connection ${connectionName} is not defined");
            }
        }
    }
}
