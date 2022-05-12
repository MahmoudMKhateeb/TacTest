using System.ComponentModel.DataAnnotations;
using TACHYON.Dto;

namespace TACHYON.EntityLogs.Dto
{
    public class GetAllEntityLogInput : PagedAndSortedInputDto
    {
        [Required] public EntityLogType EntityType { get; set; }
        [Required] public string EntityId { get; set; }
    }
}