using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using Abp.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PricePackages.PricePackageProposals.Jobs;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageProposals
{
    public class PricePackageProposalManager : TACHYONDomainServiceBase, IPricePackageProposalManager
    {
        private readonly IRepository<PricePackageProposal> _proposalRepository;
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<BinaryObject,Guid> _binaryObjectRepository;
        private readonly IBackgroundJobManager _jobManager;

        public PricePackageProposalManager(
            IRepository<PricePackageProposal> proposalRepository,
            IRepository<TmsPricePackage> tmsPricePackageRepository, 
            IRepository<BinaryObject, Guid> binaryObjectRepository,
            IBackgroundJobManager jobManager)
        {
            _proposalRepository = proposalRepository;
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _binaryObjectRepository = binaryObjectRepository;
            _jobManager = jobManager;
        }

        public async Task<int> CreateProposal(PricePackageProposal createdProposal,List<int> tmsPricePackages,string emailAddress)
        {
            
            // check name if already used before

            bool isNameDuplicated = await _proposalRepository.GetAll()
                .AnyAsync(x => x.ProposalName.Equals(createdProposal.ProposalName));

            if (isNameDuplicated) throw new UserFriendlyException(L("ProposalNameAlreadyUsedBefore"));
            // check items if them used in any another proposal

            bool anyItemUsedInAnotherProposal = await _tmsPricePackageRepository.GetAll()
                .AnyAsync(x => tmsPricePackages.Any(i => i == x.Id) && x.ProposalId.HasValue);

            if (anyItemUsedInAnotherProposal)
                throw new UserFriendlyException(L("YouCanNotAddItemUsedInAnotherProposal"));
            
            // check items if them for another shipper 
            bool anyItemNotForSelectedShipper = await _tmsPricePackageRepository.GetAll()
                .AnyAsync(x => tmsPricePackages.Any(i => i == x.Id) && x.ShipperId != createdProposal.ShipperId);
            
            if (anyItemNotForSelectedShipper) 
                throw new UserFriendlyException(L("YouMustSelectItemForSelectedShipper"));
            
            var createdProposalId = await _proposalRepository.InsertAndGetIdAsync(createdProposal);
            
            tmsPricePackages.ForEach(tmsPricePackageId=>
            {
                _tmsPricePackageRepository.Update(tmsPricePackageId, x => x.ProposalId = createdProposalId);
            });
            await _jobManager.EnqueueAsync<GenerateProposalFileJob, GenerateProposalFileJobArgument>(
                new GenerateProposalFileJobArgument()
                {
                    ProposalId = createdProposalId, ProposalReceiverEmailAddress = emailAddress
                });
            return createdProposalId;
        }

        public async Task UpdateProposal(PricePackageProposal updatedProposal, string emailAddress)
        {
            if (updatedProposal.ProposalFileId.HasValue) 
                await _binaryObjectRepository.DeleteAsync(updatedProposal.ProposalFileId.Value);

            var jobArgument = new GenerateProposalFileJobArgument()
            {
                ProposalId = updatedProposal.Id,
                ProposalReceiverEmailAddress = emailAddress
            };
            
            await _jobManager.EnqueueAsync<GenerateProposalFileJob, GenerateProposalFileJobArgument>(jobArgument);
            
        }

        public async Task<BinaryObject> GenerateProposalPdfFile(PricePackageProposal proposal)
        {
            using var documentProcessor = new RichEditDocumentServer();
            await using var documentStream = GetResourceStream(TACHYONConsts.ProposalTemplateFullNamespace);
            await documentProcessor.LoadDocumentAsync(documentStream, DocumentFormat.Rtf);
            var document = documentProcessor.Document;
            string proposalDate = proposal.ProposalDate?.ToString("dd-MM-yyyy");
            var truckTypes = proposal.TmsPricePackages.Select(x => x.TrucksTypeFk.Key)
                .Distinct().ToList();
            var routeTypes =  proposal.TmsPricePackages.Select(x => Enum.GetName(typeof(ShippingRequestRouteType), x.RouteType))
                .Distinct().ToList();
            var shippingTypes = proposal.TmsPricePackages.Select(x => Enum.GetName(typeof(PricePackageType), x.Type))
                .Distinct().ToList();
            
            document.ReplaceAll(TACHYONConsts.ProposalTemplateCompanyName, proposal.Shipper.companyName,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.ProposalTemplateDate,proposalDate ,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.ProposalTemplateTruckType, string.Join(", ",truckTypes),SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.ProposalTemplateRouteType, string.Join(", ",routeTypes),SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.ProposalTemplateShippingType, string.Join(", ",shippingTypes) ,SearchOptions.None);

            var routeDetailsTable = document.Tables[1];

            var routeDetails = proposal?.TmsPricePackages?.Select(x => new
            {
                OriginCity = x.OriginCity.DisplayName,DestinationCity = x.DestinationCity.DisplayName,
                TmsPrice = x.TachyonManageTotalPrice, DrPrice= x.DirectRequestTotalPrice
            }).ToArray();
            
            for (int i = 0; i < routeDetails?.Length; i++)
            {
                int rowLenght = routeDetailsTable.Rows.Count;
                if (i > 0) routeDetailsTable.Rows.InsertAfter(rowLenght- 1);
                var currentColumn = i == 0 ? rowLenght - 1 : rowLenght;
                document.InsertText(routeDetailsTable[currentColumn, 0].Range.Start, $"{i + 1}");
                document.InsertText(routeDetailsTable[currentColumn, 1].Range.Start, routeDetails[i].OriginCity);
                document.InsertText(routeDetailsTable[currentColumn, 2].Range.Start, routeDetails[i].DestinationCity);
                document.InsertText(routeDetailsTable[currentColumn, 3].Range.Start, $"{routeDetails[i].TmsPrice} SR");
                document.InsertText(routeDetailsTable[currentColumn, 4].Range.Start, $"{routeDetails[i].DrPrice} SR");
            }
            await using var memoryStream = new MemoryStream();
            await documentProcessor.ExportToPdfAsync(memoryStream);

            BinaryObject pdfProposal = new BinaryObject(null, memoryStream.ToArray());
            
            var fileId = await _binaryObjectRepository.InsertAndGetIdAsync(pdfProposal);
            
            _proposalRepository.Update(proposal.Id, x => x.ProposalFileId = fileId);
            return pdfProposal;
        }
        private static Stream GetResourceStream(string resourceName)
        {
            return typeof(PricePackageProposalManager).GetAssembly().GetManifestResourceStream(resourceName);
        }
    }
}