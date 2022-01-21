using Abp.Web.Models;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class UploadDocumentFileOutput : ErrorInfo
    {
        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileToken { get; set; }

        public UploadDocumentFileOutput()
        {
        }

        public UploadDocumentFileOutput(ErrorInfo error)
        {
            Code = error.Code;
            Details = error.Details;
            Message = error.Message;
            ValidationErrors = error.ValidationErrors;
        }
    }
}