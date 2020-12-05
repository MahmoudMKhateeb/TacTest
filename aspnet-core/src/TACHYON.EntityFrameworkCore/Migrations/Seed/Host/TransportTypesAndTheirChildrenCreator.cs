using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.EntityFrameworkCore;
using TACHYON.Trucks.TruckCategories.TransportSubtypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckSubtypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Migrations.Seed.Host
{
    public class TransportTypesAndTheirChildrenCreator
    {
        private readonly TACHYONDbContext _context;
        public TransportTypesAndTheirChildrenCreator(TACHYONDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //create Ambient transportType
            CreateOrUpdateTransportType("Ambient", 1);
            CreateOrUpdateTransportType("Cold Chain", 2);

            //create tranport subtypes
            CreateOrUpdateTransportSubType("General", 1, 1);
            CreateOrUpdateTransportSubType("Temp controlled", 2, 2);
            CreateOrUpdateTransportSubType("Frozen", 3, 2);

            //create truck type
            CreateOrUpdateTruckType("Flatbed", 1, 1);
            CreateOrUpdateTruckType("Closed box trailer", 2, 1);
            CreateOrUpdateTruckType("Curtain sides trailer", 3, 1);
            CreateOrUpdateTruckType("Sidewall trailer", 4, 1);
            CreateOrUpdateTruckType("Open top", 5, 1);
            CreateOrUpdateTruckType("Car Shuttler", 6, 1);
            CreateOrUpdateTruckType("Dyana & smaller capacities", 7, 1);
            CreateOrUpdateTruckType("Genset", 8, 2);
            CreateOrUpdateTruckType("Closed box reefer ", 9, 2);
            CreateOrUpdateTruckType("Reefer Dyana & smaller capacity", 10, 2);
            CreateOrUpdateTruckType("Genset", 11, 3);
            CreateOrUpdateTruckType("Closed box freezer", 12, 3);
            CreateOrUpdateTruckType("Freezer Dyana & smaller capacities", 13, 3);

            //create trucksubtypes
            CreateOrUpdateTruckSubType("General", 1, 1);
            CreateOrUpdateTruckSubType("General", 2, 2);
            CreateOrUpdateTruckSubType("General", 3, 3);
            CreateOrUpdateTruckSubType("General", 4, 4);
            CreateOrUpdateTruckSubType("General", 5, 5);
            CreateOrUpdateTruckSubType("General", 6, 6);
            CreateOrUpdateTruckSubType("Closed box", 7, 7);
            CreateOrUpdateTruckSubType("Open Top", 8, 7);
            CreateOrUpdateTruckSubType("Open Top", 9, 8);
            CreateOrUpdateTruckSubType("Open Top", 10, 9);
            CreateOrUpdateTruckSubType("Open Top", 11, 10);
            CreateOrUpdateTruckSubType("Open Top", 12, 11);
            CreateOrUpdateTruckSubType("Open Top", 13, 12);
            CreateOrUpdateTruckSubType("Open Top", 14, 13);

            //create Capacities
            CreateOrUpdateCapacity("Max Payload at 24 Tonnes", 1, 1);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 2, 2);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 3, 3);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 4, 4);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 5, 5);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 6, 6);
            CreateOrUpdateCapacity("Capacities as a number in tonnes 'payload'.i,e, 2Tonnes, 5 Tonnes..up to 12 Tonnes", 7, 7);
            CreateOrUpdateCapacity("Capacities as a number in tonnes 'payload'. i,e, 2Tonnes, 5 Tonnes..up to 12 Tonnes", 8, 8);
            CreateOrUpdateCapacity("Max Payload at 24 Tonnes", 9, 9);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 10, 10);
            CreateOrUpdateCapacity("Capacities as a number in tonnes 'payload'. i,e, 2Tonnes, 5 Tonnes..up to 12 Tonnes", 11, 11);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 12, 12);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 13, 13);
            CreateOrUpdateCapacity("Payload at 24 Tonnes", 14, 14);

        }


        #region TransportType
        private void CreateOrUpdateTransportType(string displayName, int id)
        {
            var item = _context.TransportTypes.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                AddTransportTypeToDB(displayName, id);
            }
            else
            {
                UpdateTransportType(item, displayName);
            }
        }

        private void AddTransportTypeToDB(string displayName,int id)
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TransportTypes] ON");
            var newItem = new TransportType() { DisplayName = displayName, IsDeleted = false, Id = id };
            _context.TransportTypes.Add(newItem);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TransportTypes] OFF");
        }

        private void UpdateTransportType(TransportType item,string displayName)
        {
            item.DisplayName = displayName;
            item.IsDeleted = false;
            _context.TransportTypes.Update(item);
            _context.SaveChanges();
        }
        #endregion

        #region TransportSubType
        private void CreateOrUpdateTransportSubType(string displayName, int id,int transportTypeId)
        {
            var item = _context.TransportSubtypes.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id );
            if (item == null)
            {
                AddTransportSubTypeToDB(displayName, id,transportTypeId);
            }
            else
            {
                UpdateTransportSubType(item, displayName,transportTypeId);
            }
        }

        private void AddTransportSubTypeToDB(string displayName, int id,int transportTypeId)
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TransportSubtypes] ON");
            var newItem = new TransportSubtype() { DisplayName = displayName, IsDeleted = false, Id = id,TransportTypeId=transportTypeId };
            _context.TransportSubtypes.Add(newItem);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TransportSubtypes] OFF");
        }

        private void UpdateTransportSubType(TransportSubtype item, string displayName, int transportTypeId)
        {
            item.DisplayName = displayName;
            item.IsDeleted = false;
            item.TransportTypeId = transportTypeId;
            _context.TransportSubtypes.Update(item);
            _context.SaveChanges();
        }
        #endregion


        #region TruckType
        private void CreateOrUpdateTruckType(string displayName, int id, int transportSubTypeId)
        {
            var item = _context.TrucksTypes.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                AddTruckTypeToDB(displayName, id, transportSubTypeId);
            }
            else
            {
                UpdateTruckType(item, displayName,transportSubTypeId);
            }
        }

        private void AddTruckTypeToDB(string displayName, int id, int transportSubTypeId)
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TrucksTypes] ON");
            var newItem = new TrucksType() { DisplayName = displayName, IsDeleted = false, Id = id, TransportSubtypeId = transportSubTypeId };
            _context.TrucksTypes.Add(newItem);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TrucksTypes] OFF");
        }

        private void UpdateTruckType(TrucksType item, string displayName, int transportSubTypeId)
        {
            item.DisplayName = displayName;
            item.IsDeleted = false;
            item.TransportSubtypeId = transportSubTypeId;
            _context.TrucksTypes.Update(item);
            _context.SaveChanges();
        }

        #endregion

        #region TruckSubType
        private void CreateOrUpdateTruckSubType(string displayName, int id, int truckTypeId)
        {
            var item = _context.TruckSubtypes.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                AddTruckSubTypeToDB(displayName, id, truckTypeId);
            }
            else
            {
                UpdateTruckSubType(item, displayName,truckTypeId);
            }
        }

        private void AddTruckSubTypeToDB(string displayName, int id, int truckTypeId)
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TruckSubtypes] ON");
            var newItem = new TruckSubtype() { DisplayName = displayName, IsDeleted = false, Id = id, TrucksTypeId = truckTypeId };
            _context.TruckSubtypes.Add(newItem);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[TruckSubtypes] OFF");
        }

        private void UpdateTruckSubType(TruckSubtype item, string displayName, int truckTypeId)
        {
            item.DisplayName = displayName;
            item.IsDeleted = false;
            item.TrucksTypeId = truckTypeId;
            _context.TruckSubtypes.Update(item);
            _context.SaveChanges();
        }

        #endregion

        #region Capacity
        private void CreateOrUpdateCapacity(string displayName, int id, int truckSubTypeId)
        {
            var item = _context.Capacities.IgnoreQueryFilters().FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                AddCapacityToDB(displayName, id, truckSubTypeId);
            }
            else
            {
                UpdateCapacity(item, displayName, truckSubTypeId);
            }
        }

        private void AddCapacityToDB(string displayName, int id, int truckSubTypeId)
        {
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Capacities] ON");
            var newItem = new Capacity() { DisplayName = displayName, IsDeleted = false, Id = id, TruckSubtypeId = truckSubTypeId };
            _context.Capacities.Add(newItem);
            _context.SaveChanges();
            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [dbo].[Capacities] OFF");
        }

        private void UpdateCapacity(Capacity item, string displayName,int truckSubTypeId)
        {
            item.DisplayName = displayName;
            item.IsDeleted = false;
            item.TruckSubtypeId = truckSubTypeId;
            _context.Capacities.Update(item);
            _context.SaveChanges();
        }
        #endregion

    }
}
