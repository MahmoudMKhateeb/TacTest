using System;

namespace TACHYON.Common
{
    public class DocumentUpload : IDocumentUpload
    {
        public string DocumentBase64 { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }

        public DocumentUpload()
        {

        }

        public DocumentUpload(string DocumentBase64,Guid? DocumentId, string DocumentName, string DocumentContentType)
        {
            this.DocumentBase64 = DocumentBase64;
            this.DocumentId = DocumentId;
            this.DocumentName = DocumentName;
            this.DocumentContentType = DocumentContentType;
        }
    }
}
