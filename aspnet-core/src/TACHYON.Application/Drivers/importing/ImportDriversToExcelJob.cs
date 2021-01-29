using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.ObjectMapping;
using Abp.Threading;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Authorization.Users.Importing;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Drivers.Dto;
using TACHYON.Drivers.importing.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Storage;

namespace TACHYON.Drivers.importing
{
    public class ImportDriversToExcelJob : BackgroundJob<ImportDriversFromExcelJobArgs>, ITransientDependency
    {
        private readonly RoleManager _roleManager;
        private readonly IDriverListExcelDataReader _driverListExcelDataReader;
        private readonly IInvalidDriverExporter _invalidDriverExporter;
        private readonly IUserPolicy _userPolicy;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAppNotifier _appNotifier;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IObjectMapper _objectMapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;

        public UserManager UserManager { get; set; }

        public ImportDriversToExcelJob(
            RoleManager roleManager,
            IDriverListExcelDataReader driverListExcelDataReader,
            IInvalidDriverExporter invalidDriverExporter,
            IUserPolicy userPolicy,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IAppNotifier appNotifier,
            IBinaryObjectManager binaryObjectManager,
            IObjectMapper objectMapper,
            IUnitOfWorkManager unitOfWorkManager, TenantManager tenantManager, IRepository<DocumentFile, Guid> documentFileRepository)
        {
            _roleManager = roleManager;
            _driverListExcelDataReader = driverListExcelDataReader;
            _invalidDriverExporter = invalidDriverExporter;
            _userPolicy = userPolicy;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _appNotifier = appNotifier;
            _binaryObjectManager = binaryObjectManager;
            _objectMapper = objectMapper;
            _unitOfWorkManager = unitOfWorkManager;
            _tenantManager = tenantManager;
            _documentFileRepository = documentFileRepository;
        }

        public override void Execute(ImportDriversFromExcelJobArgs args)
        {
            var drivers = GetDriverListFromExcelOrNull(args);
            if (drivers == null || !drivers.Any())
            {
                SendInvalidExcelNotification(args);
                return;
            }

            CreateDrivers(args, drivers);
        }

        private List<ImportDriverDto> GetDriverListFromExcelOrNull(ImportDriversFromExcelJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    try
                    {
                        var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                        return _driverListExcelDataReader.GetDriversFromExcel(file.Bytes);
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

        private void CreateDrivers(ImportDriversFromExcelJobArgs args, List<ImportDriverDto> drivers)
        {
            var invalidDrivers = new List<ImportDriverDto>();


            var tenancyName = _tenantManager.GetById(args.TenantId.Value).TenancyName;


            foreach (var driver in drivers)
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                    {
                        if (driver.CanBeImported())
                        {
                            try
                            {
                                AsyncHelper.RunSync(() => CreateDriverAsync(driver, tenancyName));
                            }
                            catch (UserFriendlyException exception)
                            {
                                driver.Exception = exception.Message;
                                invalidDrivers.Add(driver);
                            }
                            catch (Exception exception)
                            {
                                driver.Exception = exception.ToString();
                                invalidDrivers.Add(driver);
                            }
                        }
                        else
                        {
                            invalidDrivers.Add(driver);
                        }
                    }

                    uow.Complete();
                }
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    AsyncHelper.RunSync(() => ProcessImportDriversResultAsync(args, invalidDrivers));
                }

                uow.Complete();
            }
        }

        private async Task CreateDriverAsync(ImportDriverDto input, string tenancyName)
        {
            var tenantId = CurrentUnitOfWork.GetTenantId();

            if (tenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(tenantId.Value);
            }

            var driver = _objectMapper.Map<User>(input); //Passwords is not mapped (see mapping configuration)

            //password
            var randomPassword = await UserManager.CreateRandomPassword();
            driver.Password = _passwordHasher.HashPassword(driver, randomPassword);
            driver.Password = randomPassword;
            //tenantId
            driver.TenantId = tenantId;
            //user name
            driver.UserName = driver.PhoneNumber;
            //email address
            driver.EmailAddress = driver.PhoneNumber + "@" + tenancyName + ".com";
            //is driver
            driver.IsDriver = true;

            (await UserManager.CreateAsync(driver)).CheckErrors();
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.


            foreach (var documentFileDto in input.CreateOrEditDocumentFileDtos)
            {
                documentFileDto.UserId = driver.Id;
                var documentFile = _objectMapper.Map<DocumentFile>(documentFileDto);
                documentFile.TenantId = tenantId;
                await _documentFileRepository.InsertAsync(documentFile);
            }

        }

        private async Task ProcessImportDriversResultAsync(ImportDriversFromExcelJobArgs args,
            List<ImportDriverDto> invalidDrivers)
        {
            if (invalidDrivers.Any())
            {
                var file = _invalidDriverExporter.ExportToFile(invalidDrivers);
                await _appNotifier.SomeUsersCouldntBeImported(args.User, file.FileToken, file.FileType, file.FileName);
            }
            else
            {
                await _appNotifier.SendMessageAsync(
                    args.User,
                    new LocalizableString("AllDriversSuccessfullyImportedFromExcel,PleaseUploadAllMissingDocuments", TACHYONConsts.LocalizationSourceName),
                    null,
                    Abp.Notifications.NotificationSeverity.Success);
            }
        }

        private void SendInvalidExcelNotification(ImportDriversFromExcelJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                        args.User,
                        new LocalizableString("FileCantBeConvertedToDriverList", TACHYONConsts.LocalizationSourceName),
                        null,
                        Abp.Notifications.NotificationSeverity.Warn));
                }
                uow.Complete();
            }
        }

        private string GetRoleNameFromDisplayName(string displayName, List<Role> roleList)
        {
            return roleList.FirstOrDefault(
                r => r.DisplayName?.ToLowerInvariant() == displayName?.ToLowerInvariant()
            )?.Name;
        }
    }
}