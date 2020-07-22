using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Organizations.Dto
{
    public class OrganizationUnitRoleListDto : EntityDto<long>
    {
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public DateTime AddedTime { get; set; }
    }
}