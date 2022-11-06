using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Reflection.Extensions;
using DevExpress.Pdf;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Storage;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    public class PricePackageAppendixManager : TACHYONDomainServiceBase, IPricePackageAppendixManager
    {
        private readonly IRepository<PricePackageAppendix> _appendixRepository;
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;

        public PricePackageAppendixManager(
            IRepository<PricePackageAppendix> appendixRepository,
            IRepository<BinaryObject, Guid> binaryObjectRepository)
        {
            _appendixRepository = appendixRepository;
            _binaryObjectRepository = binaryObjectRepository;
        }

        public async Task<BinaryObject> GenerateAppendixFile(int appendixId)
        {
            var appendix = await _appendixRepository.GetAll()
                .Include(x => x.Proposal)
                .ThenInclude(x => x.TmsPricePackages)
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
            var contractDate = appendix.ContractDate.ToString("dd/MM/yyyy");

            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractNumber, appendix.ContractName,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateAppendixDate, appendixDate ,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractDate, contractDate,SearchOptions.None);
            document.ReplaceAll(TACHYONConsts.AppendixTemplateContractNumber, $"{appendix.ContractNumber}",SearchOptions.None);
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