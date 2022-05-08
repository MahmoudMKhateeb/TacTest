using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Exporting;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;

namespace TACHYON.Shipping.Trips.RejectReasons
{
    [AbpAuthorize()]
    public class ShippingRequestTripRejectReasonAppService : TACHYONAppServiceBase,
        IShippingRequestTripRejectReasonAppService
    {
        private readonly IRepository<ShippingRequestTripRejectReason> _shippingRequestTripRejectReasonRepository;
        private readonly IExcelExporterManager<ShippingRequestTripRejectReasonListDto> _excelExporterManager;


        public ShippingRequestTripRejectReasonAppService(
            IRepository<ShippingRequestTripRejectReason> shippingRequestTripRejectReasonRepository,
            IExcelExporterManager<ShippingRequestTripRejectReasonListDto> excelExporterManager)
        {
            _shippingRequestTripRejectReasonRepository = shippingRequestTripRejectReasonRepository;
            _excelExporterManager = excelExporterManager;
        }

        public ListResultDto<ShippingRequestTripRejectReasonListDto> GetAllRejectReason(FilterInput Input)
        {
            return new ListResultDto<ShippingRequestTripRejectReasonListDto>(
                ObjectMapper.Map<List<ShippingRequestTripRejectReasonListDto>>(GetReason(Input)));
        }

        public async Task<CreateOrEditShippingRequestTripRejectReasonDto> GetForEdit(EntityDto input)
        {
            return ObjectMapper.Map<CreateOrEditShippingRequestTripRejectReasonDto>(await
                _shippingRequestTripRejectReasonRepository.GetAllIncluding(x => x.Translations)
                    .FirstOrDefaultAsync(e => e.Id == input.Id));
        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
            await ValidateDuplicateNameAsync(input);
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        private async Task ValidateDuplicateNameAsync(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
            foreach (var transItem in input.Translations)
            {
                if (string.IsNullOrWhiteSpace(transItem.Name))
                {
                    throw new UserFriendlyException(L("DisplayNameCannotBeEmpty"));
                }

                var isDuplicateUserName = await _shippingRequestTripRejectReasonRepository
                    .FirstOrDefaultAsync(x => x.Translations.Any(i => i.Name == transItem.Name) &&
                                              x.Id != input.Id);
                if (isDuplicateUserName != null)
                {
                    throw new UserFriendlyException(string.Format(L("TripRejectReasonDuplicateName"), transItem.Name));
                }
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _shippingRequestTripRejectReasonRepository.DeleteAsync(input.Id);
        }

        public FileDto Exports(FilterInput Input)
        {
            string[] HeaderText;
            Func<ShippingRequestTripRejectReasonListDto, object>[] propertySelectors;
            HeaderText = new string[] { "Id", "Name" };
            propertySelectors = new Func<ShippingRequestTripRejectReasonListDto, object>[] { _ => _.Id, _ => _.Name };

            var ReasonListDto = ObjectMapper.Map<List<ShippingRequestTripRejectReasonListDto>>(GetReason(Input));
            return _excelExporterManager.ExportToFile(ReasonListDto, "Reasons", HeaderText, propertySelectors);
        }

        #region Heleper

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Create)]
        private async Task Create(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
            var Reason = ObjectMapper.Map<ShippingRequestTripRejectReason>(input);

            await _shippingRequestTripRejectReasonRepository.InsertAsync(Reason);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Reject_Reason_Edit)]
        private async Task Update(CreateOrEditShippingRequestTripRejectReasonDto input)
        {
            var Reason = await _shippingRequestTripRejectReasonRepository.GetAllIncluding(x => x.Translations)
                .SingleAsync(e => e.Id == input.Id);
            Reason.Translations.Clear();
            ObjectMapper.Map(input, Reason);
        }

        private IQueryable<ShippingRequestTripRejectReason> GetReason(FilterInput Input)
        {
            return _shippingRequestTripRejectReasonRepository
                .GetAllIncluding(x => x.Translations)
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter),
                    e => e.Translations.Any(x => x.Name.Contains(Input.Filter.Trim())))
                .OrderBy(!string.IsNullOrEmpty(Input.Sorting) ? Input.Sorting : "id asc");
        }

        #endregion
    }
}