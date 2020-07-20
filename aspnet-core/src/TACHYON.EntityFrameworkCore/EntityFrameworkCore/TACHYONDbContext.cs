using TACHYON.Countries;
using TACHYON.Routs.RoutTypes;
using TACHYON.Goods.GoodCategories;
using TACHYON.Trailers;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks;
using Abp.IdentityServer4;
using Abp.Organizations;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization.Delegation;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;
using TACHYON.Chat;
using TACHYON.Editions;
using TACHYON.Friendships;
using TACHYON.MultiTenancy;
using TACHYON.MultiTenancy.Accounting;
using TACHYON.MultiTenancy.Payments;
using TACHYON.Storage;
using TACHYON.Trailers.PayloadMaxWeights;

namespace TACHYON.EntityFrameworkCore
{
    public class TACHYONDbContext : AbpZeroDbContext<Tenant, Role, User, TACHYONDbContext>, IAbpPersistedGrantDbContext
    {
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

        public TACHYONDbContext(DbContextOptions<TACHYONDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);




           
            modelBuilder.Entity<Trailer>(t =>
            {
                t.HasIndex(e => new { e.TenantId });
            });
 modelBuilder.Entity<Truck>(t =>
            {
                t.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<TruckStatus>(t =>
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
