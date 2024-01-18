using Elsa.Persistence.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    public class WtmElsaContext : ElsaContext
    {
        public WtmElsaContext(DbContextOptions options) : base(options)
        {
        }

        public override string Schema
        {
            get
            {
                if (Database.IsOracle())
                {
                    return null;
                }
                else
                {
                    return base.Schema;
                }
            }
        }
    }
}
