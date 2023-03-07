using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Common;
using TACHYON.Tracking.AdditionalSteps;

namespace TACHYON.Tracking.Dto
{
    public class InvokeStepInputDto : EntityDto<long>, ICustomValidate
    {
        public string Action { get; set; }

        public string Code { get; set; }

        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            switch (Action)
            {
                case AdditionalStepWorkflowActionConst.ReceiverConfirmation when string.IsNullOrEmpty(Code):
                    context.Results.Add(new ValidationResult("YouMustProvideACode"));
                    break;
                case AdditionalStepWorkflowActionConst.DeliveryConfirmation when DocumentId is null:
                case AdditionalStepWorkflowActionConst.UploadEirFile when DocumentId is null:
                case AdditionalStepWorkflowActionConst.UploadManifestFile when DocumentId is null:
                case AdditionalStepWorkflowActionConst.UploadConfirmationDocument when DocumentId is null:
                    context.Results.Add(new ValidationResult("YouMustUploadDocument"));
                    break;

                //default:
                //    context.Results.Add(new ValidationResult("NotSupportedAction"));
                //    break;
            }


        }
    }
}