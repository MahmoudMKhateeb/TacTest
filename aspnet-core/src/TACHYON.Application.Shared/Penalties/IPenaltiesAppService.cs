using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Penalties.Dto;

namespace TACHYON.Penalties
{
   public interface IPenaltiesAppService : IApplicationService
    {
        Task<LoadResult> GetAll(LoadOptionsInput input);
        Task CreateOrEdit(CreateOrEditPenaltyDto input);
        Task<CreateOrEditPenaltyDto> GetPenaltyForEditDto(long Id);
        Task ConfirmPenalty(long id);
    }
}
