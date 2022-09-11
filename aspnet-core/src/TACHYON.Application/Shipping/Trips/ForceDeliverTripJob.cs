using Abp;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Timing;
using NUglify.Helpers;
using System;
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
        }

        [UnitOfWork]
        protected override async Task ExecuteAsync(ForceDeliverTripJobArgs args)
        {

            
            _workFlowProvider.AbpSession.Use(args.RequestedByTenantId,args.RequestedByUserId);
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId);
            var importedTripDeliveryDetails = _excelDataReader.GetTripDeliveryDetails(binaryObject.Bytes);
            importedTripDeliveryDetails.ForEach(_workFlowProvider.ForceDeliverTrip);
            await _appNotifier.NotifyUserWhenBulkDeliverySucceeded(new UserIdentifier(args.RequestedByTenantId,
                args.RequestedByUserId));
        }
        
        
    }
}