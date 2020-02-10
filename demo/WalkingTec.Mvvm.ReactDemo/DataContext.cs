using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.ReactDemo.Models;

namespace WalkingTec.Mvvm.ReactDemo
{
    public class DataContext : FrameworkContext
    {
        public DbSet<Major> Majors { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<City> Cities { get; set; }

        public DataContext(CS cs)
     : base(cs)
        {
        }

        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }
    }
}
