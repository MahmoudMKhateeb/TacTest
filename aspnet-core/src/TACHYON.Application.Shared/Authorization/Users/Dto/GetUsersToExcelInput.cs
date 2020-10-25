using Abp.Runtime.Validation;
using System.Collections.Generic;

namespace TACHYON.Authorization.Users.Dto
{
    public class GetUsersToExcelInput : IShouldNormalize, IGetUsersInput
    {
        public string Filter { get; set; }

        public List<string> Permissions { get; set; }

        public int? Role { get; set; }

        public bool OnlyLockedUsers { get; set; }
        public bool OnlyDrivers { get; set; }

        public string Sorting { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Name,Surname";
            }
        }
    }
}