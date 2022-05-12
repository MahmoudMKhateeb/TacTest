using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Routs.RoutPoints
{
    [Table("RoutPointDocuments")]
    public class RoutPointDocument : FullAuditedEntity<long>
    {
        public long RoutPointId { get; set; }
        [ForeignKey("RoutPointId")] public RoutPoint RoutPointFk { get; set; }
        public RoutePointDocumentType RoutePointDocumentType { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}