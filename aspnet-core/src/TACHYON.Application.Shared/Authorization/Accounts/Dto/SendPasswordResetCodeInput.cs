using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Authorization.Accounts.Dto
{
    public class SendPasswordResetCodeInput
    {
        [Required]
        [MaxLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }
    }
}