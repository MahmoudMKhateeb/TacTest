using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Routs.RoutPoints
{
    public enum RoutePointDocumentType : byte
    {
        POD = 1,
        DeliveryNote = 2,
        DeliveryGood = 3,
        Appointment = 4,
        Manifest = 5,
        ConfirmationDocuments = 6,
        Eir = 7,
    }
}