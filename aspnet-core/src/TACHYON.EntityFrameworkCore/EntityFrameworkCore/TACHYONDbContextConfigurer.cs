using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace TACHYON.EntityFrameworkCore
{
    public static class TACHYONDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<TACHYONDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, x => x.UseNetTopologySuite());
        }

        public static void Configure(DbContextOptionsBuilder<TACHYONDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection, x => x.UseNetTopologySuite());
        }
    }
}