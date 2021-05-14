using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.Models.Virus;

namespace WalkingTec.Mvvm.BlazorDemo.DataAccess
{
    public class DataContext : FrameworkContext
    {
        public DataContext(CS cs)
             : base(cs)
        {
        }

        public DataContext(string cs, DBTypeEnum dbtype)
            : base(cs, dbtype)
        {
        }

        public DataContext(string cs, DBTypeEnum dbtype, string version = null)
            : base(cs, dbtype, version)
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<FrameworkUser> FrameworkUsers { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<Virus> Viruses { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<ControlCenter> ControlCenters { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Report> Reports { get; set; }

        public override async Task<bool> DataInit(object allModules, bool IsSpa)
        {
            var state = await base.DataInit(allModules, IsSpa);
            bool emptydb = false;

            try
            {
                emptydb = Set<FrameworkUser>().Count() == 0 && Set<FrameworkUserRole>().Count() == 0;
            }
            catch { }

            if (state == true || emptydb == true)
            {
                //when state is true, means it's the first time EF create database, do data init here
                //当state是true的时候，表示这是第一次创建数据库，可以在这里进行数据初始化
                var user = new FrameworkUser
                {
                    ITCode = "admin",
                    Password = Utils.GetMD5String("000000"),
                    IsValid = true,
                    Name = "Admin"
                };

                var userrole = new FrameworkUserRole
                {
                    UserCode = user.ITCode,
                    RoleCode = "001"
                };

                Set<FrameworkUser>().Add(user);
                Set<FrameworkUserRole>().Add(userrole);
                Set<FrameworkGroup>().Add(new FrameworkGroup
                {
                    GroupCode = "1",
                    GroupName = "学生组"
                });
                Set<FrameworkGroup>().Add(new FrameworkGroup
                {
                    GroupCode = "2",
                    GroupName = "老师组"
                });
                Set<City>().Add(new City
                {
                    Name = "北京"
                });
                Set<City>().Add(new City
                {
                    Name = "上海"
                });
                Set<City>().Add(new City
                {
                    Name = "广东"
                });

                var adminmenus = Set<FrameworkMenu>().Where(x => x.Url != null && x.Url.StartsWith("/api") == false).ToList();
                foreach (var item in adminmenus)
                {
                    item.Url = "/_admin" + item.Url;
                }
                await SaveChangesAsync();
            }
            return state;
        }

    }
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            return new DataContext("你的完整连接字符串", DBTypeEnum.SqlServer);
        }
    }
}
