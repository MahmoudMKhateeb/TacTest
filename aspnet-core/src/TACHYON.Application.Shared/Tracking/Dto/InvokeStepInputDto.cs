using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using TACHYON.Common;
using TACHYON.Tracking.AdditionalSteps;

namespace TACHYON.Tracking.Dto
{
    public class InvokeStepInputDto : EntityDto<long>, ICustomValidate
    {
        public string Action { get; set; }

        public string Code { get; set; }

        public IHasDocument Document { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            switch (Action)
            {
                case AdditionalStepWorkflowActionConst.ReceiverConfirmation when string.IsNullOrEmpty(Code):
                    context.Results.Add(new ValidationResult("YouMustProvideACode"));
                    break;
                case AdditionalStepWorkflowActionConst.DeliveryConfirmation when Document is null:
                case AdditionalStepWorkflowActionConst.UploadEirFile when Document is null:
                case AdditionalStepWorkflowActionConst.UploadManifestFile when Document is null:
                case AdditionalStepWorkflowActionConst.UploadConfirmationDocument when Document is null:
                    context.Results.Add(new ValidationResult("YouMustUploadDocument"));
                    break;

                default:
                    context.Results.Add(new ValidationResult("NotSupportedAction"));
                    break;
            }


        }
    }
}