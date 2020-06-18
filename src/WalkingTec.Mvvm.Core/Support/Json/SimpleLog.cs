using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.Core.Support.Json
{

    public class SimpleLog
    {

        public ActionLog GetActionLog()
        {
            return new ActionLog
            {
                ActionName = this.ActionName,
                ActionTime = this.ActionTime,
                ActionUrl = this.ActionUrl,
                CreateBy = this.CreateBy,
                CreateTime = this.CreateTime,
                Duration = this.Duration,
                IP = this.IP,
                ITCode = this.ITCode,
                LogType = this.LogType,
                ModuleName = this.ModuleName,
                Remark = this.Remark,
                UpdateBy = this.UpdateBy,
                UpdateTime = this.UpdateTime,
            };
        }

        public string ModuleName { get; set; }

        public string ActionName { get; set; }

        public string ITCode { get; set; }

        public string ActionUrl { get; set; }

        public DateTime ActionTime { get; set; }

        public double Duration { get; set; }

        public string Remark { get; set; }

        public string IP { get; set; }

        public ActionLogTypesEnum LogType { get; set; }

        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// CreateBy
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// UpdateBy
        /// </summary>
        public string UpdateBy { get; set; }
    }
}
