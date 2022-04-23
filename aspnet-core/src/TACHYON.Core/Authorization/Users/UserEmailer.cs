using Abp.Application.Editions;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using TACHYON.Chat;
using TACHYON.Documents.DocumentFiles;
using TACHYON.EmailTemplates;
using TACHYON.EmailTemplates.Dtos;
using TACHYON.MultiTenancy;

namespace TACHYON.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    internal class UserEmailer : TACHYONServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IRepository<Edition> _editionRepository;
        private readonly IRepository<User, long> _lookupUserRepository;
        private readonly IRepository<EmailTemplate> _emailTemplatesRepository;

        public UserEmailer(
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IRepository<Edition> editionRepository,
            IRepository<User, long> lookupUserRepository,
            IRepository<EmailTemplate> emailTemplatesRepository)
        {
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _editionRepository = editionRepository;
            _lookupUserRepository = lookupUserRepository;
            _emailTemplatesRepository = emailTemplatesRepository;
        }


        public async Task SendTestTemplateEmail(TestEmailTemplateInputDto input)
        {
            dynamic content =
                JsonConvert.DeserializeObject<ExpandoObject>(input.TestTemplate.Content, new ExpandoObjectConverter());
            if (content != null)
                await _emailSender.SendAsync(new MailMessage
                {
                    To = { input.TestEmail },
                    Subject = input.TestTemplate.DisplayName,
                    Body = content.html,
                    IsBodyHtml = true
                });
        }


        #region TACHYON_Emails

        [UnitOfWork]
        public virtual async Task SendEmailActivationEmail(User user, string link, string password)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
                throw new UserFriendlyException("EmailConfirmationCodeNotFound");

            try
            {
                string html = await GetContent(EmailTemplateTypesEnum.EmailActivation);

                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

                if (user.TenantId.HasValue)
                    link = link.Replace("{tenantId}", user.TenantId.ToString());

                link = EncryptQueryParameters(link);

                var tenancyName = GetTenancyNameOrNull(user.TenantId);

                html = html
                    .Replace("{{Name}}", user.FullName)
                    .Replace("{{Company name}}", tenancyName ?? "")
                    .Replace("{{Email}}", user.EmailAddress)
                    .Replace("{{Password}}", password)
                    .Replace("{{Link}}", link);

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { user.EmailAddress },
                    Subject = L("EmailActivation"),
                    Body = html,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        [UnitOfWork]
        public virtual async Task SendApprovedDocumentEmail(int tenantId, string loginLink)
        {
            try
            {
                string html = await GetContent(EmailTemplateTypesEnum.ApprovedDocument);
                html = html
                    .Replace("{{CompanyName}}", await GetCompanyName(tenantId))
                    .Replace("{{Link}}", loginLink);

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { await GetTenantAdminEmailAddress(tenantId) },
                    Subject = L("DocumentsApproved"),
                    Body = html,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }


        [UnitOfWork]
        public virtual async Task SendRejectedDocumentEmail(int tenantId, string documentName, string rejectionReason)
        {
            try
            {
                string html = await GetContent(EmailTemplateTypesEnum.RejectedDocument);
                html = html.Replace("{{CompanyName}}", await GetCompanyName(tenantId))
                    .Replace("{{DocumentName}}", documentName)
                    .Replace("{{RejectionReason}}", rejectionReason);

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { await GetTenantAdminEmailAddress(tenantId) },
                    Subject = L("RejectedDocument"),
                    Body = html,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendExpiredDocumentsEmail(int tenantId, params DocumentFile[] documents)
        {
            try
            {
                var htmlTemplate = await GetContent(EmailTemplateTypesEnum.ExpiredDocuments);
                var companyName = await GetCompanyName(tenantId);
                htmlTemplate = htmlTemplate.Replace("{{CompanyName}}", companyName ?? "")
                    .Replace("{{DocumentsTable}}", documents.Length.ToString());
                var adminEmail = await GetTenantAdminEmailAddress(tenantId);

                if (!adminEmail.IsNullOrEmpty())
                    await _emailSender.SendAsync(new MailMessage
                    {
                        To = { adminEmail },
                        Subject = L("ExpiredDocument"),
                        Body = htmlTemplate,
                        IsBodyHtml = true
                    });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendResetPasswordEmail(User user, string link)
        {
            #region InitRequiredData

            if (user.PasswordResetCode.IsNullOrEmpty())
                throw new UserFriendlyException(L("PasswordResetCodeNotFound"));


            var companyName = user.TenantId is null ? "" : await GetCompanyName(user.TenantId.Value);
            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));
                if (user.TenantId.HasValue)
                    link = link.Replace("{tenantId}", user.TenantId.ToString());

                link = EncryptQueryParameters(link);
            }

            #endregion

            try
            {
                var htmlTemplate = await GetContent(EmailTemplateTypesEnum.ResetPassword);
                htmlTemplate = htmlTemplate.Replace("{{CompanyName}}", companyName)
                    .Replace("{{Link}}", link);

                if (user.TenantId.HasValue)
                    await _emailSender.SendAsync(new MailMessage
                    {
                        To = { await GetTenantAdminEmailAddress(user.TenantId.Value) },
                        Subject = L("ResetPassword"),
                        Body = htmlTemplate,
                        IsBodyHtml = true
                    });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendPasswordUpdatedEmail(int? tenantId, string userEmail, string newPassword)
        {
            try
            {
                var htmlTemplate = await GetContent(EmailTemplateTypesEnum.PasswordUpdated);
                htmlTemplate = htmlTemplate.Replace("{{NewPassword}}", newPassword);

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { userEmail },
                    Subject = L("PasswordUpdated"),
                    Body = htmlTemplate,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendSuspendedAccountWarningEmail(int tenantId, string documentName,
            DateTime documentExpireDate)
        {
            try
            {
                var emailTemplate = await GetContent(EmailTemplateTypesEnum.SuspendedAccountWarning);
                var companyName = await GetCompanyName(tenantId);
                emailTemplate = emailTemplate.Replace("{{CompanyName}}", companyName)
                    .Replace("{{DocumentName}}", documentName)
                    .Replace("{{ExpirationDate}}", documentExpireDate.ToString("dd-MM-yyyy"));

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { await GetTenantAdminEmailAddress(tenantId) },
                    Subject = L("WarningSuspendAccount"),
                    Body = emailTemplate,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendSuspendedAccountEmail(int tenantId, string documentName)
        {
            try
            {
                var emailTemplate = await GetContent(EmailTemplateTypesEnum.SuspendedAccount);
                var companyName = await GetCompanyName(tenantId);
                emailTemplate = emailTemplate.Replace("{{CompanyName}}", companyName)
                    .Replace("{{DocumentName}}", documentName);

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { await GetTenantAdminEmailAddress(tenantId) },
                    Subject = L("SuspendedAccountDocumentExpired"),
                    Body = emailTemplate,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendInvoiceDueEmail(int tenantId, string invoiceNumber, decimal invoiceTotalAmount)
        {
            try
            {
                var emailTemplate = await GetContent(EmailTemplateTypesEnum.InvoiceDue);
                var companyName = await GetCompanyName(tenantId);
                emailTemplate = emailTemplate.Replace("{{CompanyName}}", companyName)
                    .Replace("{{InvoiceNumber}}", invoiceNumber)
                    .Replace("{{TotalAmount}}", invoiceTotalAmount.ToString(CultureInfo.CurrentUICulture));

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { await GetTenantAdminEmailAddress(tenantId) },
                    Subject = L("InvoiceDue_Title"),
                    Body = emailTemplate,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendIssuedInvoiceEmail(int tenantId, DateTime invoiceDueDate,
            DateTime invoiceIssueDate, decimal invoiceTotalAmount, string invoiceUrl)
        {
            // You Can Get Invoice Url From IAppUrlService , Method Name: CreateInvoiceDetailsFormat()

            try
            {
                var emailTemplate = await GetContent(EmailTemplateTypesEnum.IssuedInvoice);
                var companyName = await GetCompanyName(tenantId);
                emailTemplate = emailTemplate.Replace("{{CompanyName}}", companyName)
                    .Replace("{{IssueDate}}", invoiceIssueDate.ToString("dd-MM-yyyy"))
                    .Replace("{{TotalAmount}}", invoiceTotalAmount.ToString(CultureInfo.CurrentUICulture))
                    .Replace("{{DueDate}}", invoiceDueDate.ToString("dd-MM-yyyy"))
                    .Replace("{{InvoiceUrl}}", invoiceDueDate.ToString("dd-MM-yyyy"));

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { await GetTenantAdminEmailAddress(tenantId) },
                    Subject = L("IssuedInvoice"),
                    Body = emailTemplate,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        #endregion

        #region ABP_Emails

        public async Task SendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                var htmlContent = await GetContent(EmailTemplateTypesEnum.SubscriptionExpire);
                var companyName = await GetCompanyName(tenantId);
                htmlContent = htmlContent.Replace("{{CompanyName}}", companyName ?? "")
                    .Replace("{{ExpirationDate}}", utcNow.ToString("yyyy-MM-dd"));

                var adminEmail = await GetTenantAdminEmailAddress(tenantId);
                if (adminEmail.IsNullOrEmpty()) return;

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { adminEmail },
                    Subject = L("SubscriptionExpired"),
                    Body = htmlContent,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendSubscriptionAssignedToAnotherEmail(int tenantId, DateTime utcNow, int expiringEditionId)
        {
            try
            {
                var editionName = await GetEditionDisplayName(expiringEditionId);

                var htmlContent = await GetContent(EmailTemplateTypesEnum.SubscriptionAssignedToAnother);
                var companyName = await GetCompanyName(tenantId);
                htmlContent = htmlContent.Replace("{{CompanyName}}", companyName ?? "")
                    .Replace("{{ExpirationDate}}", utcNow.ToString("yyyy-MM-dd"))
                    .Replace("{{ExpiringEditionName}}", editionName ?? "");

                var adminEmail = await GetTenantAdminEmailAddress(tenantId);
                if (adminEmail.IsNullOrEmpty()) return;

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { adminEmail },
                    Subject = L("EditionSubscriptionExpired"),
                    Body = htmlContent,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }
        }

        public async Task SendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                var htmlContent = await GetContent(EmailTemplateTypesEnum.FailedSubscriptionTerminations);
                htmlContent = htmlContent
                    .Replace("{{failedDate}}", utcNow.ToString("yyyy-MM-dd"));

                var adminEmail = await GetTenantAdminEmailAddress(null); // Get Host Email
                if (adminEmail.IsNullOrEmpty()) return;

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { adminEmail },
                    Subject = L("FailedSubscriptionTerminations"),
                    Body = htmlContent,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

            // try
            // {
            //     var hostAdmin = await _userManager.GetAdminAsync();
            //     if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
            //     {
            //         return;
            //     }
            //
            //     var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, hostAdmin.TenantId, hostAdmin.Id);
            //     var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
            //     var emailTemplate = await GetEmailTemplate(null, L("FailedSubscriptionTerminations_Title"), L("FailedSubscriptionTerminations_SubTitle"));
            //     var mailMessage = new StringBuilder();
            //
            //     mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("FailedSubscriptionTerminations_Email_Body", culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
            //     mailMessage.AppendLine("<br />");
            //
            //     await ReplaceBodyAndSend(hostAdmin.EmailAddress, L("FailedSubscriptionTerminations_Email_Subject"), emailTemplate, mailMessage);
            // }
            // catch (Exception exception)
            // {
            //     Logger.Error(exception.Message, exception);
            // }
        }

        public async Task SendSubscriptionExpiringSoonEmail(int tenantId, DateTime dateToCheckRemainingDayCount)
        {
            try
            {
                var htmlContent = await GetContent(EmailTemplateTypesEnum.SubscriptionExpiringSoon);
                var companyName = await GetCompanyName(tenantId);
                htmlContent = htmlContent
                    .Replace("{{CompanyName}}", companyName ?? "")
                    .Replace("{{ExpirationDate}}", dateToCheckRemainingDayCount.ToString("yyyy-MM-dd"));

                var adminEmail = await GetTenantAdminEmailAddress(tenantId);
                if (adminEmail.IsNullOrEmpty()) return;

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { adminEmail },
                    Subject = L("SubscriptionExpiringSoon"),
                    Body = htmlContent,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }

            // try
            // {
            //     using (_unitOfWorkManager.Begin())
            //     {
            //         using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            //         {
            //             var tenantAdmin = await _userManager.GetAdminAsync();
            //             if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
            //             {
            //                 return;
            //             }
            //
            //             var tenantAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
            //             var culture = CultureHelper.GetCultureInfoByChecking(tenantAdminLanguage);
            //
            //             var emailTemplate = await GetEmailTemplate(null, L("SubscriptionExpiringSoon_Title"), L("SubscriptionExpiringSoon_SubTitle"));
            //             var mailMessage = new StringBuilder();
            //
            //             mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpiringSoon_Email_Body", culture, dateToCheckRemainingDayCount.ToString("yyyy-MM-dd") + " UTC") + "<br />");
            //             mailMessage.AppendLine("<br />");
            //
            //             await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpiringSoon_Email_Subject"), emailTemplate, mailMessage);
            //         }
            //     }
            // }
            // catch (Exception exception)
            // {
            //     Logger.Error(exception.Message, exception);
            // }
        }

        public async Task SendChatMessageMail(User user, string senderUsername, string senderTenancyName,
            ChatMessage chatMessage)
        {
            try
            {
                var htmlContent = await GetContent(EmailTemplateTypesEnum.ChatMessage);
                htmlContent = htmlContent
                    .Replace("{{UserName}}", user.Name)
                    .Replace("{{SenderName}}", senderUsername)
                    .Replace("{{SenderTenancyName}}", senderTenancyName)
                    .Replace("{{Message}}", chatMessage.Message)
                    .Replace("{{SentAt}}", chatMessage.CreationTime.ToString("dd-MM-yyyy"));

                await _emailSender.SendAsync(new MailMessage
                {
                    To = { user.EmailAddress },
                    Subject = L("ChatMessage"),
                    Body = htmlContent,
                    IsBodyHtml = true
                });
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
            }


            // try
            // {
            //     var emailTemplate = await GetEmailTemplate(user.TenantId, L("NewChatMessageEmail_Title"));
            //     var mailMessage = new StringBuilder();
            //
            //     mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername + "<br />");
            //     mailMessage.AppendLine("<b>" + L("Time") + "</b>: " + chatMessage.CreationTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + " UTC<br />");
            //     mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
            //     mailMessage.AppendLine("<br />");
            //
            //     await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate, mailMessage);
            // }
            // catch (Exception exception)
            // {
            //     Logger.Error(exception.Message, exception);
            // }
        }

        #endregion

        #region Helpers

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null) return null;

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private static string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = link.Substring(link.IndexOf('?')).TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" +
                   HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        [UnitOfWork]
        private async Task<string> GetTenantAdminEmailAddress(int? tenantId)
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

        private async Task<string> GetContent(EmailTemplateTypesEnum type)
        {
            var template = await _emailTemplatesRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.EmailTemplateType == type);

            var emailTemplateTranslation =
                template.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name));
            var converter = new ExpandoObjectConverter();

            dynamic content = JsonConvert.DeserializeObject<ExpandoObject>(
                emailTemplateTranslation != null ? emailTemplateTranslation.TranslatedContent : template.Content,
                converter);

            return content?.html;
        }

        private async Task<string> GetEditionDisplayName(int editionId)
        {
            return await _editionRepository.GetAll()
                .Where(x => x.Id == editionId)
                .Select(x => x.DisplayName)
                .FirstOrDefaultAsync();
        }

        #endregion
    }
}