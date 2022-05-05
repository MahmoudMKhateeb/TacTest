using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.UnitOfMeasures.Dtos
{
    public class GetAllUnitOfMeasureForDropDownOutput
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public bool IsOther { get; set; }
    }
}