using Elsa.Persistence.EntityFramework.Core;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Mvc.Helper
{
    /// <summary>
    /// WTM 对 Elsa 工作流持久化 DbContext 的封装，针对 Oracle 数据库禁用独立 Schema
    /// </summary>
    public class WtmElsaContext : ElsaContext
    {
        /// <summary>
        /// 使用指定的 DbContext 选项构造实例
        /// </summary>
        /// <param name="options">DbContext 配置选项</param>
        public WtmElsaContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// 获取数据库架构名；Oracle 数据库返回 null（不使用独立 Schema），其他数据库沿用 Elsa 默认值
        /// </summary>
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
