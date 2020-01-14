using System;
using System.Collections.Generic;
using System.Linq;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// 树形结构Model的扩展函数
    /// </summary>
    public static class ITreeDataExtension
    {

        /// <summary>
        /// 获取一个父节点下的所有子节点，包括子节点的子节点
        /// </summary>
        /// <typeparam name="T">树形结构类</typeparam>
        /// <param name="self">树形结构实例</param>
        /// <param name="order">排序字段，可为空</param>
        /// <returns>树形结构列表，包含所有子节点</returns>
        public static List<T> GetAllChildren<T>(this T self, Func<T, object> order = null)
            where T : TopBasePoco, ITreeData<T>
        {
            List<T> rv = new List<T>();
            var children = self.Children;
            if(order != null && children != null)
            {
                children = children.OrderBy(order).ToList();
            }
            if (children != null && children.Count() > 0)
            {
                var dictinct = children.Where(x => x.ID != self.ID).ToList();
                foreach (var item in dictinct)
                {
                    rv.Add(item);
                    //递归添加子节点的子节点
                    rv.AddRange(item.GetAllChildren(order));
                }
            }
            return rv;
        }

        public static int GetLevel<T>(this T self)
            where T : TopBasePoco, ITreeData<T>
        {
            int level = 0;
            while (self.Parent != null)
            {
                level++;
                self = self.Parent;
            }
            return level;
        }

        /// <summary>
        /// 查询数据库，根据某个节点ID递归获取其下所有级别的子节点ID
        /// </summary>
        /// <typeparam name="T">树形结构类</typeparam>
        /// <param name="self">树形结构实例</param>
        /// <param name="dc">dc</param>
        /// <param name="subids">子节点ID列表</param>
        /// <returns>所有级别子节点ID</returns>
        public static List<Guid> GetAllChildrenIDs<T>(this T self
            , IDataContext dc
            , List<Guid> subids = null)
            where T : TopBasePoco, ITreeData<T>
        {
            List<Guid> rv = new List<Guid>();
            List<Guid> ids = null;
            if (subids == null)
            {
                ids = dc.Set<T>().Where(x => x.ParentId == self.ID).Select(x => x.ID).ToList();
            }
            else
            {
                ids = dc.Set<T>().Where(x => subids.Contains(x.ParentId.Value)).Select(x => x.ID).ToList();
            }
            if (ids != null && ids.Count > 0)
            {
                rv.AddRange(ids);
                rv.AddRange(self.GetAllChildrenIDs(dc, ids));
            }
            return rv;
        }

        /// <summary>
        /// 将树形结构列表转变为标准列表
        /// </summary>
        /// <typeparam name="T">树形结构类</typeparam>
        /// <param name="self">树形结构实例</param>
        /// <param name="order">排序字段，可以为空</param>
        /// <returns>返回标准列表，所有节点都在同一级上</returns>
        public static List<T> FlatTree<T>(this List<T> self, Func<T,object> order = null)
            where T : TopBasePoco, ITreeData<T>
        {
            List<T> rv = new List<T>();
            if(order != null)
            {
                self = self.OrderBy(order).ToList();
            }
            foreach (var item in self)
            {
                rv.Add(item);
                rv.AddRange(item.GetAllChildren(order));
            }
            return rv;
        }

        /// <summary>
        /// 将树形结构列表转变为标准列表
        /// </summary>
        /// <param name="self">树形结构实例</param>
        /// <param name="order">排序字段，可以为空</param>
        /// <returns>返回标准列表，所有节点都在同一级上</returns>
        public static List<TreeSelectListItem> FlatTreeSelectList(this List<TreeSelectListItem> self, Func<TreeSelectListItem, object> order = null)
        {
            List<TreeSelectListItem> rv = new List<TreeSelectListItem>();
            if (order != null)
            {
                self = self.OrderBy(order).ToList();
            }
            foreach (var item in self)
            {
                rv.Add(item);
                if (item.Children != null)
                {
                    rv.AddRange(item.GetTreeSelectChildren(order));
                }
            }
            return rv;
        }

        /// <summary>
        /// 获取TreeSelect节点下所有子节点
        /// </summary>
        /// <param name="self"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static List<TreeSelectListItem> GetTreeSelectChildren(this TreeSelectListItem self, Func<TreeSelectListItem, object> order = null)
        {
            List<TreeSelectListItem> rv = new List<TreeSelectListItem>();
            var children = self.Children;
            if (order != null && children != null)
            {
                children = children.OrderBy(order).ToList();
            }
            if (children != null && children.Count() > 0)
            {
                var dictinct = children.Where(x => x.Id != self.Id).ToList();
                foreach (var item in dictinct)
                {
                    rv.Add(item);
                    //递归添加子节点的子节点
                    rv.AddRange(item.GetTreeSelectChildren(order));
                }
            }
            return rv;
        }


    }
}
