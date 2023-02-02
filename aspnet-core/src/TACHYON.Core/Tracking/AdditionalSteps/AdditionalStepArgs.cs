using Abp;
using Newtonsoft.Json;
using TACHYON.Common;

namespace TACHYON.Tracking.AdditionalSteps
{
    public class AdditionalStepArgs
    {
        public long PointId { get; set; }

        public string Code { get; set; }
        public IHasDocument Document { get; set; }

        [JsonIgnore]
        public UserIdentifier CurrentUser { get; set; }
    }
}