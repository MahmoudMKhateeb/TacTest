using Microsoft.Extensions.Hosting;
using TACHYON.EntityFrameworkCore;

namespace TACHYON.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly TACHYONDbContext _context;
        private readonly IHostEnvironment _env;

        public InitialHostDbBuilder(TACHYONDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new ShippingRequestBidStatusesCreator(_context).Create();
            new CountriesAndCitiesCreator(_context).Create();
            new TransportTypesAndTheirChildrenCreator(_context).Create();
            _context.SaveChanges();
        }
    }
}