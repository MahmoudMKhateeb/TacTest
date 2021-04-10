using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Common
{
   public interface IHasDocument
    {
         Guid? DocumentId { get; set; }
         string DocumentName { get; set; }
         string DocumentContentType { get; set; }
    }
}
