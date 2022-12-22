using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PricePackages.Dto.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageAppendices.Jobs;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    public class PricePackageAppendixManager : TACHYONDomainServiceBase, IPricePackageAppendixManager
    {
        private readonly IRepository<PricePackageAppendix> _appendixRepository;
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<PricePackageProposal> _proposalRepository;
        private readonly IBackgroundJobManager _jobManager;
        private readonly IRepository<NormalPricePackage> _normalPricePackage;

        public PricePackageAppendixManager(
            IRepository<PricePackageAppendix> appendixRepository,
            IRepository<BinaryObject, Guid> binaryObjectRepository,
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<PricePackageProposal> proposalRepository,
            IBackgroundJobManager jobManager,
            IRepository<NormalPricePackage> normalPricePackage)
        {
            _appendixRepository = appendixRepository;
            _binaryObjectRepository = binaryObjectRepository;
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _proposalRepository = proposalRepository;
            _jobManager = jobManager;
            _normalPricePackage = normalPricePackage;
        }

        public async Task CreateAppendix(PricePackageAppendix createdAppendix, List<string> pricePackages,string emailAddress)
        {
            DisableTenancyFilters();

            var tenantId = createdAppendix.DestinationTenantId;

            if (tenantId == null && createdAppendix.ProposalId.HasValue)
            {
               tenantId = await _proposalRepository.GetAll().Where(x => x.Id == createdAppendix.ProposalId).Select(x => x.ShipperId)
                    .FirstOrDefaultAsync();
            }
            var lastCreatedAppendix = await _appendixRepository.GetAll()
                .Where(x=> (!x.DestinationTenantId.HasValue || x.DestinationTenantId == tenantId) 
                           || (!x.ProposalId.HasValue || x.Proposal.ShipperId == tenantId))
                .OrderByDescending(x => x.MajorVersion).ThenByDescending(x=> x.MinorVersion).FirstOrDefaultAsync();

            #region Handle Versioning

            if (lastCreatedAppendix == null)
            {
                createdAppendix.MajorVersion = 1;
                createdAppendix.MinorVersion = 1;
            }
            else if (lastCreatedAppendix.MinorVersion == 9)
            {
                createdAppendix.MajorVersion = lastCreatedAppendix.MajorVersion + 1;
                createdAppendix.MinorVersion = 1;
            }
            else
            {
                createdAppendix.MajorVersion = lastCreatedAppendix.MajorVersion;
                createdAppendix.MinorVersion = lastCreatedAppendix.MinorVersion + 1;
            }

            #endregion

            var appendixId = await _appendixRepository.InsertAndGetIdAsync(createdAppendix);
            
            if (createdAppendix.ProposalId.HasValue)
                _proposalRepository.Update(createdAppendix.ProposalId.Value, x => x.AppendixId = appendixId);

            // this step for carrier appendix
            
            if (!createdAppendix.ProposalId.HasValue && !pricePackages.IsNullOrEmpty())
            {
                var tmsPricePackages = await _tmsPricePackageRepository.GetAll().Where(x => pricePackages.Contains(x.PricePackageId))
                    .Select(x=> x.Id).ToListAsync();
                var normalPricePackages = await _normalPricePackage.GetAll().Where(x => pricePackages.Contains(x.PricePackageId))
                    .Select(x=> x.Id).ToListAsync();
                
                foreach (var pricePackageId in normalPricePackages)
                    _normalPricePackage.Update(pricePackageId, x => x.AppendixId = appendixId);
                foreach (var pricePackageId in tmsPricePackages)
                    _tmsPricePackageRepository.Update(pricePackageId, x => x.AppendixId = appendixId);
                
            }
              // todo handle normal price package in file 
            await _jobManager.EnqueueAsync<GenerateAppendixFileJob, GenerateAppendixFileJobArgument>(
                new GenerateAppendixFileJobArgument
                {
                    AppendixId = appendixId, FileReceiverEmailAddress = emailAddress
                });
        }
        
        public async Task UpdateAppendix(CreateOrEditAppendixDto updatedAppendix, List<string> pricePackages, string emailAddress)
        { 
            
            if (!updatedAppendix.Id.HasValue) return;
            
            DisableTenancyFilters();
            
            var oldAppendix = await _appendixRepository.GetAll()
                .Include(x=> x.TmsPricePackages).Include(x=> x.NormalPricePackages)
                .Include(x=> x.Proposal).SingleAsync(x=> x.Id == updatedAppendix.Id);

            if (oldAppendix.Status == AppendixStatus.Confirmed)
                throw new UserFriendlyException(L("YouCanNotUpdateConfirmedAppendix"));
            
            var oldProposal = oldAppendix.ProposalId;
            ObjectMapper.Map(updatedAppendix,oldAppendix);
            var newProposal = updatedAppendix.ProposalId;
            if (!newProposal.HasValue && oldProposal.HasValue) throw new UserFriendlyException(L("YouMustSelectAProposal"));

            if (oldProposal != newProposal)
            {
                _proposalRepository.Update(newProposal.Value, x => x.AppendixId = updatedAppendix.Id);
                if (oldProposal.HasValue)
                    _proposalRepository.Update(oldProposal.Value, x => x.AppendixId = null);
            }
                

            // this step for carrier appendix
            
            if (!updatedAppendix.ProposalId.HasValue && !pricePackages.IsNullOrEmpty())
            {

                #region Tms price packages created/deleted Handling

                var oldTmsPricePackages = oldAppendix.TmsPricePackages.Select(x => x.Id).ToList();
                
                var tmsPricePackages = await _tmsPricePackageRepository.GetAll().Where(x => pricePackages.Contains(x.PricePackageId))
                    .Select(x=> x.Id).ToListAsync();

                var addedTmsPricePackages = tmsPricePackages.Where(x => oldTmsPricePackages.All(o => o != x));
                var deletedTmsPricePackages = oldTmsPricePackages.Where(x => tmsPricePackages.All(o => o != x));

                foreach (var addedItemId in addedTmsPricePackages)
                    _tmsPricePackageRepository.Update(addedItemId, x => x.AppendixId = updatedAppendix.Id);
                
                
                foreach (var deletedItemId in deletedTmsPricePackages)
                    _tmsPricePackageRepository.Update(deletedItemId, x => x.AppendixId = null);
                
                #endregion

                #region Normal price packages created/deleted Handling

                var oldNormalPricePackages = oldAppendix.NormalPricePackages.Select(x => x.Id).ToList();

                var normalPricePackages = await _normalPricePackage.GetAll().Where(x => pricePackages.Contains(x.PricePackageId))
                    .Select(x=> x.Id).ToListAsync();
                
                var addedNormalPricePackages = normalPricePackages.Where(x => oldNormalPricePackages.All(o => o != x));
                var deletedNormalPricePackages = oldNormalPricePackages.Where(x => normalPricePackages.All(o => o != x));

                foreach (var addedItemId in addedNormalPricePackages)
                    _normalPricePackage.Update(addedItemId, x => x.AppendixId = updatedAppendix.Id);
                
                
                foreach (var deletedItemId in deletedNormalPricePackages)
                    _normalPricePackage.Update(deletedItemId, x => x.AppendixId = null);
                
                #endregion

            }
             
            await _jobManager.EnqueueAsync<GenerateAppendixFileJob, GenerateAppendixFileJobArgument>(
                new GenerateAppendixFileJobArgument
                {
                    AppendixId = updatedAppendix.Id.Value, FileReceiverEmailAddress = emailAddress
                });
        }
        
        public async Task<BinaryObject> GenerateAppendixFile(int appendixId)
        {
            DisableTenancyFilters();
            
            var appendix = await _appendixRepository.GetAll()
                .Include(x => x.Proposal).ThenInclude(x => x.TmsPricePackages).ThenInclude(x=> x.TransportTypeFk)
                .Include(x => x.Proposal).ThenInclude(x => x.TmsPricePackages).ThenInclude(x=> x.TrucksTypeFk)
                .Include(x => x.Proposal).ThenInclude(x => x.TmsPricePackages).ThenInclude(x=> x.OriginCity)
                .Include(x => x.Proposal).ThenInclude(x => x.TmsPricePackages).ThenInclude(x=> x.DestinationCity)
                .Include(x => x.Proposal).ThenInclude(x => x.Shipper)
                .Include(x=> x.NormalPricePackages).ThenInclude(x=> x.TransportTypeFk)
                .Include(x=> x.NormalPricePackages).ThenInclude(x=> x.TrucksTypeFk)                
                .Include(x=> x.NormalPricePackages).ThenInclude(x=> x.OriginCityFK)
                .Include(x=> x.NormalPricePackages).ThenInclude(x=> x.DestinationCityFK)
                .Include(x=> x.TmsPricePackages).ThenInclude(x=> x.TransportTypeFk)
                .Include(x=> x.TmsPricePackages).ThenInclude(x=> x.TrucksTypeFk)
                .Include(x=> x.TmsPricePackages).ThenInclude(x=> x.OriginCity)
                .Include(x=> x.TmsPricePackages).ThenInclude(x=> x.DestinationCity)
                .Include(x=> x.DestinationTenant)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == appendixId);

            if (appendix is null) throw new EntityNotFoundException(L("AppendixNotFound"));
            
            await using var stream = GetResourceStream(TACHYONConsts.AppendixTemplateFullNamespace);
            using var documentProcessor = new RichEditDocumentServer();
            await documentProcessor.LoadDocumentAsync(stream);
            var document = documentProcessor.Document;

            List<AppendixTableItem> items;
            string[] truckTypes, transportTypes, routeTypes;
            
            // check if this shipper appendix
            if (appendix.ProposalId.HasValue)
            {
                items = appendix.Proposal?.TmsPricePackages?.Select(x => new AppendixTableItem()
                {
                    Origin = x.OriginCity?.DisplayName,
                    Destination = x.DestinationCity?.DisplayName,
                    Price = x.TotalPrice,
                    TruckType = x.TrucksTypeFk?.DisplayName
                }).ToList();
                
                truckTypes = appendix.Proposal?.TmsPricePackages?
                    .Select(x => x.TrucksTypeFk?.DisplayName)?.Distinct()
                    .ToArray();
            
                transportTypes = appendix.Proposal?.TmsPricePackages?
                    .Select(x => x.TransportTypeFk?.DisplayName)?.Distinct()
                    .ToArray();
            
                routeTypes = appendix.Proposal?.TmsPricePackages?
                    .Select(x => Enum.GetName(typeof(ShippingRequestRouteType),x.RouteType))?.Distinct()
                    .ToArray();
                
            }
             // check if this carrier appendix
            else if (appendix.DestinationTenantId.HasValue && !appendix.ProposalId.HasValue)
            {
                
                #region Truck Types

                var tmsTruckTypes = appendix.TmsPricePackages?
                    .Select(x => x.TrucksTypeFk?.DisplayName).ToList();
                
                var normalTruckTypes = appendix.NormalPricePackages?
                    .Select(x => x.TrucksTypeFk?.DisplayName).ToList();
                if (normalTruckTypes != null && normalTruckTypes.Count > 0)
                    tmsTruckTypes?.AddRange(normalTruckTypes);
                truckTypes = tmsTruckTypes?.Distinct().ToArray();

                    #endregion

                #region Transport Types

                    var tmsTransportTypes = appendix.TmsPricePackages?
                        .Select(x => x.TransportTypeFk?.DisplayName).ToList();
                
                    var normalTransportTypes = appendix.NormalPricePackages?
                        .Select(x => x.TransportTypeFk?.DisplayName).ToList();
                    if (normalTransportTypes != null && normalTransportTypes.Count > 0)
                        tmsTransportTypes?.AddRange(normalTransportTypes);

                    transportTypes = tmsTransportTypes?.Distinct().ToArray();

                    #endregion

                #region Route Types

                    var tmsRouteTypes = appendix.TmsPricePackages?
                        .Select(x => Enum.GetName(typeof(ShippingRequestRouteType),x.RouteType)).ToList();

                    var normalTRouteTypes = appendix.NormalPricePackages?
                        .Select(x => Enum.GetName(typeof(ShippingRequestRouteType),
                            x.IsMultiDrop
                                ? ShippingRequestRouteType.MultipleDrops
                                : ShippingRequestRouteType.SingleDrop)).ToList();
                    if (normalTRouteTypes != null && normalTRouteTypes.Count > 0)
                        tmsRouteTypes?.AddRange(normalTRouteTypes);

                    routeTypes = tmsRouteTypes?.Distinct().ToArray();

                #endregion
                    
                items = appendix.TmsPricePackages?.Select(x => new AppendixTableItem()
                {
                    Origin = x.OriginCity?.DisplayName,
                    Destination = x.DestinationCity?.DisplayName,
                    Price = x.TotalPrice,
                    TruckType = x.TrucksTypeFk?.DisplayName
                }).ToList();

                var normalPricePackage = appendix.NormalPricePackages?.Select(x => new AppendixTableItem()
                {
                    Origin = x.OriginCityFK?.DisplayName,
                    Destination = x.DestinationCityFK?.DisplayName,
                    Price = x.TachyonMSRequestPrice,
                    TruckType = x.TrucksTypeFk?.DisplayName
                }).ToList();

                if (normalPricePackage != null && normalPricePackage.Count > 0)
                    items?.AddRange(normalPricePackage);
            }
            else throw new UserFriendlyException(L("AppendixMustHaveDestinationCompanyOrProposal"));
             
            var companyName = appendix.ProposalId.HasValue ? appendix.Proposal?.Shipper?.companyName : appendix.DestinationTenant?.companyName;
            string contractNumber = appendix.ProposalId.HasValue ? appendix.Proposal?.Shipper?.ContractNumber : appendix.DestinationTenant?.ContractNumber;
            var appendixDate = ClockProviders.Local.Normalize(appendix.AppendixDate).ToString("dd/MM/yyyy");
            var contractDate = appendix.ProposalId.HasValue ? appendix.Proposal?.Shipper?.CreationTime : appendix.DestinationTenant?.CreationTime;
            if (!contractDate.HasValue) throw new UserFriendlyException(L("CanNotFindContractDate"));
            
            
            string formattedContractDate = ClockProviders.Local.Normalize(contractDate.Value).ToString("dd/MM/yyyy");

            string formattedScopeOverview = appendix.ScopeOverview.Replace(TACHYONConsts.AppendixTemplateClientName, companyName);
            
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractNumber, contractNumber,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateAppendixDate, appendixDate ,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractDate, formattedContractDate,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractNumber, $"{appendix?.Proposal?.Shipper?.ContractNumber}",SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateClientName, companyName,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateScopeOverview, formattedScopeOverview,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateNotes, appendix.Notes,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateVersion, $"{appendix.MajorVersion}.{appendix.MinorVersion}",SearchOptions.None);
            if (truckTypes != null && truckTypes.Length > 0) 
                document.ReplaceAll(TACHYONConsts.AppendixTemplateTruckTypes, string.Join(',',truckTypes),SearchOptions.None);
            
            if (transportTypes != null && transportTypes.Length > 0) 
                document.ReplaceAll(TACHYONConsts.AppendixTemplateTransportTypes, string.Join(',',transportTypes),SearchOptions.None);
            
            if (routeTypes != null && routeTypes.Length > 0) 
                document.ReplaceAll(TACHYONConsts.AppendixTemplateRouteTypes, string.Join(',',routeTypes),SearchOptions.None);

            if (items == null || items.Count < 1) throw new UserFriendlyException(L("AppendixMustHavePricePackages"));

            var pricingDetailsTable = document.Tables[2];
            for (int i = 0; i < items?.Count; i++)
            {
                int rowLength = pricingDetailsTable.Rows.Count;
                if (i > 0) pricingDetailsTable.Rows.InsertAfter(rowLength- 1);
                var currentColumn = i == 0 ? rowLength - 1 : rowLength;
                document.InsertText(pricingDetailsTable[currentColumn, 0].Range.Start, $"{i + 1}");
                document.InsertText(pricingDetailsTable[currentColumn, 1].Range.Start, items[i].Origin);
                document.InsertText(pricingDetailsTable[currentColumn, 2].Range.Start, items[i].Destination);
                document.InsertText(pricingDetailsTable[currentColumn, 3].Range.Start, items[i].TruckType);
                document.InsertText(pricingDetailsTable[currentColumn, 4].Range.Start, $"{items[i].Price} SR");
            }
            
            await using MemoryStream outputFileStream = new MemoryStream();

            await documentProcessor.ExportToPdfAsync(outputFileStream);

            BinaryObject pdfFile = new BinaryObject(null, outputFileStream.ToArray());
            var pdfFileId = await _binaryObjectRepository.InsertAndGetIdAsync(pdfFile);

            _appendixRepository.Update(appendix.Id, x => x.AppendixFileId = pdfFileId);
            return pdfFile;

        }
        
        private static Stream GetResourceStream(string resourceName)
        {
            return typeof(PricePackageAppendixManager).GetAssembly().GetManifestResourceStream(resourceName);
        }
    }
}