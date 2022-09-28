using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TACHYON.Dto;

namespace TACHYON.ServiceAreas
{
    public class CreateServiceAreaDto : EntityDto<string>, ICustomValidate
    {
        [JsonIgnore]
        public int CityId { get; set; }

        [Required]
        [StringLength(100,MinimumLength = 1)]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public bool? IsOther { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (int.TryParse(Id, out int result))
            {
                CityId = result;
                return;
            }

            context.Results.Add(new ValidationResult("Id is not a valid format"));
        }
    }
}