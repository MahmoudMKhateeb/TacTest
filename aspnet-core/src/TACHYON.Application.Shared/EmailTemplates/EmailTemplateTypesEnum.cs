namespace TACHYON.EmailTemplates
{
    public enum EmailTemplateTypesEnum
    {
        EmailTemplateLayout = 0,
        
        #region TACHYON_Emails

        EmailActivation = 1,
        ApprovedDocument = 2,
        RejectedDocument = 3,
        ResetPassword = 4,
        PasswordUpdated = 5,
        SuspendedAccountWarning = 6,
        SuspendedAccount = 7,
        InvoiceDue = 8,
        IssuedInvoice = 9,
        ExpiredDocuments = 10,

        #endregion

        #region ABP_Emails

        SubscriptionExpire = 11,
        SubscriptionAssignedToAnother = 12,
        FailedSubscriptionTerminations = 13,
        SubscriptionExpiringSoon = 14,
        ChatMessage = 15,

        #endregion
        

    }
}