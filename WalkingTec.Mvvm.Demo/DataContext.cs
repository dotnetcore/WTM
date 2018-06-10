using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo
{
    public class DataContext : FrameworkContext
    {
        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }

        public DbSet<TestUser> TestUsers { get; set; }
        public DbSet<TestRole> TestRoles { get; set; }
        public DbSet<ShipperInfo> ShipperInfo { get; set; }

        public DbSet<Company> Companys { get; set; }
        public DbSet<Employee> Employee { get; set; }

        public async override Task<bool> DataInit(object allModules)
        {
            if (await base.DataInit(allModules))
            {
                return true;
            }
            return false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseLoggerFactory(WTMLoggerFactory));

        }
        public new static readonly LoggerFactory WTMLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
#endif

    }
}
