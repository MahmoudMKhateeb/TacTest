using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TACHYON.Chat;
using TACHYON.Documents;
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
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IRepository<User, long> _lookupUserRepository;

        private bool isRTL = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            EditionManager editionManager,
            UserManager userManager,
            IRepository<User, long> lookupUserRepository)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _lookupUserRepository = lookupUserRepository;
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="password">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string password)
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
            var emailTemplate = await GetEmailTemplate(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var mailMessage = new StringBuilder("<div class=\"data\"><ul>");
            mailMessage.AppendLine($"<li><span class=\"first\">{L("Name")}</span><span class=\"last\">{user.FullName}</span></li>");
            mailMessage.AppendLine($"<li><span class=\"first\">{L("CompanyNameEmailTemplate")}</span><span class=\"last\">{tenancyName}</span></li>");
            mailMessage.AppendLine($"<li><span class=\"first\">{L("Email")}</span><span class=\"last\">{user.EmailAddress}</span></li>");
            mailMessage.AppendLine($"<li><span class=\"first\">{L("Password")}</span><span class=\"last\">{password}</span></li>");
            mailMessage.AppendLine($"</ul></div><p class=\"lead\">{L("ClickButtonMessage")}</p>");
            mailMessage.AppendLine($"<button onclick=\"location.href='{link}';\" class=\"btn btn-red\">{L("Verify")}</button>");
            mailMessage.AppendLine($"<p class=\"lead\">{L("CopyLinkMessage")}</p>");
            mailMessage.AppendLine($"<p class=\"lead\">{link}</p>");
            await ReplaceBodyAndSend(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Send Email to tenant when approve all documents and eligible to use platform
        /// </summary>
        /// <param name="loginLink"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [UnitOfWork]
        public virtual async Task SendAllApprovedDocumentsAsync(int tenantId, string loginLink)
        {
            // You Can Get Login Link From IAppUrlService And Method Name : GetTachyonPlatformLoginUrl()
            var emailTemplate = await GetEmailTemplate(tenantId, L("ApprovedDocuments_Title"));
            var mailMessage = new StringBuilder($"<div class=\"data\"><h2>{L("DearsAt")}");
            mailMessage.AppendLine($" {IntoSpan(await GetCompanyName(tenantId))}</h2></div>");
            mailMessage.AppendLine($"<p class=\"lead\" style=\"width: 65%; margin: 30px auto\">{L("ApprovalDocumentsEmailMessage")}</p>");
            mailMessage.AppendLine($"<a href=\"{loginLink}\" class=\"lead\" target=\"blank\" ");
            mailMessage.AppendLine($"style=\"width: 85%; margin: 30px auto\">{L("LoginLink")}</a>");

            await ReplaceBodyAndSend(await GetTenantAdminEmailAddress(tenantId), L("DocumentsApproved"), emailTemplate, mailMessage);
        }


        [UnitOfWork]
        public virtual async Task SendRejectedDocumentEmail(int tenantId, string documentName, string rejectionReason)
        {
            var emailTemplate = await GetEmailTemplate(null, L("RejectedDocument_Title"));
            var mailMessage = new StringBuilder($"<div class=\"data\"><h2>{L("DearsAt")}");
            mailMessage.AppendLine($" {IntoSpan(await GetCompanyName(tenantId))}</h2></div>");
            mailMessage.AppendLine("<p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine($"{L("RejectedDocumentEmailMessage", IntoSpan(documentName), IntoSpan(rejectionReason))}</p>");

            await ReplaceBodyAndSend(await GetTenantAdminEmailAddress(tenantId), L("RejectedDocument_Title"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Send Email to tenant when approve all documents and eligible to use platform
        /// </summary>
        /// <param name="documentFileName"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        [UnitOfWork]
        public virtual async Task SendExpiredDateDocumentsAsyn(Tenant tenant, string documentFileName)
        {
            var adminUser = await _userManager.GetAdminByTenantIdAsync(tenant.Id);
            var mailMessage = new StringBuilder();
            var tenantItem = await _tenantRepository.GetAsync(tenant.Id);

            var emailTemplate = await GetEmailTemplate(tenant.Id, L("ExpiredDateDocuments_Title"));

            if (isRTL)
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>:" + tenantItem.TenancyName);
                mailMessage.AppendLine("<b>" + L("CompanyNameEmailTemplate") + "</b>:" + tenantItem.companyName);
                mailMessage.AppendLine("<b>" + L("Address") + "</b>:" + tenantItem.Address);
            }
            else
            {
                mailMessage.AppendLine("<b>" + tenantItem.TenancyName + "</b>:" + L("TenancyName"));

                mailMessage.AppendLine("<b>" + tenantItem.companyName + "</b>:" + L("CompanyNameEmailTemplate"));
                mailMessage.AppendLine("<b>" + tenantItem.Address + "</b>:" + L("Address"));

            }
            mailMessage.AppendLine(L(String.Format("Document File: {0} has been expired message", documentFileName)));

            await ReplaceBodyAndSend(adminUser.EmailAddress, L("ExpiredDocument"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Expiration Document (reminder)
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
            var emailTemplate = await GetEmailTemplate(tenantId, L("DocumentsExpirerationreminder_Title"));

            if (isRTL)
            {
                mailMessage.AppendLine("<div class=\"data\"><ul>");
                mailMessage.AppendLine($"<li><span class=\"first\">{L("CompanyNameEmailTemplate")}</span><span class=\"last\">{tenantItem.TenancyName}</span></li>");

                mailMessage.AppendLine($"<li><span class=\"first\">{L("Address")}</span><span class=\"last\">{tenantItem.Address}</span></li><br>");
                mailMessage.AppendLine($"<p class=\"lead\" style=\"width: 65%; margin: 30px auto; text-align:Left\">{L("ReminderDocumentsEmailMessage")}</p>");
            }
            else
            {
                mailMessage.AppendLine("<div class=\"data\"><ul>");
                mailMessage.AppendLine($"<li><span class=\"first\">{tenantItem.TenancyName}</span><span class=\"last\">{L("CompanyNameEmailTemplate")}</span></li>");
                mailMessage.AppendLine($"<li><span class=\"first\">{tenantItem.Address}</span><span class=\"last\">{L("Address")}</span></li><br>");
                mailMessage.AppendLine($"<p class=\"lead\" style=\"width: 65%; margin: 30px auto; text-align:Right\">{L("ReminderDocumentsEmailMessage")}</p>");
            }


            //Truck table
            //If exists Truck files
            if (files.Any(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Truck))
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


            await ReplaceBodyAndSend(adminUser.EmailAddress, L("DocumentsExpiredReminder"), emailTemplate, mailMessage);
        }




        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");

            if (user.TenantId is null) return;
            var emailTemplate = await GetEmailTemplate(user.TenantId,
                L("PasswordResetEmail_Title"));
            var mailMessage = new StringBuilder("<div class=\"data\">");
            var companyName = await GetCompanyName(user.TenantId.Value);

            var passwordRequirements = new string[]
            {
                L("Contain8To12Characters"),
                L("ContainAtLeast1UpperCaseLetter"),
                L("ContainAtLeast1LowerCaseLetter"),
                L("ContainAtLeast1Number"),
            };
            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));
                if (user.TenantId.HasValue)
                    link = link.Replace("{tenantId}", user.TenantId.ToString());

                link = EncryptQueryParameters(link);
            }

            mailMessage.AppendLine($"<h2>{L("DearsAt")} {IntoSpan(companyName)}</h2> <br>");
            mailMessage.AppendLine("<p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine($"{L("YouAreReceivingThisEmailBecauseWe")}</p>");
            mailMessage.AppendLine($"<button onclick=\"location.href='{link}';\" class=\"btn btn-red\">{L("ResetPassword")}</button>");
            mailMessage.AppendLine($"<p class=\"lead\">{L("YourNewPasswordMust")}</p><ul>");
            foreach (string pr in passwordRequirements)
                mailMessage.AppendLine(
                        $"<li><p style=\"font-size: 20px;padding: 10px;margin-bottom: 5px;\">&#8592; {pr}</p></li>");
            mailMessage.AppendLine($"</ul><p class=\"lead\">{L("IfYouDidNotRequestPasswordReset")}</p></div>");

            await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
        }

        public async Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = await GetEmailTemplate(user.TenantId, L("NewChatMessageEmail_Title"));
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

        public async Task SendPasswordUpdatedEmail(int? tenantId, string userEmail, string newPassword)
        {
            var emailTemplate = await GetEmailTemplate(tenantId, L("PasswordUpdated_Title"));
            var mailMessage = new StringBuilder("<div class=\"data\">");
            mailMessage.AppendLine("<p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine($"{L("YourPasswordChangedSuccessfully")} <br><br>");
            mailMessage.AppendLine($"{L("YourNewPasswordIs")} <span style=\"color: #d82631\">{newPassword}</span>");
            mailMessage.AppendLine("</p></div>");

            await ReplaceBodyAndSend(userEmail, L("PasswordUpdated"), emailTemplate, mailMessage);
        }

        public async Task SendWarningSuspendAccountForExpiredDocumentEmail(int tenantId, string documentName,
            DateTime documentExpireDate)
        {
            var emailTemplate = await GetEmailTemplate(tenantId, L("WarningSuspendAccount_Title"));
            var adminEmail = await GetTenantAdminEmailAddress(tenantId);
            var mailMessage = new StringBuilder($"<div class=\"data\"><h2>{L("DearsAt")} {IntoSpan(await GetCompanyName(tenantId))}");
            mailMessage.AppendLine($"</h2></div><p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine(L("DocumentExpirationReminderMessage", IntoSpan(documentName),
                IntoSpan(documentExpireDate.ToString("dd-MM-yyyy"))) + "</p>");

            await ReplaceBodyAndSend(adminEmail, L("WarningSuspendAccount"), emailTemplate, mailMessage);
        }

        //Document Expired
        public async Task SendSuspendedAccountForExpiredDocumentEmail(int tenantId, string documentName)
        {
            var emailTemplate = await GetEmailTemplate(tenantId, L("SuspendedAccountDocumentExpired_Title"));
            var adminEmail = await GetTenantAdminEmailAddress(tenantId);
            var mailMessage = new StringBuilder($"<div class=\"data\"><h2>{L("DearsAt")} {IntoSpan(await GetCompanyName(tenantId))}");
            mailMessage.AppendLine("</h2></div><p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine(L("SuspendedAccountForExpiredDocumentMessage", IntoSpan(documentName)) + "</p>");

            await ReplaceBodyAndSend(adminEmail, L("WarningSuspendAccount"), emailTemplate, mailMessage);
        }

        public async Task SendInvoiceDueEmail(int tenantId, string invoiceNumber, decimal invoiceTotalAmount)
        {
            var emailTemplate = await GetEmailTemplate(tenantId, L("InvoiceDue_Title"));
            var adminEmail = await GetTenantAdminEmailAddress(tenantId);
            var mailMessage = new StringBuilder($"<div class=\"data\"><h2>{L("DearsAt")}");
            mailMessage.AppendLine($" {IntoSpan(await GetCompanyName(tenantId))}</h2></div>");
            mailMessage.AppendLine("<p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine(L("DueInvoiceMessage", IntoSpan(invoiceNumber), IntoSpan(invoiceTotalAmount)));
            await ReplaceBodyAndSend(adminEmail, L("InvoiceDue"), emailTemplate, mailMessage);
        }

        public async Task SendIssuedInvoiceEmail(int tenantId, DateTime invoiceDueDate,
            DateTime invoiceIssueDate, decimal invoiceTotalAmount, string invoiceUrl)
        {
            // You Can Get Invoice Url From IAppUrlService , Method Name: CreateInvoiceDetailsFormat()

            var args = new Object[]
            {
                "<br/>", IntoSpan(invoiceIssueDate.ToString("dd-MM-yyyy")), invoiceTotalAmount,
                IntoSpan(invoiceDueDate.ToString("dd-MM-yyyy")),
                $"<a href=\"{invoiceUrl}\" target=\"blank\" style=\"display: inline;\">{L("ClickHere")}</a>"
            };
            var emailTemplate = await GetEmailTemplate(tenantId, L("IssuedInvoice_Title"));
            var adminEmail = await GetTenantAdminEmailAddress(tenantId);
            var mailMessage = new StringBuilder($"<div class=\"data\"><h2>{L("DearsAt")} {IntoSpan(await GetCompanyName(tenantId))}");
            mailMessage.AppendLine("</h2></div> <p class=\"lead\" style=\"width: 65%; margin: 30px auto\">");
            mailMessage.AppendLine(L("IssuedInvoiceMessage", args) + "</p>");
            await ReplaceBodyAndSend(adminEmail, L("IssuedInvoice"), emailTemplate, mailMessage);
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
                        var emailTemplate = await GetEmailTemplate(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
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
                        var emailTemplate = await GetEmailTemplate(tenantId, L("SubscriptionExpire_Title"));
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
                var emailTemplate = await GetEmailTemplate(null, L("FailedSubscriptionTerminations_Title"), L("FailedSubscriptionTerminations_SubTitle"));
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

                        var emailTemplate = await GetEmailTemplate(null, L("SubscriptionExpiringSoon_Title"), L("SubscriptionExpiringSoon_SubTitle"));
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

        private async Task<StringBuilder> GetEmailTemplate(int? tenantId, string title, string subTitle = null)
        {
            var emailTemplate = new StringBuilder(await _emailTemplateProvider.GetDefaultTemplate(tenantId));
            if (subTitle != null)
                emailTemplate.Replace("<p hidden>{EMAIL_SUB_TITLE}</p>", $"<p>{subTitle}</p>");

            emailTemplate.Replace("{EMAIL_TITLE}", title);
            if (isRTL)
                emailTemplate = emailTemplate.Replace("<body>", "<body style=\"direction: rtl\"> ");
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
        private async Task BindDriverFilesTable(StringBuilder mailMessage, List<DocumentFile> files)
        {
            mailMessage.AppendLine("<b>" + L("DriverDocuments") + "</b>");
            mailMessage.AppendLine("<table style=\"border-collapse: collapse; border: 1px solid black;\"> " +
                "<tr>" +
                " <th style=\"border: 1px solid black;  \">" + L("DriverName") + "</th> " +
                " <th style=\"border: 1px solid black;  \">" + L("DriverId") + "</th> " +
                "<th style=\"border: 1px solid black;  \"> " + L("DocumentName") + " </th> " +
                "<th style=\"border: 1px solid black;  \"> " + L("ExpiredStatus") + "</th>" +
                " <th style=\"border: 1px solid black;  \"> " + L("ExpiredDate") + " </th>" +
                " </tr>");

            foreach (var file in files)
            {
                var expiredStatus = file.ExpirationDate != null ? (file.ExpirationDate.Value.Date < DateTime.Now.Date ? L("Expired") : L("Active")) : "Active";
                //var documentType = file.TruckId != null ? L("Truck") : L("Driver");
                var driverId = await _documentFilesManager.GetDriverIqamaActiveDocumentAsync(file.UserId.Value);
                mailMessage.AppendLine("<tr>" +
                    "<td style=\"border: 1px solid black;  \">" + file.UserFk.Name + "</td>" +
                    "<td style=\"border: 1px solid black;  \">" + driverId + "</td>" +
                    " <td style=\"border: 1px solid black;  \">" + file.Name + " </td> " +
                    "<td style=\"border: 1px solid black;  \">" + expiredStatus + " </td>" +
                    " <td style=\"border: 1px solid black;  \"> " + file.ExpirationDate + "</td>" +
                    " </tr>");
            }

            mailMessage.AppendLine("</table>");
        }


        #region Helpers

        /// <summary>
        /// Rap Text Into Span With Red Text Color
        /// </summary>
        /// <param name="rappedText"></param>
        /// <returns></returns>
        private static string IntoSpan(object rappedText)
            => $"<span style=\"color: #d82631\">{rappedText}</span>";

        [UnitOfWork]
        private async Task<string> GetTenantAdminEmailAddress(int tenantId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await (from user in _lookupUserRepository.GetAll()
                              where user.TenantId == tenantId
                                    && user.UserName.Equals(AbpUserBase.AdminUserName)
                              select user.EmailAddress).FirstOrDefaultAsync();

            }

        }

        [UnitOfWork]
        private async Task<string> GetCompanyName(int tenantId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await (from tenant in _tenantRepository.GetAll()
                              where tenant.Id == tenantId
                              select tenant.companyName).FirstOrDefaultAsync();
            }

        }

        #endregion

    }
}