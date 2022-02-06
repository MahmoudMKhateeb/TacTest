using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Authorization.Users.Dto
{
    public class UserListDto : EntityDto<long>, IPassivable, IHasCreationTime
    {
        public int? TenantId { get; set; }
        public string AccountNumber { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public List<UserListRoleDto> Roles { get; set; }

        public bool IsActive { get; set; }
        public bool IsMissingDocumentFiles { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? DateOfBirth { get; set; }

    }

    public class DriverListDto : EntityDto<long>, IPassivable, IHasCreationTime
    {
        public string AccountNumber { get; set; }
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public bool IsEmailConfirmed { get; set; }
        
        public string CompanyName { get; set; }
        
        public string Nationality { get; set; }


        public bool IsActive { get; set; }
        public bool IsMissingDocumentFiles { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public decimal Rate { get; set; }

    }

}