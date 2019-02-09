using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WebApplication9
{
    public class DataContext : FrameworkContext
    {
        public DataContext(string cs, DBTypeEnum dbtype)
             : base(cs, dbtype)
        {
        }

    }
}
