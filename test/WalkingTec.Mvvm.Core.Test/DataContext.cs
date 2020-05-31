using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Core.Test
{
    public class DataContext : FrameworkContext
    {
        public DataContext(string cs, DBTypeEnum dbtype, string version=null)
             : base(cs, dbtype,version)
        {
        }

        public DbSet<Major> Majors { get; set; }
        public DbSet<OptMajor> OptMajors { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<StudentMajor> StudentMajors { get; set; }

        public DbSet<SchoolWithOtherID> SchoolWithOtherIDs { get; set; }
        public DbSet<MajorWithOtherID> MajorWithOtherIDs { get; set; }
        public DbSet<StudentMajorWithOtherID> StudentMajorWithOtherIDs { get; set; }
        public DbSet<SchoolTop> SchoolTops { get; set; }
        public DbSet<MajorTop> MajorTops { get; set; }
        public DbSet<StudentMajorTop> StudentMajorTops { get; set; }

        public DbSet<GoodsSpecification> GoodsSpecifications { get; set; }
        public DbSet<GoodsCatalog> GoodsCatalogs { get; set; }
    }
}
