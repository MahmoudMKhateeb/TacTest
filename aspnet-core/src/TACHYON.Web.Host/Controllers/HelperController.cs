using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetZeroCore.Net;
using Abp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TACHYON.Trucks;

namespace TACHYON.Web.Controllers
{
   
    public class HelperController : TACHYONControllerBase
    {
        private readonly ITrucksAppService _trucksAppService;

        public HelperController(ITrucksAppService trucksAppService)
        {
            _trucksAppService = trucksAppService;
        }

        public async Task<FileResult> GetTruckPictureByTruckId(Guid truckId)
        {
            var output = await _trucksAppService.GetPictureContentForTruck(truckId);
            if (output.IsNullOrEmpty())
            {
                return File(Convert.FromBase64String(""), MimeTypeNames.ImageJpeg);
            }

            return File(Convert.FromBase64String(output), MimeTypeNames.ImageJpeg);
        }

    }
}
