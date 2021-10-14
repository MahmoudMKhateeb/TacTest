using Abp.Events.Bus.Entities;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq.Expressions;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Ports;
using TACHYON.Authorization.Delegation;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;
using TACHYON.Chat;
using TACHYON.Cities;
using TACHYON.Cities.CitiesTranslations;
using TACHYON.Countries;
using TACHYON.Countries.CountriesTranslations;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.DriverLocationLogs;
using TACHYON.Editions;
using TACHYON.Friendships;
using TACHYON.Goods;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Invoices;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Groups;
using TACHYON.Invoices.PaymentMethods;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.Transactions;
using TACHYON.Localization;
using TACHYON.Mobile;
using TACHYON.MultiTenancy;
using TACHYON.MultiTenancy.Payments;
using TACHYON.Nationalities;
using TACHYON.Nationalities.NationalitiesTranslation;
using TACHYON.Offers;
using TACHYON.Packing.PackingTypes;
using TACHYON.PriceOffers;
using TACHYON.Rating;
using TACHYON.Receivers;
using TACHYON.Routs;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.RoutPoints;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.ShippingRequestTripVases;
using TACHYON.ShippingRequestVases;
using TACHYON.Storage;
using TACHYON.TachyonPriceOffers;
using TACHYON.TermsAndConditions;
using TACHYON.Trailers;
using TACHYON.Trailers.PayloadMaxWeights;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;
using TACHYON.Trucks.PlateTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations;
using TACHYON.Trucks.TruckStatusesTranslations;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;
using TACHYON.UnitOfMeasures;
using TACHYON.Vases;

namespace TACHYON.EntityFrameworkCore
{
    public class TACHYONDbContext : AbpZeroDbContext<Tenant, Role, User, TACHYONDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<DangerousGoodType> DangerousGoodTypes { get; set; }
        public virtual DbSet<DangerousGoodTypeTranslation> DangerousGoodTypeTranslations { get; set; }

        public virtual DbSet<CitiesTranslation> CitiesTranslations { get; set; }

        public virtual DbSet<CountriesTranslation> CountriesTranslations { get; set; }

        public virtual DbSet<PlateType> PlateTypes { get; set; }

        public virtual DbSet<PlateTypeTranslation> PlateTypeTranslations { get; set; }

        public virtual DbSet<TruckCapacitiesTranslation> TruckCapacitiesTranslations { get; set; }

        public virtual DbSet<TruckStatusesTranslation> TruckStatusesTranslations { get; set; }

        #region Trips
        public virtual DbSet<ShippingRequestTripRejectReason> ShippingRequestTripRejectReasons { get; set; }
        public DbSet<ShippingRequestTripRejectReasonTranslation> ShippingRequestTripRejectReasonTranslations { get; set; }
        public virtual DbSet<ShippingRequestTripTransition> ShippingRequestTripTransitions { get; set; }
        public virtual DbSet<ShippingRequestTrip> ShippingRequestTrips { get; set; }

        public virtual DbSet<ShippingRequestReasonAccident> ShippingRequestReasonAccidents { get; set; }
        public DbSet<ShippingRequestReasonAccidentTranslation> ShippingRequestReasonAccidentTranslations { get; set; }
        public virtual DbSet<ShippingRequestTripAccident> ShippingRequestTripAccidents { get; set; }
        public virtual DbSet<ShippingRequestTripAccidentResolve> ShippingRequestTripAccidentResolves { get; set; }


        #endregion
        #region TachyonDeal
        public virtual DbSet<ShippingRequestsCarrierDirectPricing> ShippingRequestsCarrierDirectPricing { get; set; }
        #endregion
        #region Localization
        public DbSet<AppLocalization> AppLocalizations { get; set; }
        public DbSet<AppLocalizationTranslation> AppLocalizationTranslations { get; set; }
        public DbSet<TerminologieEdition> TerminologieEditions { get; set; }

        public DbSet<TerminologiePage> TerminologiePages { get; set; }


        #endregion
        public virtual DbSet<PackingType> PackingTypes { get; set; }
        public virtual DbSet<PackingTypeTranslation> PackingTypeTranslations { get; set; }

        public virtual DbSet<ShippingType> ShippingTypes { get; set; }
        public virtual DbSet<ShippingTypeTranslation> ShippingTypeTranslations { get; set; }

        public virtual DbSet<Nationality> Nationalities { get; set; }

        public virtual DbSet<NationalityTranslation> NationalityTranslations { get; set; }

        public virtual DbSet<TrucksTypesTranslation> TrucksTypesTranslations { get; set; }

        public virtual DbSet<TransportTypesTranslation> TransportTypesTranslations { get; set; }

        public virtual DbSet<Vas> Vases { get; set; }

        public virtual DbSet<VasTranslation> VasTranslations { get; set; }

        public virtual DbSet<VasPrice> VasPrices { get; set; }
        public virtual DbSet<ShippingRequestVas> ShippingRequestVases { get; set; }
        public virtual DbSet<ShippingRequestTripVas> ShippingRequestTripVases { get; set; }
        public virtual DbSet<TachyonPriceOffer> TachyonPriceOffers { get; set; }

        public virtual DbSet<Receiver> Receivers { get; set; }

        public virtual DbSet<TermAndConditionTranslation> TermAndConditionTranslations { get; set; }

        public virtual DbSet<TermAndCondition> TermAndConditions { get; set; }

        public virtual DbSet<Capacity> Capacities { get; set; }

        public virtual DbSet<TransportType> TransportTypes { get; set; }

        public virtual DbSet<DocumentTypeTranslation> DocumentTypeTranslations { get; set; }

        public virtual DbSet<DocumentsEntity> DocumentsEntities { get; set; }

        public virtual DbSet<Shipping.ShippingRequestStatuses.ShippingRequestStatus> ShippingRequestStatuses { get; set; }

        public virtual DbSet<Port> Ports { get; set; }

        // public virtual DbSet<PickingType> PickingTypes { get; set; }

        public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

        public virtual DbSet<Facility> Facilities { get; set; }

        public virtual DbSet<DocumentFile> DocumentFiles { get; set; }

        public virtual DbSet<DocumentType> DocumentTypes { get; set; }

        public virtual DbSet<ShippingRequest> ShippingRequests { get; set; }
        public DbSet<ShippingRequestDirectRequest> ShippingRequestDirectRequests { get; set; }

        public virtual DbSet<GoodsDetail> GoodsDetails { get; set; }

        public virtual DbSet<Offer> Offers { get; set; }

        public virtual DbSet<RoutStep> RoutSteps { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<RoutType> RoutTypes { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<County> Counties { get; set; }


        public virtual DbSet<GoodCategory> GoodCategories { get; set; }

        public virtual DbSet<GoodCategoryTranslation> GoodCategoryTranslations { get; set; }

        public virtual DbSet<Trailer> Trailers { get; set; }

        public virtual DbSet<TrailerStatus> TrailerStatuses { get; set; }

        public virtual DbSet<PayloadMaxWeight> PayloadMaxWeights { get; set; }

        public virtual DbSet<TrailerType> TrailerTypes { get; set; }

        public virtual DbSet<Truck> Trucks { get; set; }

        public virtual DbSet<TrucksType> TrucksTypes { get; set; }

        public virtual DbSet<TruckStatus> TruckStatuses { get; set; }

        /*Invoice entity*/
        public virtual DbSet<InvoicePeriod> InvoicePeriod { get; set; }
        public virtual DbSet<Invoices.Invoice> Invoice { get; set; }
        public DbSet<InvoiceProforma> InvoiceProforma { get; set; }


        public virtual DbSet<InvoiceTrip> InvoiceTrips { get; set; }
        public virtual DbSet<GroupPeriod> GroupPeriod { get; set; }
        public virtual DbSet<GroupShippingRequests> GroupShippingRequests { get; set; }
        public virtual DbSet<GroupPeriodInvoice> GroupPeriodInvoice { get; set; }
        public DbSet<InvoicePaymentMethod> InvoicePaymentMethods { get; set; }

        public DbSet<SubmitInvoice> SubmitInvoices { get; set; }
        public DbSet<SubmitInvoiceTrip> SubmitInvoiceTrips { get; set; }
        public virtual DbSet<BalanceRecharge> BalanceRecharge { get; set; }


        public DbSet<Transaction> Transaction { get; set; }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<MultiTenancy.Accounting.Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }

        public virtual DbSet<ShippingRequestBid> ShippingRequestBids { get; set; }

        public virtual DbSet<RoutPoint> RoutPoints { get; set; }
        public virtual DbSet<RoutPointDocument> RoutPointDocuments { get; set; }
        public virtual DbSet<RoutPointStatusTransition> RoutPointStatusTransitions { get; set; }
        public virtual DbSet<RatingLog> RatingLoogs { get; set; }

        public DbSet<TenantCarrier> TenantCarriers { get; set; }
        public DbSet<DriverLocationLog> DriverLocationLogs { get; set; }

        protected virtual bool CurrentIsCanceled => true;
        protected virtual bool CurrentIsDrafted => false;
        protected virtual bool IsCanceledFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("IHasIsCanceled") == true;

        protected virtual bool IsDraftedFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("IHasIsDrafted") == true;

        #region Mobile
        public DbSet<UserDeviceToken> UserDeviceTokens { get; set; }
        public DbSet<UserOTP> UserOTPs { get; set; }


        #endregion
        #region Price offers
        public DbSet<PriceOffer> PriceOffers { get; set; }
        public DbSet<PriceOfferDetail> PriceOfferDetails { get; set; }

        #endregion
        public TACHYONDbContext(DbContextOptions<TACHYONDbContext> options)
            : base(options)
        {

        }

        protected override bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType)
        {
            if (typeof(IHasIsCanceled).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            else if (typeof(IHasIsDrafted).IsAssignableFrom(typeof(TEntity)))
            {
                return true;
            }
            return base.ShouldFilterEntity<TEntity>(entityType);
        }

        protected override Expression<Func<TEntity, bool>> CreateFilterExpression<TEntity>()
        {
            var expression = base.CreateFilterExpression<TEntity>();
            if (typeof(IHasIsCanceled).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> mayHaveOUFilter = e => ((IHasIsCanceled)e).IsCancled == CurrentIsCanceled || (((IHasIsCanceled)e).IsCancled == CurrentIsCanceled) == IsCanceledFilterEnabled;
                expression = expression == null ? mayHaveOUFilter : CombineExpressions(expression, mayHaveOUFilter);
            }

            else if (typeof(IHasIsDrafted).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> mayHaveOUFilter = e => ((IHasIsDrafted)e).IsDrafted == CurrentIsDrafted || (((IHasIsDrafted)e).IsDrafted == CurrentIsDrafted) == IsDraftedFilterEnabled;
                expression = expression == null ? mayHaveOUFilter : CombineExpressions(expression, mayHaveOUFilter);
            }

            return expression;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ShippingRequestVas>(s =>
            //{
            //    s.HasIndex(e => new { e.TenantId });
            //});
            modelBuilder.Entity<VasPrice>(v =>
                       {
                           v.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Receiver>(r =>
            {
                r.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<ShippingRequestBid>().HasQueryFilter(p => !p.IsCancled);

            modelBuilder.Entity<Facility>(f =>
            {
                f.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<DocumentFile>(d =>
                       {
                           d.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ShippingRequest>(s =>
                       {
                           s.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ShippingRequestBid>(s =>
            {
                s.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<TachyonPriceOffer>(s =>
            {
                s.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<Offer>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            //modelBuilder.Entity<Route>(r =>
            //           {
            //               r.HasIndex(e => new { e.TenantId });
            //           });
            modelBuilder.Entity<RoutStep>(r =>
            {
                r.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<Trailer>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Truck>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });

            modelBuilder.Entity<TrucksType>()
                .Property(b => b.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<GoodCategory>()
                .Property(b => b.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<BinaryObject>(b =>
                       {
                           b.HasIndex(e => new { e.TenantId });
                       });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
            {
                b.HasQueryFilter(m => !m.IsDeleted)
                    .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
                    .IsUnique();
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.SourceUserId });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId });
            });

            modelBuilder.Entity<ShippingRequestTrip>()
            .HasIndex(e => e.WaybillNumber)
            .IsUnique();

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(a => a.AccountNumber).IsUnique();
                b.HasIndex(a => a.ContractNumber).IsUnique();
            });


            modelBuilder.Entity<RoutPoint>()
            .HasIndex(e => e.WaybillNumber)
            .IsUnique();

            modelBuilder.Entity<User>()
            .HasIndex(e => e.AccountNumber)
            .IsUnique();

            modelBuilder.Entity<Invoice>()
            .HasIndex(e => e.InvoiceNumber)
            .IsUnique();

            modelBuilder.Entity<ShippingRequest>()
            .HasIndex(e => e.ReferenceNumber)
            .IsUnique();
            modelBuilder.ConfigurePersistedGrantEntity();
        }

    }
}