using Abp;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Internal;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Notifications;
using TACHYON.Shipping.Trips.Importing;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.Storage;
using TACHYON.Tracking;
using TACHYON.Tracking.Dto;

namespace TACHYON.Shipping.Trips
{
    [AutomaticRetry(Attempts = 1)]
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
            LocalizationSourceName ??= TACHYONConsts.LocalizationSourceName;
        }

        [UnitOfWork]
        protected override async Task ExecuteAsync(ForceDeliverTripJobArgs args)
        {

            
            _workFlowProvider.AbpSession.Use(args.RequestedByTenantId,args.RequestedByUserId);
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId);
            UserIdentifier userIdentifier = new UserIdentifier(args.RequestedByTenantId,
                args.RequestedByUserId);
            if (binaryObject == null)
            {
                await _appNotifier.NotifyUserWhenBulkDeliveryFailed(userIdentifier, L("NotFoundOrDeletedFileError"));
                return;
            }

            List<ImportTripTransactionFromExcelDto> importedTripDeliveryDetails =
                _excelDataReader.GetTripDeliveryDetails(binaryObject.Bytes).ToList();
            if (importedTripDeliveryDetails.IsNullOrEmpty() ||
                importedTripDeliveryDetails.Any(x => !x.Exception.IsNullOrEmpty()))
            {
                string errorMsg = importedTripDeliveryDetails.IsNullOrEmpty()
                    ? L("EmptyFileError")
                    : GetErrorsMessage(importedTripDeliveryDetails);

                await _appNotifier.NotifyUserWhenBulkDeliveryFailed(userIdentifier, errorMsg);
                return;
            }

            try
            {
                importedTripDeliveryDetails.ForEach(_workFlowProvider.ForceDeliverTrip);
            }
            catch (Exception e)
            {
                await _appNotifier.NotifyUserWhenBulkDeliveryFailed(userIdentifier, e.Message);
                Logger.Error(e.Message, e);
                return;
            }

            await _appNotifier.NotifyUserWhenBulkDeliverySucceeded(userIdentifier);
            await _binaryObjectManager.DeleteAsync(args.BinaryObjectId);
        }

        private static string GetErrorsMessage(
            IEnumerable<ImportTripTransactionFromExcelDto> importedTripDeliveryDetails)
        {
            string errorMsg = string.Empty;

            importedTripDeliveryDetails.Where(x => !string.IsNullOrEmpty(x.Exception))
                .Select(x => x.Exception + "\n")
                .ToList().ForEach(i => errorMsg += i);

            return errorMsg;
        }
        
        
    }
}