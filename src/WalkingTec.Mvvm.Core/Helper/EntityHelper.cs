using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DataTable和Entity之间转换的辅助类
    /// </summary>
    public static class EntityHelper
    {
        /// <summary>
        /// 根据DataTable获取Entity列表
        /// </summary>
        /// <typeparam name="T">Entity类型</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>Entity列表</returns>
        public static IList<T> GetEntityList<T>(DataTable table)
        {
            IList<T> entityList = new List<T>();

            var properties = typeof(T).GetProperties().ToLookup(property => property.Name, property => property).ToDictionary(i => i.Key, i => i.First()).Values;

            //循环Datable中的每一行
            foreach (DataRow row in table.Rows)
            {
                //新建Entity
                T entity = (T)Activator.CreateInstance(typeof(T));
                //循环Entity的每一个属性
                foreach (var item in properties)
                {
                    //如果DataTable中有列名和属性名一致，则把单元格内容赋值给Entity的该属性
                    if (row.Table.Columns.Contains(item.Name))
                    {
                        //判断null值
                        if (string.IsNullOrEmpty(row[item.Name].ToString()))
                        {
                            item.SetValue(entity, null);
                        }
                        else
                        {
                            var ptype = item.PropertyType;
                            if (ptype.IsNullable())
                            {
                                ptype = ptype.GenericTypeArguments[0];
                            }
                            //如果是Guid或Guid?类型
                            if (ptype == typeof(Guid))
                            {
                                item.SetValue(entity, Guid.Parse(row[item.Name].ToString()));
                            }
                            //如果是enum或enum?类型
                            else if (ptype.IsEnum)
                            {
                                item.SetValue(entity, Enum.ToObject(ptype, row[item.Name]));
                            }
                            else
                            {
                                item.SetValue(entity, Convert.ChangeType(row[item.Name], ptype));
                            }

                        }
                    }
                }
                entityList.Add(entity);
            }
            return entityList;
        }

        #region 实体类转换成DataTable

        /// <summary>
        /// 实体类转换成DataSet
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns>DataSet</returns>
        public static DataSet ToDataSet<T>(List<T> modelList) where T : new()
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            else
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(ToDataTable(modelList));
                return ds;
            }
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(List<T> modelList) where T : new()
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                //循环实体类所有属性，给对应的DataTable字段赋值
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    var res = propertyInfo.GetValue(model);
                    dataRow[propertyInfo.Name] = res ?? DBNull.Value;
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns>DataTable</returns>
        private static DataTable CreateData<T>(T model) where T : new()
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            var types = typeof(T).GetProperties();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    if (propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && propertyInfo.PropertyType.GenericTypeArguments.Length > 0)
                    {
                        dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType.GenericTypeArguments[0]);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
                dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
            }
            return dataTable;
        }

        #endregion
    }
}
