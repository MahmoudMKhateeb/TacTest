using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.ObjectMapping;
using Abp.Threading;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization.Roles;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Notifications;
using TACHYON.Storage;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.PlateTypes;

namespace TACHYON.Trucks.Importing.Dto
{
    public class ImportTrucksToExcelJob : BackgroundJob<ImportTrucksFromExcelJobArgs>, ITransientDependency
    {
        private readonly ITruckListExcelDataReader _truckListExcelDataReader;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<PlateType> _plateTypeRepository;
        private readonly IInvalidTruckExporter _invalidTruckExporter;
        private readonly IAppNotifier _appNotifier;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IObjectMapper _objectMapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly TruckManager _truckManager;

        public ImportTrucksToExcelJob(
            IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            IObjectMapper objectMapper,
            IUnitOfWorkManager unitOfWorkManager,
            ITruckListExcelDataReader truckListExcelDataReader,
            IRepository<Truck, long> truckRepository,
            IInvalidTruckExporter invalidTruckExporter,
            IRepository<DocumentFile, Guid> documentFileRepository,
            IRepository<PlateType> plateTypeRepository,
            TruckManager truckManager)
        {
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _objectMapper = objectMapper;
            _unitOfWorkManager = unitOfWorkManager;
            _truckListExcelDataReader = truckListExcelDataReader;
            _truckRepository = truckRepository;
            _invalidTruckExporter = invalidTruckExporter;
            _documentFileRepository = documentFileRepository;
            _truckManager = truckManager;
            _plateTypeRepository = plateTypeRepository;
        }

        public override void Execute(ImportTrucksFromExcelJobArgs args)
        {
            var trucks = GetTruckListFromExcelOrNull(args);
            if (trucks == null || !trucks.Any())
            {
                SendInvalidExcelNotification(args);
                return;
            }

            CreateTrucks(args, trucks);
        }

        private List<ImportTruckDto> GetTruckListFromExcelOrNull(ImportTrucksFromExcelJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    try
                    {
                        var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                        return _truckListExcelDataReader.GetTrucksFromExcel(file.Bytes);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    finally
                    {
                        uow.Complete();
                    }
                }
            }
        }

        private void CreateTrucks(ImportTrucksFromExcelJobArgs args, List<ImportTruckDto> Trucks)
        {
            var invalidTrucks = new List<ImportTruckDto>();

            foreach (var Truck in Trucks)
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                    {
                        if (Truck.CanBeImported())
                        {
                            try
                            {
                                AsyncHelper.RunSync(() => CreateTruckAsync(Truck));
                            }
                            catch (UserFriendlyException exception)
                            {
                                Truck.Exception = exception.Message;
                                invalidTrucks.Add(Truck);
                            }
                            catch (Exception exception)
                            {
                                Truck.Exception = exception.ToString();
                                invalidTrucks.Add(Truck);
                            }
                        }
                        else
                        {
                            invalidTrucks.Add(Truck);
                        }
                    }

                    uow.Complete();
                }
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    AsyncHelper.RunSync(() => ProcessImportTrucksResultAsync(args, invalidTrucks));
                }

                uow.Complete();
            }
        }

        private async Task CreateTruckAsync(ImportTruckDto input)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();


            var truck = _objectMapper.Map<Truck>(input);

            if (tenantId.HasValue)
            {
                truck.TenantId = tenantId.Value;
            }

            truck.DocumentFiles = new List<DocumentFile>();

            foreach (var importTruckDocumentFileDto in input.ImportTruckDocumentFileDtos)
            {
                importTruckDocumentFileDto.TruckId = truck.Id;
                var documentFile = _objectMapper.Map<DocumentFile>(importTruckDocumentFileDto);
                documentFile.TenantId = tenantId;
                truck.DocumentFiles.Add(documentFile);
            }

            truck.PlateTypeId = await _plateTypeRepository.GetAll()
                .AsNoTracking().Where(x => x.IsDefault)
                .Select(x => x.Id).FirstOrDefaultAsync();

            await _truckManager.CreateAsync(truck);
        }

        private async Task ProcessImportTrucksResultAsync(ImportTrucksFromExcelJobArgs args,
            List<ImportTruckDto> invalidTrucks)
        {
            if (invalidTrucks.Any())
            {
                var file = _invalidTruckExporter.ExportToFile(invalidTrucks);
                await _appNotifier.SomeTrucksCouldntBeImported(args.User, file.FileToken, file.FileType, file.FileName);
            }
            else
            {
                await _appNotifier.SendMessageAsync(
                    args.User,
                    new LocalizableString("AllTrucksSuccessfullyImportedFromExcel,PleaseUploadAllMissingDocuments",
                        TACHYONConsts.LocalizationSourceName),
                    null,
                    Abp.Notifications.NotificationSeverity.Success);
            }
        }

        private void SendInvalidExcelNotification(ImportTrucksFromExcelJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                        args.User,
                        new LocalizableString("FileCantBeConvertedToTruckList", TACHYONConsts.LocalizationSourceName),
                        null,
                        Abp.Notifications.NotificationSeverity.Warn));
                }

                uow.Complete();
            }
        }
    }
}