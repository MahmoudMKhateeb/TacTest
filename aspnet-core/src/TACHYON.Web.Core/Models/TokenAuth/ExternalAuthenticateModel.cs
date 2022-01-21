using Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Web.Models.TokenAuth
{
    public class ExternalAuthenticateModel
    {
        [Required]
        [MaxLength(UserLogin.MaxLoginProviderLength)]
        public string AuthProvider { get; set; }

        [Required]
        [MaxLength(UserLogin.MaxProviderKeyLength)]
        public string ProviderKey { get; set; }

        [Required] public string ProviderAccessCode { get; set; }

        public string ReturnUrl { get; set; }

        public bool? SingleSignIn { get; set; }
    }
}