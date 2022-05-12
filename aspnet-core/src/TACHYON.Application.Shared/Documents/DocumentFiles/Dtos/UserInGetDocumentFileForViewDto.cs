using Abp.Application.Services.Dto;
using Abp.Domain.Entities;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class UserInGetDocumentFileForViewDto : EntityDto<long>, IPassivable
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}