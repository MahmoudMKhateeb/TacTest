using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;

namespace TACHYON.Penalties
{
   public interface IPenaltiesAppService : IApplicationService
    {
        Task<LoadResult> GetAll(LoadOptionsInput input);
    }
}
