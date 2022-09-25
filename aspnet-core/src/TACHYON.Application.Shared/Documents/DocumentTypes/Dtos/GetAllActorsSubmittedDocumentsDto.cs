using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Documents.DocumentTypes.Dtos
{
    public class GetAllActorsSubmittedDocumentsDto
    {
        public Guid Id { get; set; }
        public string DocumentTypeName { get; set; }
        public int Actor { get; set; }
        public string ActorName { get; set; }
        public DateTime CreationTime { get; set; }
        public string Number { get; set; }
        public string Extn { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }
    }
}
