using Abp.Linq.Expressions;
using Castle.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TACHYON.Extension;
using TACHYON.Reports.ReportParameters;

namespace TACHYON.Reports.ExtensionMethods
{
    public static class ReportExtensions
    {
        public static IQueryable<TEntity> ApplyReportParameters<TEntity>(this IQueryable<TEntity> queryable, IEnumerable<ReportParameterItem<TEntity>> parameters)
        where TEntity : class
        {
            //  ToList() Used to avoid multiple enumeration
            var reportParameterItems = parameters.ToList();
            if (reportParameterItems.IsNullOrEmpty()) return queryable;

            var parameterExpressions = reportParameterItems
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => x.Expression)
                .ToList();

            if (parameterExpressions.Count == 0) return queryable;

            var combinedExpression = ExpressionCombiner.CombineExpressions(parameterExpressions.ToArray());
            
            return queryable.Where(combinedExpression);
            
        }


    }
}