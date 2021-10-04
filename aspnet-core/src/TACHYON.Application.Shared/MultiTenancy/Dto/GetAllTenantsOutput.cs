// unset

using TACHYON.Authorization.Users.Dto;

namespace TACHYON.MultiTenancy.Dto
{
    public class GetAllTenantsOutput
    {
        public TenantListDto TenantListDto { get; set; }
        public UserListDto UserListDto { get; set; }
    }
}