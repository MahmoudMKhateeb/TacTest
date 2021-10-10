using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Common
{
    public static class OthersExpressions
    {
        public static Expression<Func<IHasDisplayName, bool>> ContainsOthersDisplayNameExpression = item => item.DisplayName.Trim().ToLower().Contains(TACHYONConsts.OthersDisplayName.Trim().ToLower());
        public static Expression<Func<IHasName, bool>> ContainsOthersNameExpression = item => item.Name.Trim().ToLower().Contains(TACHYONConsts.OthersDisplayName.Trim().ToLower());
        public static Expression<Func<IHasKey, bool>> ContainsOthersKeyExpression = item => item.Key.Trim().ToLower().Contains(TACHYONConsts.OthersDisplayName.Trim().ToLower());

    }
}