using System;

namespace TACHYON.Receivers.Dtos
{
    public class GetReceiverForViewDto
    {
        public ReceiverDto Receiver { get; set; }

        public string FacilityName { get; set; }

        public DateTime CreationTime { get; set; }


    }
}