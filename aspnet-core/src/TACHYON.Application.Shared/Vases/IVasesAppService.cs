﻿using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using TACHYON.Vases.Dtos;
using TACHYON.Dto;

namespace TACHYON.Vases
{
    public interface IVasesAppService : IApplicationService
    {
        Task<PagedResultDto<GetVasForViewDto>> GetAll(GetAllVasesInput input);

        Task<GetVasForViewDto> GetVasForView(int id);

        Task<GetVasForEditOutput> GetVasForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditVasDto input);

        Task Delete(EntityDto input);

        Task<FileDto> GetVasesToExcel(GetAllVasesForExcelInput input);

    }
}