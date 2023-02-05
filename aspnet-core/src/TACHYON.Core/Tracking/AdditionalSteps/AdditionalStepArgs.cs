using Abp;
using Newtonsoft.Json;
using System;
using TACHYON.Common;

namespace TACHYON.Tracking.AdditionalSteps
{
    public class AdditionalStepArgs
    {
        public long PointId { get; set; }

        public string Code { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        [JsonIgnore]
        public UserIdentifier CurrentUser { get; set; }


    }
}