using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using Abp.UI;
using DevExpress.Pdf;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
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
            
            (int lastAppendixMajorVersion, int lastAppendixMinorVersion) = await _appendixRepository.GetAll()
                // to do: Improve this in future 
               // .Where(x=> (!x.DestinationTenantId.HasValue || x.DestinationTenantId == createdAppendix.DestinationTenantId) || (!x.ProposalId.HasValue || x.Proposal.ShipperId == createdAppendix.p) )
                .OrderByDescending(x => x.MajorVersion).ThenByDescending(x=> x.MinorVersion)
                .Select(x => new Tuple<int, int>(x.MajorVersion, x.MinorVersion)).FirstOrDefaultAsync();

            #region Handle Versioning

            if (lastAppendixMajorVersion == default || lastAppendixMinorVersion == default)
            {
                createdAppendix.MajorVersion = 1;
                createdAppendix.MinorVersion = 1;
            }
            else if (lastAppendixMinorVersion == 9)
            {
                createdAppendix.MajorVersion = lastAppendixMajorVersion + 1;
                createdAppendix.MinorVersion = 1;
            }
            else
            {
                createdAppendix.MajorVersion = lastAppendixMajorVersion;
                createdAppendix.MinorVersion = lastAppendixMinorVersion + 1;
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
        
        public async Task UpdateAppendix(PricePackageAppendix updatedAppendix, List<string> pricePackages, string emailAddress)
        { 
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
                    AppendixId = updatedAppendix.Id, FileReceiverEmailAddress = emailAddress
                });
        }
        
        public async Task<BinaryObject> GenerateAppendixFile(int appendixId)
        {
            var appendix = await _appendixRepository.GetAll()
                .Include(x => x.Proposal)
                .ThenInclude(x => x.TmsPricePackages)
                .Include(x=> x.NormalPricePackages)
                .Include(x=> x.TmsPricePackages)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == appendixId);

            if (appendix is null) throw new EntityNotFoundException(L("AppendixNotFound"));
            
            await using var stream = GetResourceStream(TACHYONConsts.AppendixTemplateFullNamespace);
            using var documentProcessor = new RichEditDocumentServer();
            await documentProcessor.LoadDocumentAsync(stream);
            var document = documentProcessor.Document;

            // todo : create method to generate document version !important

            var truckTypes = appendix.Proposal?.TmsPricePackages?
                .Select(x => x.TrucksTypeFk?.DisplayName)?.Distinct()
                .ToArray();
            
            var transportTypes = appendix.Proposal?.TmsPricePackages?
                .Select(x => x.TransportTypeFk?.DisplayName)?.Distinct()
                .ToArray();
            
            var routeTypes = appendix.Proposal?.TmsPricePackages?
                .Select(x => Enum.GetName(typeof(ShippingRequestRouteType),x.RouteType))?.Distinct()
                .ToArray();
            var companyName = appendix.Proposal?.Shipper?.companyName;
            var appendixDate = appendix.AppendixDate.ToString("dd/MM/yyyy");
            var contractDate = appendix.CreationTime.ToString("dd/MM/yyyy");

            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractNumber, appendix.ContractName,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateAppendixDate, appendixDate ,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractDate, contractDate,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractNumber, $"{appendix?.Proposal?.Shipper?.ContractNumber}",SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateCompanyName, companyName,SearchOptions.None);
            if (truckTypes != null && truckTypes.Length > 0) 
                document.ReplaceAll(TACHYONConsts.AppendixTemplateTruckTypes, string.Join(',',truckTypes),SearchOptions.None);
            
            if (transportTypes != null && transportTypes.Length > 0) 
                document.ReplaceAll(TACHYONConsts.AppendixTemplateTransportTypes, string.Join(',',transportTypes),SearchOptions.None);
            
            if (routeTypes != null && routeTypes.Length > 0) 
                document.ReplaceAll(TACHYONConsts.AppendixTemplateRouteTypes, string.Join(',',routeTypes),SearchOptions.None);

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