using System.Collections.Generic;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Drivers.importing.Dto
{
    public class ImportDriverDto
    {

        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        /// <summary>
        /// Can be set when reading data from excel or when importing user
        /// </summary>
        public string Exception { get; set; }

        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }

        public bool IsDriver { get; set; }

        public List<CreateOrEditDocumentFileDto> CreateOrEditDocumentFileDtos { get; set; }


    }
}