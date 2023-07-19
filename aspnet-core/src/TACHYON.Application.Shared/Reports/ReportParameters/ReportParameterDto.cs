using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Reports.ReportParameters
{
    public class ReportParameterDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }
    }
}