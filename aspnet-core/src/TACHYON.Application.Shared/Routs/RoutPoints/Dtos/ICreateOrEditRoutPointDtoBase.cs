using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public interface ICreateOrEditRoutPointDtoBase
    {
         PickingType PickingType { get; set; }
        [Required] long FacilityId { get; set; }
        int? ReceiverId { get; set; }
        string Code { get; set; }


    }
}
