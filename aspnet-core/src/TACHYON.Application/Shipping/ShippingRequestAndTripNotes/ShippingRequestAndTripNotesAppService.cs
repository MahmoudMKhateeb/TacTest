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
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Profile;
using TACHYON.Documents;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Features;
using TACHYON.Shipping.Notes;
using TACHYON.Shipping.Notes.Dto;
using TACHYON.Shipping.ShippingRequestAndTripNotes;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Shipping.ShippingRequests.ShippingRequestAndTripNotes

{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestAndTripNotes)]
    public class ShippingRequestAndTripNotesAppService : TACHYONAppServiceBase,IShippingRequestAndTripNotes
    {
        private readonly IRepository<ShippingRequestAndTripNote> _ShippingRequestAndTripNoteRepository;
        private readonly ProfileAppService _ProfileAppService;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly UserManager _userManager;


        public ShippingRequestAndTripNotesAppService(
            ProfileAppService ProfileAppService,
            DocumentFilesAppService documentFilesAppService,
            DocumentFilesManager documentFilesManager,
            UserManager userManager,
            IRepository<DocumentFile, Guid> documentFileRepository,
            IRepository<ShippingRequestAndTripNote> ShippingRequestAndTripNoteRepository
        )
        {
            _ShippingRequestAndTripNoteRepository = ShippingRequestAndTripNoteRepository;
            _ProfileAppService = ProfileAppService;
            _documentFilesAppService = documentFilesAppService;
            _documentFilesManager = documentFilesManager;
            _documentFileRepository = documentFileRepository;
            _userManager = userManager;
        }

        public async Task<GetAllShippingRequestAndTripNotesDto> GetShippingRequestNotes(GetAllNotesInput Input)
        {
            DisableTenancyFilters();
            var query = _ShippingRequestAndTripNoteRepository
                .GetAll()
                .Include(r => r.ShippingRequestFK)
                .Include(r => r.TenantFK)
                .Include(r => r.TripFK)
                .Include(r => r.DocumentFiles)
                .Where(x => Input.ShippingRequestId != null && x.ShippingRequetId == Input.ShippingRequestId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.TenantId == AbpSession.TenantId ||
                    (x.TenantId != AbpSession.TenantId && (x.Visibility == VisibilityNotes.ShipperOnly || x.Visibility == VisibilityNotes.Internal)))
                .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier),
                    x => ((x.ShippingRequestFK.CarrierTenantId == AbpSession.TenantId || x.ShippingRequestFK.CarrierTenantIdForDirectRequest == AbpSession.TenantId) &&
                    (x.Visibility == VisibilityNotes.Internal ||
                    x.Visibility == VisibilityNotes.CarrierOnly ||
                    x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                    (x.TenantId == AbpSession.TenantId)
                    )
              .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.TachyonDealer),
                    x => (x.ShippingRequestFK.IsTachyonDeal && 
                   (x.Visibility == VisibilityNotes.TMSOnly
                   || x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                   (x.TenantId == AbpSession.TenantId)
                   ).OrderByDescending(r=>r.Id);
              
            
            var totalCount = await query.CountAsync();
            var ResultPage = query.PageBy(Input);
            var output = ObjectMapper.Map<List<ShippingRequestAndTripNotesDto>>(await ResultPage.ToListAsync());
            output.ForEach(e =>
            {
                var TenantImage_date64 = _ProfileAppService.GetProfilePicture(e.TenantId).Result.ProfilePicture;
                var ModifiedUserImage_date64 = String.IsNullOrEmpty(e.LastModifierUserId.ToString()) ? _ProfileAppService.GetProfilePictureByUser(e.LastModifierUserId).Result.ProfilePicture : "";
                e.TenantImage = String.IsNullOrEmpty(TenantImage_date64) ? null : "data:image/jpeg;base64," + TenantImage_date64;
                e.LastModifierUserImage = String.IsNullOrEmpty(ModifiedUserImage_date64) ? null : "data:image/jpeg;base64," + ModifiedUserImage_date64;
                e.LastModifierUserName = e.LastModifierUserId > 0 ? _userManager.GetUserById(e.LastModifierUserId).FullName : "";

            });
            return new GetAllShippingRequestAndTripNotesDto()
            {
                Data = new PagedResultDto<ShippingRequestAndTripNotesDto>(
                    totalCount, output
                )
            };
        }

        public async Task<GetAllShippingRequestAndTripNotesDto> GetTripNotes(GetAllNotesInput Input)
        {
            DisableTenancyFilters();
            var query = _ShippingRequestAndTripNoteRepository
                .GetAll()
                .Include(r => r.ShippingRequestFK)
                .Include(r => r.TenantFK)
                .Include(r => r.TripFK)
                .ThenInclude(r => r.ShippingRequestFk)
                .Include(r => r.DocumentFiles)
                .Where(x => x.TripId != null && x.TripId == Input.TripId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.TenantId == AbpSession.TenantId ||
                    (x.TenantId != AbpSession.TenantId && (x.Visibility == VisibilityNotes.ShipperOnly || x.Visibility == VisibilityNotes.Internal)))
                .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier),
                    x => ((x.TripFK.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId || x.TripFK.ShippingRequestFk.CarrierTenantIdForDirectRequest == AbpSession.TenantId) &&
                    (x.Visibility == VisibilityNotes.Internal ||
                    x.Visibility == VisibilityNotes.CarrierOnly ||
                    x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                    (x.TenantId == AbpSession.TenantId)
                    )
              .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.TachyonDealer),
                    x => (x.TripFK.ShippingRequestFk.IsTachyonDeal &&
                   (x.Visibility == VisibilityNotes.TMSOnly
                   || x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                   (x.TenantId == AbpSession.TenantId)
                   ).OrderByDescending(r => r.Id);


            var totalCount = await query.CountAsync();
            var ResultPage = query.PageBy(Input);
            var output = ObjectMapper.Map<List<ShippingRequestAndTripNotesDto>>(await ResultPage.ToListAsync());
            output.ForEach(e =>
            {
                var TenantImage_date64 = _ProfileAppService.GetProfilePicture(e.TenantId).Result.ProfilePicture;
                var ModifiedUserImage_date64 = String.IsNullOrEmpty(e.LastModifierUserId.ToString()) ? _ProfileAppService.GetProfilePictureByUser(e.LastModifierUserId).Result.ProfilePicture : "";
                e.TenantImage = String.IsNullOrEmpty(TenantImage_date64) ? null : "data:image/jpeg;base64," + TenantImage_date64;
                e.LastModifierUserImage = String.IsNullOrEmpty(ModifiedUserImage_date64) ? null : "data:image/jpeg;base64," + ModifiedUserImage_date64;
                e.LastModifierUserName = e.LastModifierUserId > 0 ? _userManager.GetUserById(e.LastModifierUserId).FullName : "";

            });
            return new GetAllShippingRequestAndTripNotesDto()
            {
                Data = new PagedResultDto<ShippingRequestAndTripNotesDto>(
                    totalCount, output
                )
            };
        }
        public async Task<CreateOrEditShippingRequestAndTripNotesDto> GetForEdit(EntityDto input)
        {
            DisableTenancyFilters();
            var query = await _ShippingRequestAndTripNoteRepository
                .GetAll()
                .Include(r => r.DocumentFiles)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            return ObjectMapper.Map<CreateOrEditShippingRequestAndTripNotesDto>(query);
        }

        public async Task CreateOrEdit(CreateOrEditShippingRequestAndTripNotesDto input)
        {
            DisableTenancyFilters();

            ValidateNote(input);

            if (input.NoteId == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestAndTripNotes_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _ShippingRequestAndTripNoteRepository.DeleteAsync((int)input.Id);
        }

        #region Heleper

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestAndTripNotes_Create)]
        private async Task Create(CreateOrEditShippingRequestAndTripNotesDto input)
        {
            DisableTenancyFilters();

            var Note = ObjectMapper.Map<ShippingRequestAndTripNote>(input);

            var NoteId = await _ShippingRequestAndTripNoteRepository.InsertAndGetIdAsync(Note);
            //add document file
            var docFileDto = input.CreateOrEditDocumentFileDto;
            if (docFileDto != null && docFileDto.Count() > 0)
            {
                foreach (var f in docFileDto)
                {
                    if (String.IsNullOrEmpty(f.UpdateDocumentFileInput?.FileToken))
                        throw new UserFriendlyException(L("FileTokenCanNotBeEmpty"));
                    f.NoteId = NoteId;
                    f.Name = f.Name + "_" + NoteId;
                    await _documentFilesAppService.CreateOrEdit(f);
                }
            }

        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestAndTripNotes_Edit)]
        private async Task Update(CreateOrEditShippingRequestAndTripNotesDto input)
        {
            var Note = await GetNote(input.NoteId);
            ObjectMapper.Map(input, Note);

            var docFileDto = input.CreateOrEditDocumentFileDto;
            if (docFileDto != null && docFileDto.Count() > 0)
            {
                foreach (var f in docFileDto)
                {
                    if (!String.IsNullOrEmpty(f.UpdateDocumentFileInput?.FileToken))
                    {
                        f.NoteId = input.NoteId;
                        f.Name = f.Name + "_" + input.NoteId;
                        await _documentFilesAppService.CreateOrEdit(f);
                    }
                }
            }
            else
            {
                var documentFiles = await _documentFileRepository
                                    .GetAll()
                                    .Where(r=>r.NoteId == input.NoteId)
                                    .ToListAsync();
                foreach (var f in documentFiles)
                {
                    await _documentFilesManager.DeleteDocumentFile(f);
                }
            }
        }

        private async Task<ShippingRequestAndTripNote> GetNote(int Id)
        {
            var Note = await _ShippingRequestAndTripNoteRepository
                .GetAll()
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync();
            if (Note == null) throw new UserFriendlyException(L("NoRecoredFound"));

            return Note;
        }


        private void ValidateNote(CreateOrEditShippingRequestAndTripNotesDto input)
        {
            if (String.IsNullOrEmpty(input.Note))
                throw new UserFriendlyException(L("NoteCanNotBeEmpty"));
        }

        #endregion
    }
}