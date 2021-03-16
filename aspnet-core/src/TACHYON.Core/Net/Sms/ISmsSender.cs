using System;
using System.Threading.Tasks;

namespace TACHYON.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);

        Task SendReceiverSmsAsync(string number, DateTime date, string shipperName, string driverName, string driverPhone, string waybillNumber, string code, string link);

    }
}