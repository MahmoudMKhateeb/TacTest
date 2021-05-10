using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Importing
{
    public interface IExcelImportManager<TEntity>
    {
        List<TEntity> ImportFromFile(byte[] fileBytes);
    }
}
