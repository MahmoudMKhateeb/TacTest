using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.QueryBuilder.Services;
using System;

namespace TACHYON.Web.Services.ExceptionHandlers
{
    public class CustomQueryBuilderExceptionHandler : QueryBuilderExceptionHandler
    {

        // Note: There is a two methods not overrode yet `GetSqlQueryValidationExceptionMessage`, `GetExceptionMessage`
        // override it only if you need to do it
        public override string GetFaultExceptionMessage(FaultException ex)
        {
            throw ex;
        }

        public override string GetUnknownExceptionMessage(Exception ex)
        {
            throw ex;
        }

    }
}
