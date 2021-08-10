using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TACHYON.Chat;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Editions;
using TACHYON.Localization;
using TACHYON.MultiTenancy;
using TACHYON.Net.Emailing;

namespace TACHYON.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : TACHYONServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISettingManager _settingManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;

        // used for styling action links on email messages.
        private string _emailButtonStyle =
                "padding-left: 30px; padding-right: 30px; padding-top: 12px; padding-bottom: 12px; color: #ffffff; background-color: #d82631; font-size: 14pt; text-decoration: none;";
        private string _emailButtonColor = "#d82631";

        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            EditionManager editionManager,
            UserManager userManager)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _editionManager = editionManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("CompanyFullLeagalName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + L("Email") + "</b>: " + user.EmailAddress + "<br />");

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("Password") + "</b>: " + plainPassword + "<br />");
            }

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + L("Verify") + "</a>");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
            mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");

            await ReplaceBodyAndSend(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Send Email to tenant when approve all documents and eligible to use platform
        /// </summary>
        /// <param name="hostAdminEmailAddresses"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        [UnitOfWork]
        public virtual async Task SendAllApprovedDocumentsAsyn(Tenant tenant)
        {
            var adminUser=await _userManager.GetAdminByTenantIdAsync(tenant.Id);
            var mailMessage = new StringBuilder();
            var tenantItem = await _tenantRepository.GetAsync(tenant.Id);
            var emailTemplate = GetTitleAndSubTitle(tenant.Id, L("ApprovedDocuments_Title"), L("ApprovedDocuments_SubTitle"));

            mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>:" + tenantItem.TenancyName);
            mailMessage.AppendLine("<b>" + L("CompanyName") + "</b>:" + tenantItem.companyName);
            mailMessage.AppendLine("<b>" + L("Address") + "</b>:" + tenantItem.Address);
            
            mailMessage.AppendLine(L("All your documents have been approved, you can now use platform message"));

            await ReplaceBodyAndSend(adminUser.EmailAddress, L("DocumentsApproved"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Send Email to tenant when approve all documents and eligible to use platform
        /// </summary>
        /// <param name="hostAdminEmailAddresses"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        [UnitOfWork]
        public virtual async Task SendExpiredDateDocumentsAsyn(Tenant tenant,string documentFileName)
        {
            var adminUser = await _userManager.GetAdminByTenantIdAsync(tenant.Id);
            var mailMessage = new StringBuilder();
            var tenantItem = await _tenantRepository.GetAsync(tenant.Id);
            var emailTemplate = GetTitleAndSubTitle(tenant.Id, L("ExpiredDateDocuments_Title"), L("ExpiredDateDocuments_SubTitle"));

            mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>:" + tenantItem.TenancyName);
            mailMessage.AppendLine("<b>" + L("CompanyName") + "</b>:" + tenantItem.companyName);
            mailMessage.AppendLine("<b>" + L("Address") + "</b>:" + tenantItem.Address);

            mailMessage.AppendLine(L(String.Format("Document File: {0} has been expired message", documentFileName)));

            await ReplaceBodyAndSend(adminUser.EmailAddress, L("ExpiredDocument"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Send Email to tenant when approve all documents and eligible to use platform
        /// </summary>
        /// <param name="file"></param>
        /// /// <param name="tenantId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public virtual async Task SendDocumentsExpiredInfoAsyn(List<DocumentFile> files, int tenantId)
        {
            var adminUser = await _userManager.GetAdminByTenantIdAsync(tenantId);
            var mailMessage = new StringBuilder();
            var tenantItem = await _tenantRepository.GetAsync(tenantId);
            var emailTemplate = GetTitleAndSubTitle(tenantId, L("DocumentsExpiredInfo_Title"), L("DocumentsExpiredInfo_SubTitle"));

            mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>:" + tenantItem.TenancyName);
            mailMessage.AppendLine("<b>" + L("CompanyName") + "</b>:" + tenantItem.companyName);
            mailMessage.AppendLine("<b>" + L("Address") + "</b>:" + tenantItem.Address + "<br/>");

            //Truck table
            //If exists Truck files
            if (files.Any(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Truck ))
            {
                //bind html Trucks table
                BindTruckFilesTable(mailMessage, 
                    files.Where(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Truck &&
                    x.TruckFk != null).ToList());
            }

            //Driver table
            //If exists driver files
            else if (files.Any(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver))
            {
                //bind html driver table
                BindDriverFilesTable(mailMessage, 
                    files.Where(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver && 
                    x.UserFk != null).ToList());
            }


            await ReplaceBodyAndSend(adminUser.EmailAddress, L("DocumentsExpiredInfo"), emailTemplate, mailMessage);
        }

        


        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link = null)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");
            }

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("PasswordResetEmail_Title"), L("PasswordResetEmail_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("CompanyFullLeagalName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                link = EncryptQueryParameters(link);

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + L("Reset") + "</a>");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
                mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");
            }

            await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
        }

        public async Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("NewChatMessageEmail_Title"), L("NewChatMessageEmail_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername + "<br />");
                mailMessage.AppendLine("<b>" + L("Time") + "</b>: " + chatMessage.CreationTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + " UTC<br />");
                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpire_Email_Body", culture, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionAssignedToAnotherEmail(int tenantId, DateTime utcNow, int expiringEditionId)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var expringEdition = await _editionManager.GetByIdAsync(expiringEditionId);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionAssignedToAnother_Email_Body", culture, expringEdition.DisplayName, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                var hostAdmin = await _userManager.GetAdminAsync();
                if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
                {
                    return;
                }

                var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, hostAdmin.TenantId, hostAdmin.Id);
                var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                var emailTemplate = GetTitleAndSubTitle(null, L("FailedSubscriptionTerminations_Title"), L("FailedSubscriptionTerminations_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("FailedSubscriptionTerminations_Email_Body", culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(hostAdmin.EmailAddress, L("FailedSubscriptionTerminations_Email_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionExpiringSoonEmail(int tenantId, DateTime dateToCheckRemainingDayCount)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var tenantAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(tenantAdminLanguage);

                        var emailTemplate = GetTitleAndSubTitle(null, L("SubscriptionExpiringSoon_Title"), L("SubscriptionExpiringSoon_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpiringSoon_Email_Body", culture, dateToCheckRemainingDayCount.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpiringSoon_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate, StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            await _emailSender.SendAsync(new MailMessage
            {
                To = { emailAddress },
                Subject = subject,
                Body = emailTemplate.ToString(),
                IsBodyHtml = true
            });
        }

        /// <summary>
        /// Returns link with encrypted parameters
        /// </summary>
        /// <param name="link"></param>
        /// <param name="encrptedParameterName"></param>
        /// <returns></returns>
        private string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = link.Substring(link.IndexOf('?')).TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" + HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        /// <summary>
        /// Bind Trucks Documents html
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <param name="files"></param>
        private void BindTruckFilesTable(StringBuilder mailMessage, List<DocumentFile> files)
        {
            mailMessage.AppendLine("<b>" + L("TruckDocuments") + "</b>");
            mailMessage.AppendLine("<table style=\"border-collapse: collapse; border: 1px solid black;\"> " +
                "<tr>" +
                " <th style=\"border: 1px solid black;  \">" + L("PlateNumber") + "</th> " +
                "<th style=\"border: 1px solid black;  \"> " + L("DocumentName") + " </th> " +
                "<th style=\"border: 1px solid black;  \"> " + L("ExpiredStatus") + "</th>" +
                " <th style=\"border: 1px solid black;  \"> " + L("ExpiredDate") + " </th>" +
                " </tr>");

            foreach (var file in files)
            {
                var expiredStatus = file.ExpirationDate != null ? (file.ExpirationDate.Value.Date < DateTime.Now.Date ? L("Expired") : L("Active")) : "Active";
                //var documentType = file.TruckId != null ? L("Truck") : L("Driver");

                mailMessage.AppendLine("<tr>" +
                    "<td style=\"border: 1px solid black;  \">" + file.TruckFk.PlateNumber + "</td>" +
                    " <td style=\"border: 1px solid black;  \">" + file.Name + " </td> " +
                    "<td style=\"border: 1px solid black;  \">" + expiredStatus + " </td>" +
                    " <td style=\"border: 1px solid black;  \"> " + file.ExpirationDate + "</td>" +
                    " </tr>");
            }

            mailMessage.AppendLine("</table> <br/>");
        }

        /// <summary>
        /// Bind Driver documents html
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <param name="files"></param>
        private void BindDriverFilesTable(StringBuilder mailMessage, List<DocumentFile> files)
        {
            mailMessage.AppendLine("<b>" + L("DriverDocuments") + "</b>");
            mailMessage.AppendLine("<table style=\"border-collapse: collapse; border: 1px solid black;\"> " +
                "<tr>" +
                " <th style=\"border: 1px solid black;  \">" + L("DriverName") + "</th> " +
                "<th style=\"border: 1px solid black;  \"> " + L("DocumentName") + " </th> " +
                "<th style=\"border: 1px solid black;  \"> " + L("ExpiredStatus") + "</th>" +
                " <th style=\"border: 1px solid black;  \"> " + L("ExpiredDate") + " </th>" +
                " </tr>");

            foreach (var file in files)
            {
                var expiredStatus = file.ExpirationDate != null ? (file.ExpirationDate.Value.Date < DateTime.Now.Date ? L("Expired") : L("Active")) : "Active";
                //var documentType = file.TruckId != null ? L("Truck") : L("Driver");

                mailMessage.AppendLine("<tr>" +
                    "<td style=\"border: 1px solid black;  \">" + file.UserFk.Name + "</td>" +
                    " <td style=\"border: 1px solid black;  \">" + file.Name + " </td> " +
                    "<td style=\"border: 1px solid black;  \">" + expiredStatus + " </td>" +
                    " <td style=\"border: 1px solid black;  \"> " + file.ExpirationDate + "</td>" +
                    " </tr>");
            }

            mailMessage.AppendLine("</table>");
        }
    }
}