using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace TACHYON.CustomValidation
{
    public class UploadBase64File : ValidationAttribute
    {
        /// <summary>
        /// Maxsize is the maximim file zie by byte, If zero value to accept any size,default value 0
        /// </summary>
        public double MaxLength = 0;

        /// <summary>
        /// Array of extension
        /// Example=new string[] {"jpeg","png","jpg"}
        /// </summary>
        public string[] AllowedExtensions;


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                var fileBytes = Convert.FromBase64String(value.ToString().Split(',')[1]);
                var ext = value.ToString().Split(';')[0].Split('/')[1].ToLower();

                if (MaxLength > 0)
                {
                    if (fileBytes.Length > MaxLength)
                        return new ValidationResult("The attached file is larger than the specified space");
                }
                else if (AllowedExtensions != null && !AllowedExtensions.Contains(ext))
                {
                    return new ValidationResult("The attachment file extension is not allowed.");
                }
            }

            return ValidationResult.Success;
        }
    }
}