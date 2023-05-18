using DevExpress.DataAccess.Web;
using DevExpress.DataAccess.Wizard.Services;

namespace TACHYON.Web.Services
{
    public class ReportsConnectionProviderFactory : IConnectionProviderFactory
    {
        public IConnectionProviderService Create()
        {
            return new ReportsConnectionProviderService();
        }
    }
}
