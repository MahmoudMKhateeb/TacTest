using Abp.Application.Services.Dto;
using Abp.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TACHYON.Timing
{
    public interface ITimeZoneService
    {
        Task<string> GetDefaultTimezoneAsync(SettingScopes scope, int? tenantId);

        TimeZoneInfo FindTimeZoneById(string timezoneId);

        List<NameValueDto> GetWindowsTimezones();
    }
}