using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.ApiDemo
{
    public class DataContext : FrameworkContext
    {
        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }
#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole()));

        }

#endif
    }
}
