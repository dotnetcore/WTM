using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkUser
    /// </summary>
    [Table("FrameworkTenants")]
    public class FrameworkTenant : BasePoco, ITenant
    {
        [Display(Name = "_Admin.TenantCode")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TCode { get; set; }

        [Display(Name = "_Admin.TenantName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TName { get; set; }

        [Display(Name = "_Admin.TenantDb")]
        public string TDb { get; set; }

        [Display(Name = "_Admin.TenantDbType")]
        public DBTypeEnum? TDbType { get; set; }

        [Display(Name = "_Admin.TenantDbContext")]
        public string DbContext { get; set; }

        [Display(Name = "_Admin.TenantDomain")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TDomain { get; set; }

        [Display(Name = "_Admin.Tenant")]
        public string TenantCode { get; set; }

        [Display(Name = "_Admin.TenantEnableSub")]
        public bool EnableSub { get; set; }

        [Display(Name = "Sys.Enable")]
        public bool Enabled { get; set; } = true;

        [NotMapped]
        public bool IsUsingDB
        {
            get
            {
                if (string.IsNullOrEmpty(TDb) == false && TDbType != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public IDataContext CreateDC(bool isdebug, ILoggerFactory loggerFactory)
        {
            var context = string.IsNullOrEmpty(this.DbContext) ? "DataContext" : this.DbContext;
            var DcConstructor = CS.CisFull.Where(x => x.DeclaringType.Name.ToLower() == context.ToLower()).FirstOrDefault();
            var tenantdc = (IDataContext)DcConstructor?.Invoke(new object[] { this.TDb, this.TDbType });
            if (tenantdc == null)
            {
                return null;
            }
            tenantdc.IsDebug = isdebug;
            if (loggerFactory != null)
            {
                tenantdc.SetLoggerFactory(loggerFactory);
            }
            tenantdc.SetTenantCode(this.TCode);
            return tenantdc;

        }
    }
}
