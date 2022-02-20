using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Chat;
using TACHYON.Documents.DocumentFiles;
using TACHYON.EmailTemplates.Dtos;

namespace TACHYON.Authorization.Users
{
    public interface IUserEmailer
    {
        Task SendTestTemplateEmail(TestEmailTemplateInputDto input);
        
        #region TACHYON_Emails

                /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        Task SendEmailActivationEmail(User user, string link, string plainPassword);

        /// <summary>
        /// Send Email to admin tenant user when all documents approved by host
        /// </summary>
        /// <param name="documentName"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task SendApprovedDocumentEmail(int tenantId, string documentName);
        

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Password reset link (optional)</param>
        Task SendResetPasswordEmail(User user, string link = null);

        /// <summary>
        /// Send an Email When User Account Password Updated or Changed
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="userEmail"></param>
        /// /// <param name="tenantId"></param>
        /// <returns></returns>
        Task SendPasswordUpdatedEmail(int? tenantId, string userEmail, string newPassword);

        /// <summary>
        /// Send an Email To Warn User From Suspend Him Account Because That He Has Document Almost Expired
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="documentName"></param>
        /// <param name="documentExpireDate"></param>
        /// <returns></returns>
        Task SendSuspendedAccountWarningEmail(int tenantId, string documentName, DateTime documentExpireDate);

        /// <summary>
        /// Send an Email To Notify User That Him Account Was Suspended For Expired Document
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="documentName"></param>
        /// <returns></returns>
        Task SendSuspendedAccountEmail(int tenantId, string documentName);

        /// <summary>
        /// Send an Email To Notify User That He Have Due Invoice
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="invoiceTotalAmount"></param>
        /// <returns></returns>
        Task SendInvoiceDueEmail(int tenantId, string invoiceNumber, decimal invoiceTotalAmount);

        /// <summary>
        /// Send Email To Tell User He Have a New Created Invoice
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="invoiceDueDate"></param>
        /// <param name="invoiceIssueDate"></param>
        /// <param name="invoiceTotalAmount"></param>
        /// <param name="invoiceUrl"></param>
        /// <returns></returns>
        Task SendIssuedInvoiceEmail(int tenantId, DateTime invoiceDueDate,
            DateTime invoiceIssueDate, decimal invoiceTotalAmount, string invoiceUrl);

        Task SendRejectedDocumentEmail(int tenantId, string documentName, string rejectionReason);
        
        Task SendExpiredDocumentsEmail(int tenantId,params DocumentFile[] documents);
        
        #endregion

        #region ABP_Emails
        
        Task SendSubscriptionExpireEmail(int tenantId, DateTime utcNow);

        Task SendSubscriptionAssignedToAnotherEmail(int tenantId, DateTime utcNow, int expiringEditionId);

        Task SendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow);

        Task SendSubscriptionExpiringSoonEmail(int tenantId, DateTime dateToCheckRemainingDayCount);
        
        /// <summary>
        /// Sends an email for unread chat message to user's email.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="senderUsername"></param>
        /// <param name="senderTenancyName"></param>
        /// <param name="chatMessage"></param>
        Task SendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage);
        #endregion
    }
}