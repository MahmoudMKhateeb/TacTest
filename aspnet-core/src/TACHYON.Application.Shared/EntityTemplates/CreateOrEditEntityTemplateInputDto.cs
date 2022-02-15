using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.Trips.Dto;

namespace TACHYON.EntityTemplates
{
    public class CreateOrEditEntityTemplateInputDto : EntityDto<long?>, ICustomValidate
    {
        [Required]
        public string TemplateName { get; set; }
        public string SavedEntity { get; set; }

        public string SavedEntityId { get; set; }

        public SavedEntityType EntityType { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (SavedEntity.IsNullOrEmpty() && SavedEntityId.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("YouMustSendEntityOrEntityId"));

            if (!SavedEntityId.IsNullOrEmpty() || SavedEntity.IsNullOrEmpty())
                return;

            var result = ValidateEntity();
            if (result != ValidationResult.Success) 
                context.Results.Add(result);
        }
        

        private ValidationResult ValidateEntity()

        {
            try
            {
                switch (EntityType)
                {
                    case SavedEntityType.ShippingRequest:
                        SavedEntityId = JsonConvert.DeserializeObject<CreateOrEditShippingRequestDto>(SavedEntity).Id.ToString();
                        break;
                    case SavedEntityType.Trip:
                        SavedEntityId = JsonConvert.DeserializeObject<CreateOrEditShippingRequestTemplateInputDto>(SavedEntity).Id.ToString();
                        break;
                    default:
                        return new ValidationResult("NotSupportedSavedEntityType");
                }
            }
            catch 
            {
                return new ValidationResult("Can't DeserializeObject SavedEntity");
            }
            return ValidationResult.Success;
        }
    }
}