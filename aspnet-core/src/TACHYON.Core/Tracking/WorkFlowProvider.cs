using Abp.Dependency;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Tracking.WorkFlows;

namespace TACHYON.Tracking
{
    public class WorkFlowProvider : ITransientDependency
    {
        public List<WorkFlow> WorkFlows;
        public WorkFlowProvider()
        {
            WorkFlows = new List<WorkFlow>
        {
            new WorkFlow
                {
                Version = 1,
                Transactions = new List<PointTransaction>{
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.ArriveToLoadingLocation,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                   ToStatus = RoutePointStatus.ArriveToLoadingLocation,
                   Func = ArriveToLoadingLocation,
                   PickingType = PickingType.Pickup
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.StartLoading,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.ArriveToLoadingLocation,
                   ToStatus = RoutePointStatus.StartLoading,
                   Func = StartLoading,
                   PickingType = PickingType.Pickup
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.FinishLoading,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartLoading,
                   ToStatus = RoutePointStatus.FinishLoading,
                   Func = FinishLoading,
                   PickingType = PickingType.Pickup
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.StartedMovingToOfLoadingLocation,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.FinishLoading,
                   ToStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                   Func = StartedMovingToOfLoadingLocation,
                   PickingType = PickingType.Dropoff
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.ArrivedToDestination,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                   ToStatus = RoutePointStatus.ArrivedToDestination,
                   Func = ArrivedToDestination,
                   PickingType = PickingType.Dropoff

               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.StartOffloading,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.ArrivedToDestination,
                   ToStatus = RoutePointStatus.StartOffloading,
                   Func = StartOffloading,
                   PickingType = PickingType.Dropoff
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.FinishOffLoadShipment,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartOffloading,
                   ToStatus = RoutePointStatus.FinishOffLoadShipment,
                   IsResolved = true,
                   Func = FinishOffLoadShipment,
                   PickingType = PickingType.Dropoff
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.ReceiverConfirmed,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.FinishOffLoadShipment,
                   ToStatus = RoutePointStatus.ReceiverConfirmed,
                   Func = ReceiverConfirmed,
                   PickingType = PickingType.Dropoff
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.DeliveryConfirmation,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.FinishOffLoadShipment,
                   ToStatus = RoutePointStatus.DeliveryConfirmation,
                   Func = ReceiverConfirmed,
                   PickingType = PickingType.Dropoff
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.DeliveryConfirmation,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.ReceiverConfirmed,
                   ToStatus = RoutePointStatus.DeliveryConfirmation,
                   Func = DeliveryConfirmation,
                   PickingType = PickingType.Dropoff
               },
               new PointTransaction
               {
                   Action =  WorkFlowActionConst.DeliveryConfirmation,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.ReceiverConfirmed,
                   ToStatus = RoutePointStatus.DeliveryConfirmation,
                   Func = DeliveryConfirmation,
                   PickingType = PickingType.Dropoff
               },

            },
            },
        };
        }

        #region Transactions Functions
        private static void ArriveToLoadingLocation(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.ArriveToLoadingLocation;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        private static void StartLoading(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.StartLoading;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        private static void FinishLoading(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.FinishLoading;
            point.Status = status;
            trip.RoutePointStatus = status;
            point.IsComplete = true;
            point.EndTime = Clock.Now;
            point.ActualPickupOrDeliveryDate = trip.ActualPickupDate = Clock.Now;
            //await SendSmsToReceivers(trip.Id);
        }
        private static void StartedMovingToOfLoadingLocation(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.StartedMovingToOfLoadingLocation;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        private static void ArrivedToDestination(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.ArrivedToDestination;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        private static void StartOffloading(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.StartOffloading;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        private static void FinishOffLoadShipment(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.FinishOffLoadShipment;
            point.Status = status;
            trip.RoutePointStatus = status;
            point.ActualPickupOrDeliveryDate = Clock.Now;
            if (!trip.RoutPoints.Any(x => x.ActualPickupOrDeliveryDate == null && x.Id != point.Id))
                trip.ActualDeliveryDate = Clock.Now;
        }
        private static void ReceiverConfirmed(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.ReceiverConfirmed;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        private static void DeliveryConfirmation(RoutPoint point, ShippingRequestTrip trip)
        {
            var status = RoutePointStatus.DeliveryConfirmation;
            point.Status = status;
            trip.RoutePointStatus = status;
        }
        #endregion

        public List<PointTransaction> GetTransactionsByType(int workFlowVersion, PickingType pickingType)
        {
            return WorkFlows.FirstOrDefault(c => c.Version == workFlowVersion)
                .Transactions.Where(x => x.PickingType == pickingType).ToList();

        }
        public List<PointTransaction> GetAvailableTransactions(int workFlowVersion, RoutePointStatus status)
        {
            return WorkFlows.FirstOrDefault(c => c.Version == 1)
                .Transactions.Where(x => x.FromStatus == status).ToList();

        }
    }
}