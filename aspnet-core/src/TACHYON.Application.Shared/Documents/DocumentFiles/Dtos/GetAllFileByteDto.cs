using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Documents.DocumentFiles.Dtos
{
    public class GetAllFileByteDto
    {
        public string Token { get; set; }
        public Byte[] File { get; set; }
    }
}