using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo
{
    public class DataContext : FrameworkContext
    {
        public DataContext(CS cs)
             : base(cs)
        {
        }

        public DbSet<Major> Majors { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<MyUser> MyUsers { get; set; }

        public DbSet<City> Cities { get; set; }
        public DbSet<不要用中文模型名> 不要中文 { get; set; }


        public override async Task<bool> DataInit(object allModules, bool IsSpa)
        {
            var state = await base.DataInit(allModules, IsSpa);

            return state;
        }

    }
}
