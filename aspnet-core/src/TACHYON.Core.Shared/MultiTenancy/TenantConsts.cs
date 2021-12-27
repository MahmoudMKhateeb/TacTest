namespace TACHYON.MultiTenancy
{
    public class TenantConsts
    {
        public const string TenancyNameRegex = "^[a-zA-Z][ a-zA-Z0-9_-]{1,}$";
        public const string TenancyLegalNameRegex = @"^[\u0600-\u06FFa-zA-Z\d\-_\s]+$";

        public const string DefaultTenantName = "Default";

        public const int MaxNameLength = 128;

        public const int DefaultTenantId = 1;

        public const string MoiNumberRegex = @"^7\d{9}$";
    }
}