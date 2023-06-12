using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;

namespace TACHYON.Reports.ReportPermissions
{
    [Table("ReportPermissions")]
    public class ReportPermission : Entity<Guid>, ISoftDelete
    {
        public int? RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
        
        public long? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        
        [Required]
        public Guid ReportId { get; set; }

        [ForeignKey(nameof(ReportId))]
        public Report Report { get; set; }

        public bool IsGranted { get; set; }
        public bool IsDeleted { get; set; }
    }
}