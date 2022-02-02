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
    }
}