using System;
using System.Linq.Expressions;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Reports.ReportParameters
{
    public class ReportParameterItem<TEntity> where TEntity : class
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public Type Type { get; set; }

        public Expression<Func<TEntity,bool>> Expression { get; set; }
    }
    public class ReportParameterItem : ReportParameterItem<ShippingRequestTrip>{}
}