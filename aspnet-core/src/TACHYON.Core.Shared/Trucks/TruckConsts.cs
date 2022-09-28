namespace TACHYON.Trucks
{
    public class TruckConsts
    {
        public const int MinPlateNumberLength = 0;
        public const int MaxPlateNumberLength = 64;

        public const string PlateNumberRegularExpression =
            @"^\d{4}\s[a-zA-Z\u0600-\u06FF]{1}\s[a-zA-Z\u0600-\u06FF]{1}\s[a-zA-Z\u0600-\u06FF]{1}$";

        public const int MinModelNameLength = 0;
        public const int MaxModelNameLength = 64;

        public const int MinModelYearLength = 0;
        public const int MaxModelYearLength = 64;
        public const string ModelYearRegularExpression = @"^\d{4}$";

        public const int MinLicenseNumberLength = 0;
        public const int MaxLicenseNumberLength = 256;

        public const int MinNoteLength = 0;
        public const int MaxNoteLength = 256;

        public const int MaxInternalTruckIdLength = 10;

    }
}