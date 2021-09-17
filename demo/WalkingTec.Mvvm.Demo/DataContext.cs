using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.Models.Virus;

namespace WalkingTec.Mvvm.Demo
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

        public DbSet<Major> Majors { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<WxReportData> WxReportDatas { get; set; }
        public DbSet<不要用中文模型名> 不要中文 { get; set; }
        public DbSet<ISOType> ISOTypes { get; set; }
        public DbSet<SoftFacInfo> SoftFacInfos { get; set; }
        public DbSet<Virus> Viruses { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<ControlCenter> ControlCenters { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<LinkTest> LinkTests { get; set; }
        public DbSet<LinkTest2> LinkTest2 { get; set; }
        public DbSet<TreeTest> TreeTests { get; set; }
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
                await SaveChangesAsync();

                Dictionary<string, List<object>> data = new Dictionary<string, List<object>>();
                SetTestData(typeof(School), data);
                SetTestData(typeof(Major), data);
                SetTestData(typeof(Student), data);
                SetTestData(typeof(School), data);
                SetTestData(typeof(City), data);
                SetTestData(typeof(ControlCenter), data);
                SetTestData(typeof(Hospital), data);
                SetTestData(typeof(Patient), data);
                SetTestData(typeof(Virus), data);
                SetTestData(typeof(Report), data);
            }
            return state;
        }

        private void SetTestData(Type modelType, Dictionary<string, List<object>> data, int count = 100)
        {
            if (data.ContainsKey(modelType.FullName) && data[modelType.FullName].Count>=count)
            {
                return;
            }
            using (var dc = this.CreateNew())
            {
                Random r = new Random();
                data[modelType.FullName] = new List<object>();
                int retry = 0;
                List<string> ids = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    var modelprops = modelType.GetRandomValuesForTestData();
                    var newobj = modelType.GetConstructor(Type.EmptyTypes).Invoke(null);
                    var idvalue = modelprops.Where(x => x.Key == "ID").Select(x=>x.Value).SingleOrDefault();
                    if (idvalue != null )
                    {
                        if (ids.Contains(idvalue.ToLower()) == false)
                        {
                            ids.Add(idvalue.ToLower());
                        }
                        else
                        {
                            retry++;
                            i--;
                            if (retry > count)
                            {
                                break;
                            }
                            continue;
                        }
                    }
                    foreach (var pro in modelprops)
                    {
                        if (pro.Value == "$fk$")
                        {
                            var fktype = modelType.GetSingleProperty(pro.Key[0..^2])?.PropertyType;
                            if (fktype != modelType)
                            {
                                SetTestData(fktype, data);
                                newobj.SetPropertyValue(pro.Key, (data[fktype.FullName][r.Next(0, data[fktype.FullName].Count)] as TopBasePoco).GetID());
                            }
                        }
                        else
                        {
                            var v = pro.Value;
                            if (v.StartsWith("\""))
                            {
                                v = v[1..];
                            }
                            if (v.EndsWith("\""))
                            {
                                v = v[..^1];
                            }
                            newobj.SetPropertyValue(pro.Key, v);
                        }
                    }
                    if(modelType == typeof(FileAttachment))
                    {
                        newobj.SetPropertyValue("Path", "./wwwroot/logo.png");
                        newobj.SetPropertyValue("SaveMode", "local");
                        newobj.SetPropertyValue("Length", 16728);
                    }
                    try
                    {
                        (dc as DbContext).Add(newobj);
                        data[modelType.FullName].Add(newobj);
                    }
                    catch
                    {
                        retry++;
                        i--;
                        if(retry > count)
                        {
                            break;
                        }
                    }
                }
                int a = dc.SaveChanges();
            }
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
