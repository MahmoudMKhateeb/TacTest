using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Editions.Dto
{
    public class CreateEditionDto
    {
        [Required] public EditionCreateDto Edition { get; set; }

        [Required] public List<NameValueDto> FeatureValues { get; set; }
    }
}