using Abp.Localization;

namespace TACHYON.Net.Sms
{
    public class SmsSenderBase
    {
        protected static ILocalizableString L(string message)
        {
            return new LocalizableString(message, TACHYONConsts.LocalizationSourceName);
        }

        protected string AddCountryCode(string number)
        {
            if (number.StartsWith("0"))

            {
                number = $"966{number.Remove(0, 1)}";
            }
            else if (!number.StartsWith("966"))
                number = $"966{number}";

            return number;
        }
    }
}