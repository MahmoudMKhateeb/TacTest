using Abp.Extensions;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.Dtos
{
    public class UpdateTruckPictureInput : ICustomValidate
    {
        [MaxLength(400)]
        public string FileToken { get; set; }

        //public Guid TruckId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (FileToken.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(FileToken));
            }
        }
    }
}