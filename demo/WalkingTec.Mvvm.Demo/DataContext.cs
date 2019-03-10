using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo
{
    public class DataContext : FrameworkContext
    {
        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }

        public DbSet<Major> Majors { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<MyUser> MyUsers { get; set; }

    }
}
