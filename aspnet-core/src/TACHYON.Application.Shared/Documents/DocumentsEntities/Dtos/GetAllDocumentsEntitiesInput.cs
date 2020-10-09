using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Documents.DocumentsEntities.Dtos
{
    public class GetAllDocumentsEntitiesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }



    }
}