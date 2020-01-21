using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;

namespace WalkingTec.Mvvm.Demo
{
    public class DataContext2 : EmptyContext
    {
        public DataContext2(CS cs)
             : base(cs)
        {
        }


        public DbSet<MyUser> MyUsers { get; set; }


    }
}
