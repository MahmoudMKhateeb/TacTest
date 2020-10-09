using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Documents.DocumentsEntities
{
	[Table("DocumentsEntities")]
    public class DocumentsEntity : FullAuditedEntity 
    {

		[Required]
		[StringLength(DocumentsEntityConsts.MaxDisplayNameLength, MinimumLength = DocumentsEntityConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

    }
}