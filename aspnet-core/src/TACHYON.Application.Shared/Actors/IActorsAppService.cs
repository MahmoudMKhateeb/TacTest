using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Actors.Dtos;
using TACHYON.Dto;

namespace TACHYON.Actors
{
    public interface IActorsAppService : IApplicationService
    {
        Task<PagedResultDto<GetActorForViewDto>> GetAll(GetAllActorsInput input);

        Task<GetActorForViewDto> GetActorForView(int id);

        Task<GetActorForEditOutput> GetActorForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditActorDto input);

        Task Delete(EntityDto input);

    }
}