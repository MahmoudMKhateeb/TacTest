using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Tracking.WorkFlows;

namespace TACHYON.Tracking
{
    public class PointTransactionArgs
    {
        public ShippingRequestTrip Trip { get; set; }
        public RoutPoint Point { get; set; }

        public string ConfirmationCode { get; set; }

    }

    public class TripTransactionArgs
    {
        public ShippingRequestTrip Trip { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TArgs">transaction args object warper</typeparam>
    /// <typeparam name="TEnum">statuses Enum</typeparam>
    public interface IWorkFlow<TArgs, TEnum> : IDomainService where TEnum : Enum
    {

        List<WorkFlow<TArgs, TEnum>> Flows { get; set; }

        List<WorkflowTransaction<TArgs, TEnum>> GetTransactionsByStatus(int workFlowVersion);

        List<WorkflowTransaction<TArgs, TEnum>> GetAvailableTransactions(int workFlowVersion, TEnum statusesEnum);
    }


    //point
    public class ShippingRequestPointWorkFlowProvider : TACHYONDomainServiceBase, IWorkFlow<PointTransactionArgs, RoutePointStatus>
    {
        public List<WorkFlow<PointTransactionArgs, RoutePointStatus>> Flows { get; set; }


        public ShippingRequestPointWorkFlowProvider()
        {
            Flows = new List<WorkFlow<PointTransactionArgs, RoutePointStatus>>
        {
            new WorkFlow<PointTransactionArgs,RoutePointStatus>
                {
                Version = 0,
                Transactions = new List<WorkflowTransaction<PointTransactionArgs,RoutePointStatus>>{
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.StartedMovingToLoadingLocation,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StandBy,
                   ToStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                   Func = StartedMovingToLoadingLocation,
                   PickingType = PickingType.Pickup
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.ArriveToLoadingLocation,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartedMovingToLoadingLocation,
                   ToStatus = RoutePointStatus.ArriveToLoadingLocation,
                   Func = ArriveToLoadingLocation,
                   PickingType = PickingType.Pickup
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.StartLoading,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.ArriveToLoadingLocation,
                   ToStatus = RoutePointStatus.StartLoading,
                   Func = StartLoading,
                   PickingType = PickingType.Pickup
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.FinishLoading,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartLoading,
                   ToStatus = RoutePointStatus.FinishLoading,
                   Func = FinishLoading,
                   PickingType = PickingType.Pickup
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.StartedMovingToOfLoadingLocation,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StandBy,
                   ToStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                   Func = StartedMovingToOfLoadingLocation,
                   PickingType = PickingType.Dropoff
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.ArrivedToDestination,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartedMovingToOfLoadingLocation,
                   ToStatus = RoutePointStatus.ArrivedToDestination,
                   Func = ArrivedToDestination,
                   PickingType = PickingType.Dropoff

               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.StartOffloading,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.ArrivedToDestination,
                   ToStatus = RoutePointStatus.StartOffloading,
                   Func = StartOffloading,
                   PickingType = PickingType.Dropoff
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.FinishOffLoadShipment,
                   IsOptional = false,
                   FromStatus = RoutePointStatus.StartOffloading,
                   ToStatus = RoutePointStatus.FinishOffLoadShipment,
                   IsResolved = true,
                   Func = FinishOffLoadShipment,
                   PickingType = PickingType.Dropoff
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.ReceiverConfirmed,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.FinishOffLoadShipment,
                   ToStatus = RoutePointStatus.ReceiverConfirmed,
                   Func = ReceiverConfirmed,
                   PickingType = PickingType.Dropoff
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.DeliveryConfirmation,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.FinishOffLoadShipment,
                   ToStatus = RoutePointStatus.DeliveryConfirmation,
                   Func = ReceiverConfirmed,
                   PickingType = PickingType.Dropoff
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
               {
                   Action =  WorkFlowActionConst.DeliveryConfirmation,
                   IsOptional = true,
                   FromStatus = RoutePointStatus.ReceiverConfirmed,
                   ToStatus = RoutePointStatus.DeliveryConfirmation,
                   Func = DeliveryConfirmation,
                   PickingType = PickingType.Dropoff
               },
               new WorkflowTransaction<PointTransactionArgs,RoutePointStatus>
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
        private void StartedMovingToLoadingLocation(PointTransactionArgs args)
        {

            var status = RoutePointStatus.StartedMovingToLoadingLocation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void ArriveToLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArriveToLoadingLocation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void StartLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartLoading;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void FinishLoading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishLoading;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
            args.Point.IsComplete = true;
            args.Point.IsResolve = true;
            args.Point.EndTime = Clock.Now;
            args.Point.ActualPickupOrDeliveryDate = args.Trip.ActualPickupDate = Clock.Now;
            //await SendSmsToReceivers(trip.Id);
        }
        private void StartedMovingToOfLoadingLocation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartedMovingToOfLoadingLocation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void ArrivedToDestination(PointTransactionArgs args)
        {
            var status = RoutePointStatus.ArrivedToDestination;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void StartOffloading(PointTransactionArgs args)
        {
            var status = RoutePointStatus.StartOffloading;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void FinishOffLoadShipment(PointTransactionArgs args)
        {
            var status = RoutePointStatus.FinishOffLoadShipment;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
            args.Point.ActualPickupOrDeliveryDate = Clock.Now;
            if (!args.Trip.RoutPoints.Any(x => x.ActualPickupOrDeliveryDate == null && x.Id != args.Point.Id))
                args.Trip.ActualDeliveryDate = Clock.Now;
        }
        private void ReceiverConfirmed(PointTransactionArgs args)
        {
            var code = args.ConfirmationCode;
            var status = RoutePointStatus.ReceiverConfirmed;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        private void DeliveryConfirmation(PointTransactionArgs args)
        {
            var status = RoutePointStatus.DeliveryConfirmation;
            args.Point.Status = status;
            args.Trip.RoutePointStatus = status;
        }
        #endregion

        public List<WorkflowTransaction<PointTransactionArgs, RoutePointStatus>> GetTransactionsByStatus(int workFlowVersion)
        {
            return Flows
                .FirstOrDefault(c => c.Version == workFlowVersion)
                .Transactions
                // .Where(x => x.PickingType == pickingType)
                .ToList();

        }
        public List<WorkflowTransaction<PointTransactionArgs, RoutePointStatus>> GetAvailableTransactions(int workFlowVersion, RoutePointStatus status)
        {
            return Flows
                .FirstOrDefault(c => c.Version == workFlowVersion)
                .Transactions
                //.Where(x => x.FromStatus == status && x.PickingType == pickingType)
                .ToList();

        }

        [UnitOfWork]
        public void Invoke(WorkflowTransaction<PointTransactionArgs, RoutePointStatus> transaction, PointTransactionArgs args)
        {

            //get transaction 

            //trans
            transaction.Func(args);






        }
    }




    //trip
    public class ShippingRequestTripWorkFlowProvider : IWorkFlow<TripTransactionArgs, ShippingRequestTripDriverStatus>
    {
        public List<WorkFlow<TripTransactionArgs, ShippingRequestTripDriverStatus>> Flows { get; set; }

        public ShippingRequestTripWorkFlowProvider()
        {
            Flows = new List<WorkFlow<TripTransactionArgs, ShippingRequestTripDriverStatus>>
            {
                new WorkFlow<TripTransactionArgs, ShippingRequestTripDriverStatus>
                {
                    Version = 0,
                    Transactions =
                        new List<WorkflowTransaction<TripTransactionArgs, ShippingRequestTripDriverStatus>>
                        {
                            new WorkflowTransaction<TripTransactionArgs, ShippingRequestTripDriverStatus>
                            {
                                Action = WorkFlowActionConst.StartedMovingToLoadingLocation,
                                IsOptional = false,
                                FromStatus = ShippingRequestTripDriverStatus.Accepted,
                                ToStatus = ShippingRequestTripDriverStatus.Rejected,
                                Func = TripTransactionTest,
                                PickingType = PickingType.Pickup
                            }
                        },
                },
            };
        }


        public List<WorkflowTransaction<TripTransactionArgs, ShippingRequestTripDriverStatus>> GetTransactionsByStatus(int workFlowVersion)
        {
            return Flows.FirstOrDefault(c => c.Version == workFlowVersion).Transactions.ToList();
        }

        public List<WorkflowTransaction<TripTransactionArgs, ShippingRequestTripDriverStatus>> GetAvailableTransactions(int workFlowVersion, ShippingRequestTripDriverStatus statusenum)
        {
            return Flows.FirstOrDefault(c => c.Version == workFlowVersion)
                .Transactions.Where(x => x.FromStatus == statusenum)
                .ToList();
        }



        #region Transactions Functions

        private static void TripTransactionTest(TripTransactionArgs args)
        {
            var trip = args.Trip;
        }

        #endregion
    }




}