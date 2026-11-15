using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// TopBasePoco
    /// </summary>
    public  class TopBasePoco
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public Guid ID
        {
            get; set;
        }

        /// <summary>
        /// 是否选中
        /// 标识当前行数据是否被选中
        /// </summary>
        [NotMapped]
        //[JsonConverter(typeof(InternalBoolConverter))]
        //[JsonProperty("LAY_CHECKED")]
        [JsonIgnore]
        public bool Checked { get; set; }

        /// <summary>
        /// BatchError
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public string BatchError { get; set; }

        /// <summary>
        /// ExcelIndex
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public long ExcelIndex { get; set; }

        /// <summary>
        /// 通过反射获取当前实体的 ID 属性值
        /// </summary>
        /// <returns>实体的 ID 值</returns>
        public object GetID()
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            var id = idpro.GetValue(this);
            return id;
        }

        /// <summary>
        /// 判断当前实体是否已被赋予有效的 ID 值
        /// </summary>
        /// <returns>存在有效 ID 时返回 true，否则返回 false</returns>
        public bool HasID()
        {
            bool rv = false;
            var id = this.GetID();
            // 按主键类型分别判断：Guid 非空、字符串非空、int/long 大于 0 均视为有效 ID
            switch (id)
            {
                case Guid g1 when g1 != Guid.Empty:
                    rv = true;
                    break;
                case string s when string.IsNullOrEmpty(s) == false:
                    rv = true;
                    break;
                case int i when i>0:
                    rv = true;
                    break;
                case long l when l > 0:
                    rv = true;
                    break;
            }
            return rv;
        }

        /// <summary>
        /// 通过反射获取当前实体的父节点 ID 属性值（用于树形结构）
        /// </summary>
        /// <returns>父节点 ID；不存在或未赋值时返回空字符串</returns>
        public object GetParentID()
        {
            var idpro = this.GetType().GetSingleProperty("ParentId");
            // 父 ID 为 null 时返回空字符串，避免调用方额外处理 null
            var id = idpro.GetValue(this) ?? "";
            return id;
        }


        /// <summary>
        /// 通过反射获取当前实体 ID 属性的类型
        /// </summary>
        /// <returns>ID 属性的 Type</returns>
        public Type GetIDType()
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            return idpro.PropertyType;
        }

        /// <summary>
        /// 通过反射设置当前实体的 ID 属性值，设置前会按属性类型自动转换
        /// </summary>
        /// <param name="id">要设置的 ID 值</param>
        public void SetID(object id)
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            idpro.SetValue(this, id.ConvertValue(idpro.PropertyType));

        }

        private bool? _isBasePoco = null;
        /// <summary>
        /// 当前实体类型是否实现了 IBasePoco 接口（即是否包含创建、更新等审计字段）
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public bool IsBasePoco
        {
            get
            {
                // 首次访问时通过反射判断并缓存结果，避免重复反射带来的性能开销
                if(_isBasePoco == null)
                {
                    _isBasePoco = typeof(IBasePoco).IsAssignableFrom(this.GetType());
                }
                return _isBasePoco.Value;
            }
        }
    }


}
