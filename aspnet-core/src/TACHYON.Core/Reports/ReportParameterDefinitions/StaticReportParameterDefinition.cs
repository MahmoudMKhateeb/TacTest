using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public class StaticReportParameterDefinition<TEntity> where TEntity : class
    {
        public string Name { get; set; }

        public Type Type { get; set; }
        public Func<Task<List<SelectItemDto>>> ListDataCallback { get; set; }

        public Func<ReportParameterExpressionCallbackArgs, Expression<Func<TEntity, bool>>> ExpressionCallback { get; set; } 

    }

    public class StaticReportParameterDefinition : StaticReportParameterDefinition<ShippingRequestTrip> { }
}