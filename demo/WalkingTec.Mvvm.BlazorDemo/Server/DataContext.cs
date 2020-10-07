using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.BlazorDemo.Server
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
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    }
}
