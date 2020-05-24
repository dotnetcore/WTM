using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// IBaseVM
    /// </summary>
    public interface IBaseVM
    {
        #region Property
        /// <summary>
        /// UniqueId
        /// </summary>
        string UniqueId { get; }
        /// <summary>
        /// WindowIds
        /// </summary>
        string WindowIds { get; set; }
        /// <summary>
        /// ViewDivId
        /// </summary>
        string ViewDivId { get; set; }
        /// <summary>
        /// DC
        /// </summary>
        IDataContext DC { get; set; }
        /// <summary>
        /// VMFullName
        /// </summary>
        string VMFullName { get; }
        /// <summary>
        /// CreatorAssembly
        /// </summary>
        string CreatorAssembly { get; set; }
        /// <summary>
        /// CurrentCS
        /// </summary>
        string CurrentCS { get; }
        /// <summary>
        /// FromFixedCon
        /// </summary>
        bool FromFixedCon { get; set; }
        /// <summary>
        /// FC
        /// </summary>
        Dictionary<string, object> FC { get; set; }
        ///// <summary>
        ///// Config
        ///// </summary>
        //Configs Config { get; set; }
        IModelStateService MSD { get; set; }
        ISessionService Session { get; set; }

        IDistributedCache Cache { get; set; }

        LoginUserInfo LoginUserInfo { get; set; }
        #endregion

        #region Event

        /// <summary>
        /// InitVM 完成后触发的事件
        /// </summary>
        event Action<IBaseVM> OnAfterInit;
        /// <summary>
        /// ReInitVM 完成后触发的事件
        /// </summary>
        event Action<IBaseVM> OnAfterReInit;

        #endregion

        #region Method
        /// <summary>
        /// 调用 InitVM 并触发 OnAfterInit 事件
        /// </summary>
        void DoInit();

        /// <summary>
        /// 调用 ReInitVM 并触发 OnAfterReInit 事件
        /// </summary>
        void DoReInit();
        #endregion
    }
}
