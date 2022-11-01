using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos.TruckAttendance;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface ITruckAttendancesAppService
    {
        Task<LoadResult> GetAll(long dedicatedTruckId, string filter);
        Task CreateOrEdit(CreateOrEditTruckAttendanceDto input);
        Task<CreateOrEditTruckAttendanceDto> GetTruckAttendanceForEdit(long id);
        Task Delete(long id);
    }
}
