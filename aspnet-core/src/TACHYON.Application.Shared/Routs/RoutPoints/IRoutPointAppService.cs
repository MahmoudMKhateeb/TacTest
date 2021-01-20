using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Routs.RoutPoints
{
    public interface IRoutPointAppService: IApplicationService
    {
        Task<PagedResultDto<GetRoutPointForViewDto>> GetAll(GetAllRoutPointInput input);

    }
}
