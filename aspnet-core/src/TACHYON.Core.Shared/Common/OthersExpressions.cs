using System;
using System.Linq.Expressions;

namespace TACHYON.Common
{
    public static class OthersExpressions
    {
        public static Expression<Func<IHasKey, bool>> ContainsOthersKeyExpression = item =>
            item.Key.Trim().ToLower().Contains(TACHYONConsts.OthersDisplayName.Trim().ToLower());
    }
}