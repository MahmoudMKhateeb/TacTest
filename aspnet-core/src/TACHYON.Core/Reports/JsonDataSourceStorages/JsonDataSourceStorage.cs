using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Reports.JsonDataSourceStorages
{
    [Table("JsonDataSourceStorages")]
    public class JsonDataSourceStorage : Entity<long>
    {
        public string ConnectionName { get; set; }

        public string ConnectionValue { get; set; }
    }
}
