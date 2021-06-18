using Abp.Authorization;
using Abp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Authorization.Permissions.Invoice
{
   public class AppAuthorizationInvoicesProvider: AppAuthorizationBaseProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull(AppPermissions.Pages) ?? context.CreatePermission(AppPermissions.Pages, L("Pages"));

            var Invoices = pages.CreateChildPermission(AppPermissions.Pages_Invoices, L("Invoices"));
            //Invoices.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Delete, L("DeletingInvoice"), multiTenancySides: MultiTenancySides.Tenant);
            Invoices.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_MakeUnPaid, L("UnPaidInvoice"), multiTenancySides: MultiTenancySides.Tenant);
            Invoices.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_MakePaid, L("PaidInvoice"), multiTenancySides: MultiTenancySides.Tenant);


            var Periods = Invoices.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Periods, L("BillingInterval"), multiTenancySides: MultiTenancySides.Host);
            Periods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Period_Create, L("CreateBillingInterval"), multiTenancySides: MultiTenancySides.Host);
            Periods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Period_Edit, L("EditBillingInterval"), multiTenancySides: MultiTenancySides.Host);
            Periods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Period_Delete, L("DeleteBillingInterval"), multiTenancySides: MultiTenancySides.Host);
            Periods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Period_Enabled, L("EnabledBillingInterval"), multiTenancySides: MultiTenancySides.Host);

            var Balances = Invoices.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Balances, L("Balances"), multiTenancySides: MultiTenancySides.Host);
            Balances.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Balances_Create, L("CreateBalance"), multiTenancySides: MultiTenancySides.Host);
            Balances.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_Balances_Delete, L("DeleteBalance"), multiTenancySides: MultiTenancySides.Host);

            var GroupPeriods = Invoices.CreateChildPermission(AppPermissions.Pages_Invoices_SubmitInvoices, L("SubmitInvoices"));
            GroupPeriods.CreateChildPermission(AppPermissions.Pages_Invoices_SubmitInvoices_Claim, L("ClaimSubmitInvoices"));
            GroupPeriods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Delete, L("DeleteSubmitInvoices"), multiTenancySides: MultiTenancySides.Host);
            GroupPeriods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Accepted, L("AcceptedSubmitInvoices"), multiTenancySides: MultiTenancySides.Host);
            GroupPeriods.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_SubmitInvoices_Rejected, L("RejectedSubmitInvoices"), multiTenancySides: MultiTenancySides.Host);
            Invoices.CreateChildPermission(AppPermissions.Pages_Invoices_Transaction, L("Transaction"));


            var paymentMethod = Invoices.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_PaymentMethods, L("PaymentMethods"), multiTenancySides: MultiTenancySides.Host);
            paymentMethod.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_PaymentMethod_Create, L("Create"), multiTenancySides: MultiTenancySides.Host);
            paymentMethod.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_PaymentMethod_Edit, L("Edit"), multiTenancySides: MultiTenancySides.Host);
            paymentMethod.CreateChildPermission(AppPermissions.Pages_Administration_Host_Invoices_PaymentMethod_Delete, L("Delete"), multiTenancySides: MultiTenancySides.Host);

        }
    }
}
