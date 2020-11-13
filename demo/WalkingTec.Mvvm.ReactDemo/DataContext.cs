using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.Core;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.ReactDemo
{
    public class DataContext : FrameworkContext
    {
        public DbSet<FrameworkUser> FrameworkUsers { get; set; }
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

        public override async Task<bool> DataInit(object allModules, bool IsSpa)
        {
            var state = await base.DataInit(allModules, IsSpa);
            if (state == true)
            {
                var user = new FrameworkUser
                {
                    ITCode = "admin",
                    Password = Utils.GetMD5String("000000"),
                    IsValid = true,
                    Name = Core.CoreProgram._localizer["Admin"]
                };

                var userrole = new FrameworkUserRole
                {
                    User = user,
                    Role = Set<FrameworkRole>().Where(x => x.RoleCode == "001").FirstOrDefault()
                };
                Set<FrameworkUserBase>().Add(user);
                Set<FrameworkUserRole>().Add(userrole);
                await SaveChangesAsync();
            }
            return state;
        }

    }
}
