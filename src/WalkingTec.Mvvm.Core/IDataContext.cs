using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// IDataContext
    /// </summary>
    public interface IDataContext : IDisposable
    {
        /// <summary>
        /// IsFake
        /// </summary>
        bool IsFake { get; set; }

        /// <summary>
        /// 是否为调试模式
        /// </summary>
        bool IsDebug { get; set; }
        /// <summary>
        /// 当前租户代码，用于多租户数据隔离
        /// </summary>
        string TenantCode { get; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        DBTypeEnum DBType { get; set; }
        /// <summary>
        /// AddEntity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void AddEntity<T>(T entity) where T : TopBasePoco;

        /// <summary>
        /// UpdateEntity
        /// </summary>
        void UpdateEntity<T>(T entity) where T : TopBasePoco;

        /// <summary>
        /// UpdateProperty
        /// </summary>
        void UpdateProperty<T>(T entity, Expression<Func<T, object>> fieldExp) where T : TopBasePoco;

        /// <summary>
        /// UpdateProperty
        /// </summary>
        void UpdateProperty<T>(T entity, string fieldName) where T : TopBasePoco;

        /// <summary>
        /// DeleteEntity
        /// </summary>
        void DeleteEntity<T>(T entity) where T : TopBasePoco;

        /// <summary>
        /// CascadeDelete
        /// </summary>
        void CascadeDelete<T>(T entity) where T : TreePoco;

        /// <summary>
        /// Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Model
        /// </summary>
        IModel Model { get; }

        /// <summary>
        /// Database
        /// </summary>
        DatabaseFacade Database { get; }

        /// <summary>
        /// CSName
        /// </summary>
        string CSName { get; set; }

        #region SaveChange

        /// <summary>
        /// SaveChanges
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// SaveChanges
        /// </summary>
        /// <returns></returns>
        int SaveChanges(bool acceptAllChangesOnSuccess);

        /// <summary>
        /// SaveChangesAsync
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// SaveChangesAsync
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="AllModel"></param>
        /// <param name="IsSpa"></param>
        /// <returns>返回true即数据新建完成，进入初始化操作，返回false即数据库已经存在</returns>
        Task<bool> DataInit(object AllModel, bool IsSpa);

        /// <summary>
        /// 确保数据库已被创建，不存在则创建
        /// </summary>
        void EnsureCreate();
        /// <summary>
        /// 创建一个同类型的新数据上下文实例
        /// </summary>
        /// <returns>新的数据上下文实例</returns>
        IDataContext CreateNew();
        /// <summary>
        /// 重建数据上下文，若当前存在活动事务则直接返回自身
        /// </summary>
        /// <param name="_logger">日志工厂，传入时开启调试日志</param>
        /// <returns>重建后的数据上下文</returns>
        IDataContext ReCreate(ILoggerFactory _logger = null);

        /// <summary>
        /// 执行存储过程，返回datatable
        /// </summary>
        /// <param name="command">存储过程名称</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        DataTable RunSP(string command, params object[] paras);
        /// <summary>
        /// 执行存储过程，返回实体列表
        /// </summary>
        /// <typeparam name="TElement">返回的实体类型</typeparam>
        /// <param name="command">存储过程名称</param>
        /// <param name="paras">存储过程参数</param>
        /// <returns>实体列表</returns>
        IEnumerable<TElement> RunSP<TElement>(string command, params object[] paras);

        /// <summary>
        /// 执行sql语句，返回datatable
        /// </summary>
        /// <param name="command">查询sql语句</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        DataTable RunSQL(string command, params object[] paras);
        /// <summary>
        /// 执行Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TElement">返回的实体类型</typeparam>
        /// <param name="sql">查询Sql语句</param>
        /// <param name="paras">参数</param>
        /// <returns>实体列表</returns>
        IEnumerable<TElement> RunSQL<TElement>(string sql, params object[] paras);
        /// <summary>
        /// 执行存储过程或Sql语句，返回DataTable
        /// </summary>
        /// <param name="sql">存储过程名称或Sql语句</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paras">参数</param>
        /// <returns>DataTable结果集</returns>
        DataTable Run(string sql, CommandType commandType, params object[] paras);
        /// <summary>
        /// 执行存储过程或Sql语句，返回实体列表
        /// </summary>
        /// <typeparam name="TElement">返回的实体类型</typeparam>
        /// <param name="sql">存储过程名称或Sql语句</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="paras">参数</param>
        /// <returns>实体列表</returns>
        IEnumerable<TElement> Run<TElement>(string sql, CommandType commandType, params object[] paras);
        /// <summary>
        /// 根据数据库类型创建对应的命令参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="dir">参数方向</param>
        /// <returns>数据库命令参数</returns>
        object CreateCommandParameter(string name, object value, ParameterDirection dir);

        /// <summary>
        /// 设置日志工厂，用于输出EF Core日志
        /// </summary>
        /// <param name="factory">日志工厂</param>
        void SetLoggerFactory(ILoggerFactory factory);
        /// <summary>
        /// 设置租户代码，用于多租户数据隔离
        /// </summary>
        /// <param name="tc">租户代码</param>
        void SetTenantCode(string tc);
    }
}
