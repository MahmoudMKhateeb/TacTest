using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace TACHYON.EntityLogs.Dto
{
    public class EntityLogListDto : EntityDto<Guid>
    {
        public string Transaction { get; set; }

        public DateTime ModificationTime { get; set; }

        // todo What Will Do If The Modifier is Host User

        public string ModifierUserName { get; set; }

        public long? ModifierUserId { get; set; }

        public int? ModifierTenantId { get; set; }

        public string ChangesData { get; set; }
    }
}