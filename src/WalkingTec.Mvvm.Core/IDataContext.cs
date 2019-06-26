using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

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
        void CascadeDelete<T>(T entity) where T : TopBasePoco, ITreeData<T>;

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

        IDataContext CreateNew();
        IDataContext ReCreate();

        /// <summary>
        /// 执行存储过程，返回datatable
        /// </summary>
        /// <param name="command">存储过程名称</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        DataTable RunSP(string command, params object[] paras);
        IEnumerable<TElement> RunSP<TElement>(string command, params object[] paras);

        /// <summary>
        /// 执行sql语句，返回datatable
        /// </summary>
        /// <param name="command">查询sql语句</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        DataTable RunSQL(string command, params object[] paras);
        IEnumerable<TElement> RunSQL<TElement>(string sql, params object[] paras);
        DataTable Run(string sql, CommandType commandType, params object[] paras);
        IEnumerable<TElement> Run<TElement>(string sql, CommandType commandType, params object[] paras);
        object CreateCommandParameter(string name, object value, ParameterDirection dir);
    }
}
