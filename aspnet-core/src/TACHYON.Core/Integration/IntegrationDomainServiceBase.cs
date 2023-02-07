using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Integration
{
    public class IntegrationDomainServiceBase : TACHYONDomainServiceBase
    {
        protected static string ToJsonLowerCaseFirstLetter(object root)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(root, new JsonSerializerSettings
            { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() });
            return body;
        }


        protected DateTime UtcToArabiaStandardTime(DateTime timeUtc)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Arabia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
        }


        /// <summary>
        /// <see href="https://en.m.wikipedia.org/wiki/Vehicle_registration_plates_of_Saudi_Arabia">HERE</see>
        /// </summary>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        protected string NormalizePlateNumber(string plateNumber)
        {
            var plate = plateNumber.ToUpper()
                .Replace("A", "ا")
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("B", "ب")
                .Replace("J", "ح")
                .Replace("D", "د")
                .Replace("R", "ر")
                .Replace("S", "س")
                .Replace("X", "ص")
                .Replace("T", "ط")
                .Replace("E", "ع")
                .Replace("G", "ق")
                .Replace("K", "ك")
                .Replace("L", "ل")
                .Replace("Z", "م")
                .Replace("N", "ن")
                .Replace("H", "ه")
                .Replace("U", "و")
                .Replace("V", "ى");

            plate = plate.Replace(" ", "");
            plate = plate.Replace("-", "");

            var numbers = String.Join("", plate.Take(4));
            var letters = String.Join("", plate.Skip(4).Take(3));
            plate = letters + numbers;


            plate = plate.Insert(1, " ");
            plate = plate.Insert(3, " ");
            plate = plate.Insert(5, " ");

            return plate;
        }
    }
}