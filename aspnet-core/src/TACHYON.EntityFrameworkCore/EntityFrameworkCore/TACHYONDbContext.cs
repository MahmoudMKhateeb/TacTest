using TACHYON.Packing.PackingTypes;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Nationalities;
using TACHYON.Nationalities.NationalitiesTranslation;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Vases;
using TACHYON.ShippingRequestVases;
using TACHYON.TermsAndConditions;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.AddressBook.Ports;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.PickingTypes;
using TACHYON.UnitOfMeasures;
using TACHYON.AddressBook;
using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Delegation;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;
using TACHYON.Chat;
using TACHYON.Cities;
using TACHYON.Countries;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Editions;
using TACHYON.Friendships;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodsDetails;
using TACHYON.MultiTenancy;
using TACHYON.MultiTenancy.Accounting;
using TACHYON.MultiTenancy.Payments;
using TACHYON.Offers;
using TACHYON.Routs;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Storage;
using TACHYON.Trailers;
using TACHYON.Trailers.PayloadMaxWeights;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Shipping.ShippingRequestBidStatuses;
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.EntityFrameworkCore
{
    public class TACHYONDbContext : AbpZeroDbContext<Tenant, Role, User, TACHYONDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<PackingType> PackingTypes { get; set; }

        public virtual DbSet<ShippingType> ShippingTypes { get; set; }

        public virtual DbSet<Nationality> Nationalities { get; set; }

        public virtual DbSet<NationalityTranslation> NationalityTranslations { get; set; }

        public virtual DbSet<TransportTypesTranslation> TransportTypesTranslations { get; set; }

        public virtual DbSet<Vas> Vases { get; set; }

        public virtual DbSet<VasPrice> VasPrices { get; set; }
        public virtual DbSet<ShippingRequestVas> ShippingRequestVases { get; set; }

        public virtual DbSet<TermAndConditionTranslation> TermAndConditionTranslations { get; set; }

        public virtual DbSet<TermAndCondition> TermAndConditions { get; set; }

        public virtual DbSet<Capacity> Capacities { get; set; }

        public virtual DbSet<TransportType> TransportTypes { get; set; }

        public virtual DbSet<DocumentTypeTranslation> DocumentTypeTranslations { get; set; }

        public virtual DbSet<DocumentsEntity> DocumentsEntities { get; set; }

        public virtual DbSet<ShippingRequestStatus> ShippingRequestStatuses { get; set; }

        public virtual DbSet<Port> Ports { get; set; }

        public virtual DbSet<PickingType> PickingTypes { get; set; }

        public virtual DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }

        public virtual DbSet<Facility> Facilities { get; set; }

        public virtual DbSet<DocumentFile> DocumentFiles { get; set; }

        public virtual DbSet<DocumentType> DocumentTypes { get; set; }

        public virtual DbSet<ShippingRequest> ShippingRequests { get; set; }

        public virtual DbSet<GoodsDetail> GoodsDetails { get; set; }

        public virtual DbSet<Offer> Offers { get; set; }

        public virtual DbSet<RoutStep> RoutSteps { get; set; }
        public virtual DbSet<Route> Routes { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<County> Counties { get; set; }

        public virtual DbSet<RoutType> RoutTypes { get; set; }

        public virtual DbSet<GoodCategory> GoodCategories { get; set; }

        public virtual DbSet<Trailer> Trailers { get; set; }

        public virtual DbSet<TrailerStatus> TrailerStatuses { get; set; }

        public virtual DbSet<PayloadMaxWeight> PayloadMaxWeights { get; set; }

        public virtual DbSet<TrailerType> TrailerTypes { get; set; }

        public virtual DbSet<Truck> Trucks { get; set; }

        public virtual DbSet<TrucksType> TrucksTypes { get; set; }

        public virtual DbSet<TruckStatus> TruckStatuses { get; set; }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }
        public virtual DbSet<ShippingRequestBid> ShippingRequestBids { get; set; }
        public virtual DbSet<ShippingRequestBidStatus> ShippingRequestBidStatuses { get; set; }
        public virtual DbSet<RoutPoint> RoutPoints { get; set; }
        protected virtual bool CurrentIsCanceled => true;
        protected virtual bool IsCanceledFilterEnabled => CurrentUnitOfWorkProvider?.Current?.IsFilterEnabled("IHasIsCanceled") == true;

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

            return expression;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShippingRequestVas>(s =>
            {
                s.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<VasPrice>(v =>
                       {
                           v.HasIndex(e => new { e.TenantId });
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
            modelBuilder.Entity<RoutPoint>(s =>
            {
                s.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<GoodsDetail>(g =>
                       {
                           g.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Offer>(o =>
                       {
                           o.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Route>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
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

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}