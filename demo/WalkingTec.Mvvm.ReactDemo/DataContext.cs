using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.ReactDemo.Models;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.ReactDemo
{
    public class DataContext : FrameworkContext
    {
        public DbSet<Major> Majors { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<City> Cities { get; set; }

        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }
    }
}
