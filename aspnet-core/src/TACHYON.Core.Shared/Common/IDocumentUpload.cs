using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Common
{
   public interface IDocumentUpload: IHasDocument
    {
        string DocumentBase64 { get; set; }
    }
}
