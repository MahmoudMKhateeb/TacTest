using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class DropAppointmentDataDto
    {
        public DateTime? AppointmentDateTime { get; set; }
        public string AppointmentNumber { get; set; }
        public string AppointmentFileName { get; set; }
        public Guid? AppointmentDocumentId { get; set; }
        public string AppointmentDocumentContentType { get; set; }
    }
}
