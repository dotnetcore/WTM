using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkUser
    /// </summary>
    [Table("FrameworkTenants")]
    public class FrameworkTenant : BasePoco, ITenant
    {
        /// <summary>
        /// 租户编码，唯一
        /// </summary>
        [Display(Name = "_Admin.TenantCode")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TCode { get; set; }

        /// <summary>
        /// 租户名称
        /// </summary>
        [Display(Name = "_Admin.TenantName")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TName { get; set; }

        /// <summary>
        /// 租户独立数据库的连接字符串，为空表示与主库共用数据库
        /// </summary>
        [Display(Name = "_Admin.TenantDb")]
        public string TDb { get; set; }

        /// <summary>
        /// 租户独立数据库的类型，为空表示与主库类型一致
        /// </summary>
        [Display(Name = "_Admin.TenantDbType")]
        public DBTypeEnum? TDbType { get; set; }

        /// <summary>
        /// 租户使用的 DataContext 类型名称，为空时使用默认的 DataContext
        /// </summary>
        [Display(Name = "_Admin.TenantDbContext")]
        public string DbContext { get; set; }

        /// <summary>
        /// 租户域名，用于通过域名识别并切换到对应租户
        /// </summary>
        [Display(Name = "_Admin.TenantDomain")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TDomain { get; set; }

        /// <summary>
        /// 所属上级租户的编码，启用子租户功能时用于标识租户层级
        /// </summary>
        [Display(Name = "_Admin.Tenant")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string TenantCode { get; set; }

        /// <summary>
        /// 是否允许该租户创建子租户
        /// </summary>
        [Display(Name = "_Admin.TenantEnableSub")]
        public bool EnableSub { get; set; }

        /// <summary>
        /// 是否启用该租户，默认为启用
        /// </summary>
        [Display(Name = "Sys.Enable")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 获取该租户是否配置了独立数据库；连接字符串与数据库类型均已设置时视为使用独立数据库
        /// </summary>
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

        /// <summary>
        /// 租户扩展属性字典，不映射到数据库，用于运行时携带租户的额外数据
        /// </summary>
        [NotMapped]
        public Dictionary<string, object> Attributes { get; set; }


        /// <summary>
        /// 为该租户创建数据上下文。
        /// 未配置独立数据库时直接基于当前上下文创建并绑定租户编码；配置了独立数据库时通过反射构造指定的 DataContext 实例
        /// </summary>
        /// <param name="wtm">当前的 WTM 上下文对象</param>
        /// <returns>租户专用的数据上下文；若使用了独立数据库但未找到匹配的 DataContext 构造函数则返回 null</returns>
        public IDataContext CreateDC(WTMContext wtm)
        {
            // 未配置独立数据库：直接复用当前数据上下文，仅绑定租户编码
            if (IsUsingDB == false)
            {
                var rv = wtm.CreateDC();
                rv.SetTenantCode(TCode);
                return rv;
            }
            else
            {
                // 未指定 DbContext 时默认使用 DataContext；否则使用租户配置的上下文类型
                var context = string.IsNullOrEmpty(this.DbContext) ? "DataContext" : this.DbContext;
                // 通过反射查找指定名称的 DataContext 构造函数，并使用租户的连接字符串与数据库类型实例化
                var DcConstructor = CS.CisFull.Where(x => x.DeclaringType.Name.ToLower() == context.ToLower()).FirstOrDefault();
                var tenantdc = (IDataContext)DcConstructor?.Invoke(new object[] { this.TDb, this.TDbType });
                if (tenantdc == null)
                {
                    return null;
                }
                // 将当前环境的调试与日志设置复制到租户上下文，并绑定租户编码
                tenantdc.IsDebug = wtm.ConfigInfo.IsQuickDebug;
                if (wtm.LoggerFactory != null)
                {
                    tenantdc.SetLoggerFactory(wtm.LoggerFactory);
                }
                tenantdc.SetTenantCode(this.TCode);
                return tenantdc;
            }
        }
    }
}
