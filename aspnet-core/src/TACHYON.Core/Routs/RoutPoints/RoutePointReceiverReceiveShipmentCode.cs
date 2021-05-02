using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutePointReceiverReceiveShipmentCodes")]
    public class RoutePointReceiverReceiveShipmentCode : Entity
    {
        public long? PointId { get; set; }
        [ForeignKey(nameof(PointId))]
        public RoutPoint Point { get; set; }
        public string ReceiverPhone { get; set; }
        public RoutePointReceiverReceiveShipmentCode() { }

        public RoutePointReceiverReceiveShipmentCode(long pointId,string phone)
        {
            PointId = pointId;
            ReceiverPhone = phone;
        }

    }
}
