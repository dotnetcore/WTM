using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
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
        /// <returns>返回true即数据新建完成，进入初始化操作，返回false即数据库已经存在</returns>
        Task<bool> DataInit(object AllModel);

        IDataContext CreateNew();
        IDataContext ReCreate();

    }
}
