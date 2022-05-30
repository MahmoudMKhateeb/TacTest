using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Penalties
{
    public class PenaltyComplaint : Entity
    {
        public int PenaltyId { get; set; }
        [ForeignKey(nameof(PenaltyId))]
        public Penalty PenaltyFK { get; set; }
        public string Description { get; set; }
        public string RejectReason { get; set; }
        public ComplaintStatus Status { get; set; } = ComplaintStatus.New;
    }
}
