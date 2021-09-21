using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Authorization.Users.Profile.Dto
{
    public class UpdateTenantProfileInformationInputDto : GetTenantProfileInformationForEditDto, ICustomValidate
    {
        [Required]
        public int TenantId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            // todo Add Validation Here

        }
    }
}