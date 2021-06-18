using System;

namespace TACHYON
{
    /// <summary>
    /// Some consts used in the application.
    /// </summary>
    public class AppConsts
    {
        #region Tachyon consts


        /// <summary>
        /// for documentType Required from entity
        /// </summary>
        //public const string TruckDocumentsEntityName = "Truck";
        //public const string TenantDocumentsEntityName = "Tenant";
        //public const string DriverDocumentsEntityName = "Driver";


        #endregion

        /// <summary>
        /// Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = 1000;

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

        public const int ResizedMaxProfilPictureBytesUserFriendlyValue = 1024;

        public const int MaxProfilPictureBytesUserFriendlyValue = 5;

        public const string TokenValidityKey = "token_validity_key";
        public const string SecurityStampKey = "AspNet.Identity.SecurityStamp";

        public const string TokenType = "token_type";

        public static string UserIdentifier = "user_identifier";

        public const string ThemeDefault = "default";
        public const string Theme2 = "theme2";
        public const string Theme3 = "theme3";
        public const string Theme4 = "theme4";
        public const string Theme5 = "theme5";
        public const string Theme6 = "theme6";
        public const string Theme7 = "theme7";
        public const string Theme8 = "theme8";
        public const string Theme9 = "theme9";
        public const string Theme10 = "theme10";
        public const string Theme11 = "theme11";

        public static TimeSpan AccessTokenExpiration = TimeSpan.FromDays(1);
        public static TimeSpan RefreshTokenExpiration = TimeSpan.FromDays(365);

        public const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:sszzz";

        public const string CarrierEditionName = "f3e9f12ed9d14bcda0a0dbd401b34e01";
        public const string TachyonEditionName = "4b4b7016956146c480a7fbb0d4a72554";
        public const string ShipperEditionName = "2d84ef198b5f433cac9fbe1690614585";
        public const int TachyonEditionId = 4;

       public class Message
        {
            public const string NotFoundRecord = "TheRecordNotFound";
        }
    }
}