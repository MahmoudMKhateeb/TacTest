using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization.Permissions;
using TACHYON.Authorization.Permissions.Dto;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Authorization.Users.Exporting;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Dto;
using TACHYON.Notifications;
using TACHYON.Organizations.Dto;
using TACHYON.Url;
using static TACHYON.Authorization.Users.Nationalites;

namespace TACHYON.Authorization.Users
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UserAppService : TACHYONAppServiceBase, IUserAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUserPolicy _userPolicy;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly UserManager _userManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        public UserAppService(

            IRepository<DocumentFile, Guid> documentFileRepository,
            IRepository<DocumentType, long> documentTypeRepository,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IUserPolicy userPolicy,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRoleManagementConfig roleManagementConfig,
            UserManager userManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository, DocumentFilesAppService documentFilesAppService)
        {
            _documentFileRepository = documentFileRepository;
            _documentTypeRepository = documentTypeRepository;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _userPolicy = userPolicy;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _organizationUnitRepository = organizationUnitRepository;
            _roleManagementConfig = roleManagementConfig;
            _userManager = userManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _documentFilesAppService = documentFilesAppService;
            _roleRepository = roleRepository;

            AppUrlService = NullAppUrlService.Instance;
        }

        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var userCount = await query.CountAsync();
            var users = new List<User>();
            var userListDtos = new List<UserListDto>();
            if (input.OnlyDrivers)
            {
                var documentTypesCount = await _documentTypeRepository.GetAll()
                .Where(a => a.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver).CountAsync();

                userListDtos = await query.Select(u => new UserListDto()
                {
                    IsMissingDocumentFiles = documentTypesCount != _documentFileRepository.GetAll().Where(t => t.UserId == u.Id).Count(),
                    EmailAddress = u.EmailAddress,
                    UserName = u.UserName,
                    CreationTime = u.CreationTime,
                    Id = u.Id,
                    IsActive = u.IsActive,
                    IsEmailConfirmed = u.IsEmailConfirmed,
                    Name = u.Name,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePictureId = u.ProfilePictureId,
                    Surname = u.Surname,
                    DateOfBirth = u.DateOfBirth,
                    AccountNumber = u.AccountNumber
                })
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
                await FillRoleNames(userListDtos);

            }
            else if (input.OnlyUsers)
            {
                users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

                userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
                await FillRoleNames(userListDtos);

            }
            return new PagedResultDto<UserListDto>(
              userCount,
              userListDtos
              );


        }


        public async Task<PagedResultDto<DriverListDto>> GetDrivers(GetDriversInput input)
        {
            var query = UserManager.Users
                .Where(u => u.IsDriver)
                .ProjectTo<DriverListDto>(AutoMapperConfigurationProvider);

            var result = await LoadResultAsync(query, input.LoadOptions);
            await FillIsMissingDocumentFiles(result);
            return result;
        }
        private async Task FillIsMissingDocumentFiles(PagedResultDto<DriverListDto> pagedResultDto)
        {
            var ids = pagedResultDto.Items.Select(x => x.Id);
            var documentTypesCount = await _documentTypeRepository.GetAll()
                .Where(doc => doc.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver)
                .Where(x => x.IsRequired)
                .CountAsync();

            var submittedDocuments = await (_documentFileRepository.GetAll()
                    .Where(x => ids.Contains((long)x.UserId))
                    .Where(x => x.DocumentTypeFk.IsRequired)
                    .GroupBy(x => x.UserId)
                    .Select(x => new { TruckId = x.Key, IsMissingDocumentFiles = x.Count() == documentTypesCount }))
                .ToListAsync();

            foreach (DriverListDto driverListDto in pagedResultDto.Items)
            {
                if (submittedDocuments != null)
                {
                    driverListDto.IsMissingDocumentFiles = submittedDocuments
                        .FirstOrDefault(x => x.TruckId == driverListDto.Id).IsMissingDocumentFiles;
                }
            }
        }


        public async Task<FileDto> GetUsersToExcel(GetUsersToExcelInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var users = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>()
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

                var allRolesOfUsersOrganizationUnits = GetAllRoleNamesOfUsersOrganizationUnits(input.Id.Value);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                    userRoleDto.InheritedFromOrganizationUnit = allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
                }
            }

            return output;
        }

        private List<string> GetAllRoleNamesOfUsersOrganizationUnits(long userId)
        {
            return (from userOu in _userOrganizationUnitRepository.GetAll()
                    join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                        .OrganizationUnitId
                    join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                    where userOu.UserId == userId
                    select userOuRoles.Name).ToList();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            CheckIfDriverPhoneNumberExists(input);

            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Unlock)]
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());

            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)

            CheckErrors(await UserManager.UpdateAsync(user));

            if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            //Update roles
            CheckErrors(await UserManager.SetRolesAsync(user, input.AssignedRoleNames));

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;

            //required Docs
            if (input.User.IsDriver)
            {
                //get requiredDocs
                var requiredDocs = await _documentFilesAppService.GetDriverRequiredDocumentFiles("");
                if (requiredDocs.Count > 0)
                {
                    foreach (var item in requiredDocs)
                    {
                        var doc = input.CreateOrEditDocumentFileDtos.FirstOrDefault(x => x.DocumentTypeId == item.DocumentTypeId);

                        if (doc.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
                        {
                            throw new UserFriendlyException(L("document missing msg :" + item.Name));
                        }

                        doc.Name = item.Name;
                    }
                }

            }


            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }

                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.


            //save docs if is a driver
            if (input.User.IsDriver)
            {
                foreach (var item in input.CreateOrEditDocumentFileDtos)
                {
                    item.UserId = user.Id;
                    item.Name = item.Name + "_" + user.Id.ToString();
                    await _documentFilesAppService.CreateOrEdit(item);
                }
            }


            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }

        }

        private async Task FillRoleNames(IReadOnlyCollection<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */
            var userIds = userListDtos.Select(u => u.Id);

            var userRoles = await _userRoleRepository.GetAll()
                .Where(userRole => userIds.Contains(userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                {
                    roleNames[roleId] = role.DisplayName;
                }
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    if (roleNames.ContainsKey(userListRoleDto.RoleId))
                    {
                        userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                    }
                }

                userListDto.Roles = userListDto.Roles.Where(r => r.RoleName != null).OrderBy(r => r.RoleName).ToList();
            }
        }

        private IQueryable<User> GetUsersFilteredQuery(IGetUsersInput input)
        {
            var query = UserManager.Users
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(input.OnlyLockedUsers, u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
                .WhereIf(input.OnlyDrivers, u => u.IsDriver)
                .WhereIf(input.OnlyUsers, u => u.IsDriver == false)
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (input.Permissions != null && input.Permissions.Any(p => !p.IsNullOrWhiteSpace()))
            {
                var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                    r => r.GrantAllPermissionsByDefault &&
                         r.Side == AbpSession.MultiTenancySide
                ).Select(r => r.RoleName).ToList();

                input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

                query = from user in query
                        join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                        from ur in urJoined.DefaultIfEmpty()
                        join urr in _roleRepository.GetAll() on ur.RoleId equals urr.Id into urrJoined
                        from urr in urrJoined.DefaultIfEmpty()
                        join up in _userPermissionRepository.GetAll()
                            .Where(userPermission => input.Permissions.Contains(userPermission.Name)) on user.Id equals up.UserId into upJoined
                        from up in upJoined.DefaultIfEmpty()
                        join rp in _rolePermissionRepository.GetAll()
                            .Where(rolePermission => input.Permissions.Contains(rolePermission.Name)) on
                            new { RoleId = ur == null ? 0 : ur.RoleId } equals new { rp.RoleId } into rpJoined
                        from rp in rpJoined.DefaultIfEmpty()
                        where (up != null && up.IsGranted) ||
                              (up == null && rp != null && rp.IsGranted) ||
                              (up == null && rp == null && staticRoleNames.Contains(urr.Name))
                        select user;
            }

            return query;
        }

        public async Task<bool> CheckIfPhoneNumberValid(string phoneNumber, long? driverId)
        {
            var result = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && x.Id != driverId);
            return (result == null);
        }

        public List<SelectItemDto> GetDriverNationalites()
        {
            var nationalites = new List<SelectItemDto>();
            nationalites.Add(new SelectItemDto("Afghanistan", "Afghanistan"));
            nationalites.Add(new SelectItemDto("Albanians", "Albanians"));
            nationalites.Add(new SelectItemDto("Algerians", "Algerians"));
            nationalites.Add(new SelectItemDto("Americans", "Americans"));
            nationalites.Add(new SelectItemDto("Andorrans", "Andorrans"));
            nationalites.Add(new SelectItemDto("Angolans", "Angolans"));
            nationalites.Add(new SelectItemDto("AntiguansandBarbudans", "AntiguansandBarbudans"));
            nationalites.Add(new SelectItemDto("Argentines", "Argentines"));
            nationalites.Add(new SelectItemDto("Armenians", "Armenians"));
            nationalites.Add(new SelectItemDto("Arubans", "Arubans"));
            nationalites.Add(new SelectItemDto("Australians", "Australians"));
            nationalites.Add(new SelectItemDto("Austrians", "Austrians"));
            nationalites.Add(new SelectItemDto("Azerbaijanis", "Azerbaijanis"));
            nationalites.Add(new SelectItemDto("Bahamians", "Bahamians"));
            nationalites.Add(new SelectItemDto("Bahrainis", "Bahrainis"));
            nationalites.Add(new SelectItemDto("Bangladeshis", "Bangladeshis"));
            nationalites.Add(new SelectItemDto("Barbadians", "Barbadians"));
            nationalites.Add(new SelectItemDto("Basques", "Basques"));
            nationalites.Add(new SelectItemDto("Belarusians", "Belarusians"));
            nationalites.Add(new SelectItemDto("Belgians", "Belgians"));
            nationalites.Add(new SelectItemDto("Belizeans", "Belizeans"));
            nationalites.Add(new SelectItemDto("Beninese", "Beninese"));
            nationalites.Add(new SelectItemDto("Bermudians", "Bermudians"));
            nationalites.Add(new SelectItemDto("Bhutanese", "Bhutanese"));
            nationalites.Add(new SelectItemDto("Bolivians", "Bolivians"));
            nationalites.Add(new SelectItemDto("Bosniaks", "Bosniaks"));
            nationalites.Add(new SelectItemDto("BosniansandHerzegovinians", "BosniansandHerzegovinians"));
            nationalites.Add(new SelectItemDto("Botswana", "Botswana"));
            nationalites.Add(new SelectItemDto("Brazilians", "Brazilians"));
            nationalites.Add(new SelectItemDto("Bretons", "Bretons"));
            nationalites.Add(new SelectItemDto("British", "British"));
            nationalites.Add(new SelectItemDto("BritishVirginIslanders", "BritishVirginIslanders"));
            nationalites.Add(new SelectItemDto("Bruneians", "Bruneians"));
            nationalites.Add(new SelectItemDto("Bulgarians", "Bulgarians"));
            nationalites.Add(new SelectItemDto("MacedonianBulgarians", "MacedonianBulgarians"));
            nationalites.Add(new SelectItemDto("Burkinabés", "Burkinabés"));
            nationalites.Add(new SelectItemDto("Burmese", "Burmese"));
            nationalites.Add(new SelectItemDto("Burundians", "Burundians"));
            nationalites.Add(new SelectItemDto("Cambodians", "Cambodians"));
            nationalites.Add(new SelectItemDto("Cameroonians", "Cameroonians"));
            nationalites.Add(new SelectItemDto("Canadians", "Canadians"));
            nationalites.Add(new SelectItemDto("Catalans", "Catalans"));
            nationalites.Add(new SelectItemDto("CapeVerdeans", "CapeVerdeans"));
            nationalites.Add(new SelectItemDto("Chaldeans", "Chaldeans"));
            nationalites.Add(new SelectItemDto("Chadians", "Chadians"));
            nationalites.Add(new SelectItemDto("Chileans", "Chileans"));
            nationalites.Add(new SelectItemDto("Chinese", "Chinese"));
            nationalites.Add(new SelectItemDto("Colombians", "Colombians"));
            nationalites.Add(new SelectItemDto("Comorians", "Comorians"));
            nationalites.Add(new SelectItemDto("Congolese(DRC)", "Congolese(DRC)"));
            nationalites.Add(new SelectItemDto("Congolese(RotC)", "Congolese(RotC)"));
            nationalites.Add(new SelectItemDto("CostaRicans", "CostaRicans"));
            nationalites.Add(new SelectItemDto("Croats", "Croats"));
            nationalites.Add(new SelectItemDto("Cubans", "Cubans"));
            nationalites.Add(new SelectItemDto("Cypriots", "Cypriots"));
            nationalites.Add(new SelectItemDto("Czechs", "Czechs"));
            nationalites.Add(new SelectItemDto("Danes", "Danes"));
            nationalites.Add(new SelectItemDto("Greenlanders", "Greenlanders"));
            nationalites.Add(new SelectItemDto("Djiboutians", "Djiboutians"));
            nationalites.Add(new SelectItemDto("Dominicans(Commonwealth)", "Dominicans(Commonwealth)"));
            nationalites.Add(new SelectItemDto("Dominicans(Republic)", "Dominicans(Republic)"));
            nationalites.Add(new SelectItemDto("Dutch", "Dutch"));
            nationalites.Add(new SelectItemDto("EastTimorese", "EastTimorese"));
            nationalites.Add(new SelectItemDto("Ecuadorians", "Ecuadorians"));
            nationalites.Add(new SelectItemDto("Egyptians", "Egyptians"));
            nationalites.Add(new SelectItemDto("Emiratis", "Emiratis"));
            nationalites.Add(new SelectItemDto("English", "English"));
            nationalites.Add(new SelectItemDto("Equatoguineans", "Equatoguineans"));
            nationalites.Add(new SelectItemDto("Eritreans", "Eritreans"));
            nationalites.Add(new SelectItemDto("Estonians", "Estonians"));
            nationalites.Add(new SelectItemDto("Ethiopians", "Ethiopians"));
            nationalites.Add(new SelectItemDto("FalklandIslanders", "FalklandIslanders"));
            nationalites.Add(new SelectItemDto("Faroese", "Faroese"));
            nationalites.Add(new SelectItemDto("Fijians", "Fijians"));
            nationalites.Add(new SelectItemDto("Finns", "Finns"));
            nationalites.Add(new SelectItemDto("FinnishSwedish", "FinnishSwedish"));
            nationalites.Add(new SelectItemDto("Filipinos", "Filipinos"));
            nationalites.Add(new SelectItemDto("Frenchcitizens", "Frenchcitizens"));
            nationalites.Add(new SelectItemDto("Gabonese", "Gabonese"));
            nationalites.Add(new SelectItemDto("Gambians", "Gambians"));
            nationalites.Add(new SelectItemDto("Georgians", "Georgians"));
            nationalites.Add(new SelectItemDto("Germans", "Germans"));
            nationalites.Add(new SelectItemDto("BalticGermans", "BalticGermans"));
            nationalites.Add(new SelectItemDto("Ghanaians", "Ghanaians"));
            nationalites.Add(new SelectItemDto("Gibraltarians", "Gibraltarians"));
            nationalites.Add(new SelectItemDto("Greeks", "Greeks"));
            nationalites.Add(new SelectItemDto("GreekMacedonians", "GreekMacedonians"));
            nationalites.Add(new SelectItemDto("Grenadians", "Grenadians"));
            nationalites.Add(new SelectItemDto("Guatemalans", "Guatemalans"));
            nationalites.Add(new SelectItemDto("Guianese(French)", "Guianese(French)"));
            nationalites.Add(new SelectItemDto("Guineans", "Guineans"));
            nationalites.Add(new SelectItemDto("Guinea-Bissaunationals", "Guinea-Bissaunationals"));
            nationalites.Add(new SelectItemDto("Guyanese", "Guyanese"));
            nationalites.Add(new SelectItemDto("Haitians", "Haitians"));
            nationalites.Add(new SelectItemDto("Hondurans", "Hondurans"));
            nationalites.Add(new SelectItemDto("HongKongers", "HongKongers"));
            nationalites.Add(new SelectItemDto("Hungarians", "Hungarians"));
            nationalites.Add(new SelectItemDto("Icelanders", "Icelanders"));
            nationalites.Add(new SelectItemDto("I-Kiribati", "I-Kiribati"));
            nationalites.Add(new SelectItemDto("Indians", "Indians"));
            nationalites.Add(new SelectItemDto("Indonesians", "Indonesians"));
            nationalites.Add(new SelectItemDto("Iranians", "Iranians"));
            nationalites.Add(new SelectItemDto("Iraqis", "Iraqis"));
            nationalites.Add(new SelectItemDto("Irish", "Irish"));
            nationalites.Add(new SelectItemDto("Italians", "Italians"));
            nationalites.Add(new SelectItemDto("Ivoirians", "Ivoirians"));
            nationalites.Add(new SelectItemDto("Jamaicans", "Jamaicans"));
            nationalites.Add(new SelectItemDto("Japanese", "Japanese"));
            nationalites.Add(new SelectItemDto("Jordanians", "Jordanians"));
            nationalites.Add(new SelectItemDto("Kazakhs", "Kazakhs"));
            nationalites.Add(new SelectItemDto("Kenyans", "Kenyans"));
            nationalites.Add(new SelectItemDto("Koreans", "Koreans"));
            nationalites.Add(new SelectItemDto("Kosovars", "Kosovars"));
            nationalites.Add(new SelectItemDto("Kuwaitis", "Kuwaitis"));
            nationalites.Add(new SelectItemDto("Kyrgyzs", "Kyrgyzs"));
            nationalites.Add(new SelectItemDto("Lao", "Lao"));
            nationalites.Add(new SelectItemDto("Latvians", "Latvians"));
            nationalites.Add(new SelectItemDto("Lebanese", "Lebanese"));
            nationalites.Add(new SelectItemDto("Liberians", "Liberians"));
            nationalites.Add(new SelectItemDto("Libyans", "Libyans"));
            nationalites.Add(new SelectItemDto("Liechtensteiners", "Liechtensteiners"));
            nationalites.Add(new SelectItemDto("Lithuanians", "Lithuanians"));
            nationalites.Add(new SelectItemDto("Luxembourgers", "Luxembourgers"));
            nationalites.Add(new SelectItemDto("Macao", "Macao"));
            nationalites.Add(new SelectItemDto("Macedonians", "Macedonians"));
            nationalites.Add(new SelectItemDto("Malagasy", "Malagasy"));
            nationalites.Add(new SelectItemDto("Malawians", "Malawians"));
            nationalites.Add(new SelectItemDto("Malaysians", "Malaysians"));
            nationalites.Add(new SelectItemDto("Maldivians", "Maldivians"));
            nationalites.Add(new SelectItemDto("Malians", "Malians"));
            nationalites.Add(new SelectItemDto("Maltese", "Maltese"));
            nationalites.Add(new SelectItemDto("Manx", "Manx"));
            nationalites.Add(new SelectItemDto("Marshallese", "Marshallese"));
            nationalites.Add(new SelectItemDto("Mauritanians", "Mauritanians"));
            nationalites.Add(new SelectItemDto("Mauritians", "Mauritians"));
            nationalites.Add(new SelectItemDto("Mexicans", "Mexicans"));
            nationalites.Add(new SelectItemDto("Micronesians", "Micronesians"));
            nationalites.Add(new SelectItemDto("Moldovans", "Moldovans"));
            nationalites.Add(new SelectItemDto("Monégasque", "Monégasque"));
            nationalites.Add(new SelectItemDto("Mongolians", "Mongolians"));
            nationalites.Add(new SelectItemDto("Montenegrins", "Montenegrins"));
            nationalites.Add(new SelectItemDto("Moroccans", "Moroccans"));
            nationalites.Add(new SelectItemDto("Mozambicans", "Mozambicans"));
            nationalites.Add(new SelectItemDto("Namibians", "Namibians"));
            nationalites.Add(new SelectItemDto("Nauruans", "Nauruans"));
            nationalites.Add(new SelectItemDto("Nepalese", "Nepalese"));
            nationalites.Add(new SelectItemDto("new Zealanders", "new Zealanders"));
            nationalites.Add(new SelectItemDto("Nicaraguans", "Nicaraguans"));
            nationalites.Add(new SelectItemDto("Nigeriens", "Nigeriens"));
            nationalites.Add(new SelectItemDto("Nigerians", "Nigerians"));
            nationalites.Add(new SelectItemDto("Norwegians", "Norwegians"));
            nationalites.Add(new SelectItemDto("Omani", "Omani"));
            nationalites.Add(new SelectItemDto("Pakistanis", "Pakistanis"));
            nationalites.Add(new SelectItemDto("Palauans", "Palauans"));
            nationalites.Add(new SelectItemDto("Palestinians", "Palestinians"));
            nationalites.Add(new SelectItemDto("Panamanians", "Panamanians"));
            nationalites.Add(new SelectItemDto("Papuanew Guineans", "Papuanew Guineans"));
            nationalites.Add(new SelectItemDto("Paraguayans", "Paraguayans"));
            nationalites.Add(new SelectItemDto("Peruvians", "Peruvians"));
            nationalites.Add(new SelectItemDto("Poles", "Poles"));
            nationalites.Add(new SelectItemDto("Portuguese", "Portuguese"));
            nationalites.Add(new SelectItemDto("PuertoRicans", "PuertoRicans"));
            nationalites.Add(new SelectItemDto("Qatari", "Qatari"));
            nationalites.Add(new SelectItemDto("Quebecers", "Quebecers"));
            nationalites.Add(new SelectItemDto("Réunionnais", "Réunionnais"));
            nationalites.Add(new SelectItemDto("Romanians", "Romanians"));
            nationalites.Add(new SelectItemDto("Russians", "Russians"));
            nationalites.Add(new SelectItemDto("BalticRussians", "BalticRussians"));
            nationalites.Add(new SelectItemDto("Rwandans", "Rwandans"));
            nationalites.Add(new SelectItemDto("SaintKittsandNevis", "SaintKittsandNevis"));
            nationalites.Add(new SelectItemDto("SaintLucians", "SaintLucians"));
            nationalites.Add(new SelectItemDto("Salvadorans", "Salvadorans"));
            nationalites.Add(new SelectItemDto("Sammarinese", "Sammarinese"));
            nationalites.Add(new SelectItemDto("Samoans", "Samoans"));
            nationalites.Add(new SelectItemDto("SãoToméandPríncipe", "SãoToméandPríncipe"));
            nationalites.Add(new SelectItemDto("Saudis", "Saudis"));
            nationalites.Add(new SelectItemDto("Scots", "Scots"));
            nationalites.Add(new SelectItemDto("Senegalese", "Senegalese"));
            nationalites.Add(new SelectItemDto("Serbs", "Serbs"));
            nationalites.Add(new SelectItemDto("Seychellois", "Seychellois"));
            nationalites.Add(new SelectItemDto("SierraLeoneans", "SierraLeoneans"));
            nationalites.Add(new SelectItemDto("Singaporeans", "Singaporeans"));
            nationalites.Add(new SelectItemDto("Slovaks", "Slovaks"));
            nationalites.Add(new SelectItemDto("Slovenes", "Slovenes"));
            nationalites.Add(new SelectItemDto("SolomonIslanders", "SolomonIslanders"));
            nationalites.Add(new SelectItemDto("Somalis", "Somalis"));
            nationalites.Add(new SelectItemDto("Somalilanders", "Somalilanders"));
            nationalites.Add(new SelectItemDto("Sotho", "Sotho"));
            nationalites.Add(new SelectItemDto("SouthAfricans", "SouthAfricans"));
            nationalites.Add(new SelectItemDto("Spaniards", "Spaniards"));
            nationalites.Add(new SelectItemDto("SriLankans", "SriLankans"));
            nationalites.Add(new SelectItemDto("Sudanese", "Sudanese"));
            nationalites.Add(new SelectItemDto("Surinamese", "Surinamese"));
            nationalites.Add(new SelectItemDto("Swazi", "Swazi"));
            nationalites.Add(new SelectItemDto("Swedes", "Swedes"));
            nationalites.Add(new SelectItemDto("Swiss", "Swiss"));
            nationalites.Add(new SelectItemDto("Syriacs", "Syriacs"));
            nationalites.Add(new SelectItemDto("Syrians", "Syrians"));
            nationalites.Add(new SelectItemDto("Taiwanese", "Taiwanese"));
            nationalites.Add(new SelectItemDto("Tamils", "Tamils"));
            nationalites.Add(new SelectItemDto("Tajik", "Tajik"));
            nationalites.Add(new SelectItemDto("Tanzanians", "Tanzanians"));
            nationalites.Add(new SelectItemDto("Thais", "Thais"));
            nationalites.Add(new SelectItemDto("Tibetans", "Tibetans"));
            nationalites.Add(new SelectItemDto("Tobagonians", "Tobagonians"));
            nationalites.Add(new SelectItemDto("Togolese", "Togolese"));
            nationalites.Add(new SelectItemDto("Tongans", "Tongans"));
            nationalites.Add(new SelectItemDto("Trinidadians", "Trinidadians"));
            nationalites.Add(new SelectItemDto("Tunisians", "Tunisians"));
            nationalites.Add(new SelectItemDto("Turks", "Turks"));
            nationalites.Add(new SelectItemDto("Tuvaluans", "Tuvaluans"));
            nationalites.Add(new SelectItemDto("Ugandans", "Ugandans"));
            nationalites.Add(new SelectItemDto("Ukrainians", "Ukrainians"));
            nationalites.Add(new SelectItemDto("Uruguayans", "Uruguayans"));
            nationalites.Add(new SelectItemDto("Uzbeks", "Uzbeks"));
            nationalites.Add(new SelectItemDto("Vanuatuans", "Vanuatuans"));
            nationalites.Add(new SelectItemDto("Venezuelans", "Venezuelans"));
            nationalites.Add(new SelectItemDto("Vietnamese", "Vietnamese"));
            nationalites.Add(new SelectItemDto("Vincentians", "Vincentians"));
            nationalites.Add(new SelectItemDto("Welsh", "Welsh"));
            nationalites.Add(new SelectItemDto("Yemenis", "Yemenis"));
            nationalites.Add(new SelectItemDto("Zambians", "Zambians"));
            nationalites.Add(new SelectItemDto("Zimbabweans", "Zimbabweans"));



            return nationalites;
        }

        public async Task<bool> CheckIfUserNameValid(string userName, long? userId)
        {
            var result = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName && x.Id != userId);
            return (result == null);
        }
        public async Task<bool> CheckIfEmailisAvailable(string email)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                var result = await UserManager.FindByEmailAsync(email);
                if (result == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private void CheckIfDriverPhoneNumberExists(CreateOrUpdateUserInput input)
        {
            if (input.User.IsDriver)
            {
                DisableTenancyFilters();
                var userDB = _userManager.Users.FirstOrDefault(x => x.IsDriver == true && x.PhoneNumber == input.User.PhoneNumber && x.Id != input.User.Id);
                if (userDB != null)
                {
                    throw new UserFriendlyException(L("DriverPhoneNumberAlreadyExists"));
                }
            }
        }
    }

    public class GetDriversInput
    {
        public string LoadOptions { get; set; }
    }
}