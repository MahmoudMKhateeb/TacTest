using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using NetTopologySuite.Geometries;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Routs.RoutPoints
{
    public class RouteTransitionManager : TACHYONDomainServiceBase
    {

        private readonly IRepository<RoutePointTransition> _RoutePointTransitionRepository;
        private readonly IRepository<RoutPoint,long> _RoutPointRepository;


        List<RoutPoint> RoutPoints;
        public RouteTransitionManager(IRepository<RoutePointTransition> RoutePointTransitionRepository)
        {
            _RoutePointTransitionRepository = RoutePointTransitionRepository;
        }

        


        //public void Start(long RequestId)
        //{
        //     RoutPoints = _RoutPointRepository.GetAll()
        //         .Include(i => i.FacilityFk)
        //         .Where(x => x.ShippingRequestId == RequestId).OrderBy(x => x.ParentId).ToList();

        //    var PikupPoint = RoutPoints.Single(x => !x.ParentId.HasValue);
        //    RoutPoints.Remove(PikupPoint);
        //    BuildTransition(PikupPoint);
        //}
        private async void BuildTransition(RoutPoint FromPoint)
        {
            var PointTo = FindClosestPoint(FromPoint);
            var PointTransition = new RoutePointTransition();
            PointTransition.FromPointId = FromPoint.Id;
            if (PointTo != null)
            {
                PointTransition.ToPointId = PointTo.Id;
                RoutPoints.Remove(PointTo);
                BuildTransition(PointTo);
            }

           await _RoutePointTransitionRepository.InsertAsync(PointTransition);
        }

        private RoutPoint FindClosestPoint(RoutPoint p)
        {
            double dist = double.MaxValue;
            RoutPoint ClosestPoint = null;
            foreach (var point in RoutPoints)
            {
                var d = DistanceTo(p, point);
                if (d < dist)
                {
                    dist = d;
                    ClosestPoint = point;
                }
            }
            return ClosestPoint;
        }
        private double DistanceTo(RoutPoint p1, RoutPoint p2)
        {
            double rlat1 = Math.PI * p1.FacilityFk.Location.X / 180;
            double rlat2 = Math.PI * p2.FacilityFk.Location.X / 180;
            double theta = p1.FacilityFk.Location.Y - p1.FacilityFk.Location.Y;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist;
        }
    }

}


