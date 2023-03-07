using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class TripManifestDataDto
    {
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}
