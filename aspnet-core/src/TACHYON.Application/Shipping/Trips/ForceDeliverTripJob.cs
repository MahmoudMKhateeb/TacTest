using Abp;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Internal;
using Hangfire;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Notifications;
using TACHYON.Shipping.Trips.Importing;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.Storage;
using TACHYON.Tracking;

namespace TACHYON.Shipping.Trips
{
    public class ForceDeliverTripJob : AsyncBackgroundJob<ForceDeliverTripJobArgs>, ITransientDependency
    {
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ForceDeliverTripExcelDataReader _excelDataReader;
        private readonly ShippingRequestPointWorkFlowProvider _workFlowProvider;
        private readonly IAppNotifier _appNotifier;

        public ForceDeliverTripJob(
            IBinaryObjectManager binaryObjectManager,
            ForceDeliverTripExcelDataReader excelDataReader,
            ShippingRequestPointWorkFlowProvider workFlowProvider,
            IAppNotifier appNotifier)
        {
            _binaryObjectManager = binaryObjectManager;
            _excelDataReader = excelDataReader;
            _workFlowProvider = workFlowProvider;
            _appNotifier = appNotifier;
            LocalizationSourceName = TACHYONConsts.LocalizationSourceName;
        }

        [UnitOfWork]
        protected override async Task ExecuteAsync(ForceDeliverTripJobArgs args)
        {

            
            _workFlowProvider.AbpSession.Use(args.RequestedByTenantId,args.RequestedByUserId);
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId);
            var importedTripDeliveryDetails = _excelDataReader.GetTripDeliveryDetails(binaryObject.Bytes).ToList();
            var userIdentifier = new UserIdentifier(args.RequestedByTenantId,
                args.RequestedByUserId);
            if (!importedTripDeliveryDetails.IsNullOrEmpty() &&
                importedTripDeliveryDetails.All(x => string.IsNullOrEmpty(x.Exception)))
            {
                importedTripDeliveryDetails.ForEach(_workFlowProvider.ForceDeliverTrip);
                await _appNotifier.NotifyUserWhenBulkDeliverySucceeded(userIdentifier);
                return;
            }

            string errorMsg = string.Empty;
            if (importedTripDeliveryDetails.IsNullOrEmpty())
                errorMsg = L("EmptyFileError");
            else
            {
                importedTripDeliveryDetails.Where(x=> !string.IsNullOrEmpty(x.Exception))
                    .Select(x => x.Exception + "\n")
                    .ToList().ForEach(i => errorMsg += i);
            }
            await _appNotifier.NotifyUserWhenBulkDeliveryFailed(userIdentifier, errorMsg);

        }
        
        
    }
}