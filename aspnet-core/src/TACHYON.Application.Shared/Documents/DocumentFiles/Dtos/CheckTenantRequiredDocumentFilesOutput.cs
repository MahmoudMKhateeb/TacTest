namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class CheckTenantRequiredDocumentFilesOutput
    {
        public bool HasMissingDocuments { get; set; }
        public int MissingDocumentFilesCount { get; set; }
    }
}