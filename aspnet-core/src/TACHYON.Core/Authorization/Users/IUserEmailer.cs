using System;
using System.Threading.Tasks;
using TACHYON.Chat;
using TACHYON.MultiTenancy;

namespace TACHYON.Authorization.Users
{
    public interface IUserEmailer
    {
        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null);
        /// <summary>
        /// Send Email to admin tenant user when all documents approved by host
        /// </summary>
        /// <param name="loginLink"></param>
        /// <param name="tenant"></param>
        /// <returns></returns>
        Task SendAllApprovedDocumentsAsync(Tenant tenant,string loginLink);
        /// <summary>
        /// Send Email to admin tenant user when all documents approved by host
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="documentFileName"></param>
        /// <returns></returns>
        Task SendExpiredDateDocumentsAsyn(Tenant tenant, string documentFileName);
        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Password reset link (optional)</param>
        Task SendPasswordResetLinkAsync(User user, string link = null);

        /// <summary>
        /// Sends an email for unread chat message to user's email.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="senderUsername"></param>
        /// <param name="senderTenancyName"></param>
        /// <param name="chatMessage"></param>
        Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage);
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
        /// <param name="tenant"></param>
        /// <param name="documentName"></param>
        /// <param name="documentExpireDate"></param>
        /// <returns></returns>
        Task SendWarningSuspendAccountForExpiredDocumentEmail(Tenant tenant,string documentName,DateTime documentExpireDate); 
       
        /// <summary>
        /// Send an Email To Notify User That Him Account Was Suspended For Expired Document
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="documentName"></param>
        /// <returns></returns>
        Task SendSuspendedAccountForExpiredDocumentEmail(Tenant tenant, string documentName);

        /// <summary>
        /// Send an Email To Notify User That He Have Due Invoice
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="invoiceIssueDate"></param>
        /// <param name="invoiceTotalAmount"></param>
        /// <returns></returns>
        Task SendInvoiceDueEmail(Tenant tenant, string invoiceNumber, DateTime invoiceIssueDate,
            decimal invoiceTotalAmount);

        /// <summary>
        /// Send Email To Tell User He Have a New Created Invoice
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="invoiceDueDate"></param>
        /// <param name="invoiceIssueDate"></param>
        /// <param name="invoiceTotalAmount"></param>
        /// <param name="invoiceLink"></param>
        /// <returns></returns>
        Task SendIssuedInvoiceEmail(Tenant tenant, DateTime invoiceDueDate,
            DateTime invoiceIssueDate, decimal invoiceTotalAmount,string invoiceLink);

    }
}