using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Dto;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllUploadedFileDto
    {
        public Guid DocumentId { get; set; }

        [Required]
        public string FileName { get; set; }
        public string FileType { get; set; }
        [Required]
        public string ThumbnailImage { get; set; }

    }
}