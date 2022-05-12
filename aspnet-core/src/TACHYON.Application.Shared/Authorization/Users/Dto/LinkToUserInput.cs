using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Authorization.Users.Dto
{
    public class LinkToUserInput
    {
        public string TenancyName { get; set; }

        [Required] public string UsernameOrEmailAddress { get; set; }

        [Required] [DisableAuditing] public string Password { get; set; }
    }
}