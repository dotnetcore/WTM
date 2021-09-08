using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// TopBasePoco
    /// </summary>
    public class TopBasePoco
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

        public object GetID()
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            var id = idpro.GetValue(this);
            return id;
        }

        public bool HasID()
        {
            bool rv = false;
            var id = this.GetID();
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

        public object GetParentID()
        {
            var idpro = this.GetType().GetSingleProperty("ParentId");
            var id = idpro.GetValue(this) ?? "";
            return id;
        }


        public Type GetIDType()
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            return idpro.PropertyType;
        }

        public void SetID(object id)
        {
            var idpro = this.GetType().GetSingleProperty("ID");
            idpro.SetValue(this, id.ConvertValue(idpro.PropertyType));

        }

        private bool? _isBasePoco = null;
        [NotMapped]
        [JsonIgnore]
        public bool IsBasePoco
        {
            get
            {
                if(_isBasePoco == null)
                {
                    _isBasePoco = typeof(IBasePoco).IsAssignableFrom(this.GetType());
                }
                return _isBasePoco.Value;
            }
        }
    }


}
