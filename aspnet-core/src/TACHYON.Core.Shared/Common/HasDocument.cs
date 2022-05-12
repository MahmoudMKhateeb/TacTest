using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Common
{
    public class HasDocument : IHasDocument
    {
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}