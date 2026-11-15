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
            where T : TreePoco<T>
        {
            List<T> rv = new List<T>();
            var children = self.Children;
            if(order != null && children != null)
            {
                children = children.OrderBy(order).ToList();
            }
            if (children != null && children.Count() > 0)
            {
                //var dictinct = children.Where(x => x.GetID().ToString() != self.GetID().ToString()).ToList();
                foreach (var item in children)
                {
                    rv.Add(item);
                    //递归添加子节点的子节点
                    rv.AddRange(item.GetAllChildren(order));
                }
            }
            return rv;
        }

        /// <summary>
        /// 获取当前节点在树形结构中所处的层级，根节点为 0 级
        /// </summary>
        /// <typeparam name="T">树形结构类</typeparam>
        /// <param name="self">树形结构实例</param>
        /// <returns>当前节点的层级，根节点返回 0</returns>
        public static int GetLevel<T>(this T self)
            where T : TreePoco<T>
        {
            int level = 0;
            //沿父节点一路向上遍历到根节点，每上一层层级加一
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
            where T : TreePoco<T>
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
            where T :TreePoco<T>
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
        /// 将单个树形结构节点转变为标准列表
        /// </summary>
        /// <typeparam name="T">树形结构类</typeparam>
        /// <param name="self">树形结构实例</param>
        /// <param name="order">排序字段，可以为空</param>
        /// <returns>返回标准列表，包含本节点及其所有子节点，所有节点都在同一级上</returns>
        public static List<T> FlatTree<T>(this T self, Func<T, object> order = null)
    where T : TreePoco<T>
        {
            List<T> rv = new List<T>();
            rv.Add(self);
            rv.AddRange(self.GetAllChildren(order));
            return rv;
        }

        /// <summary>
        /// 将标准列表组装成树形结构，返回顶层节点列表，子节点挂载在各节点的 Children 属性上
        /// </summary>
        /// <typeparam name="T">树形结构类</typeparam>
        /// <param name="self">树形结构实例列表</param>
        /// <param name="order">排序字段，可以为空</param>
        /// <returns>树形结构列表，只包含顶层节点，子节点存放在各节点的 Children 属性中</returns>
        public static List<T> MakeTree<T>(this List<T> self, Func<T, object> order = null)
      where T : TreePoco<T>
        {
            List<T> rv = new List<T>();
            if (order != null)
            {
                self = self.OrderBy(order).ToList();
            }
            foreach (var item in self)
            {
                //初始化 Children 列表，避免后续添加子节点时出现空引用
                if(item.Children == null)
                {
                    item.Children = new List<T>();
                }
                //从列表中找出父节点为当前节点的数据作为其子节点，并反向设置 Parent 属性
                var children = self.Where(x => x.ParentId == item.ID).ToList();
                children.ForEach(x =>x.Parent = item);
                item.Children.AddRange(children);
                rv.Add(item);                
            }
            //没有父节点的即为顶层节点，只返回顶层节点以形成树形结构
            return rv.Where(x=>x.ParentId == null).ToList();
        }
        /// <summary>
        /// 将树形结构列表转变为标准列表
        /// </summary>
        /// <param name="self">树形结构实例</param>
        /// <param name="order">排序字段，可以为空</param>
        /// <returns>返回标准列表，所有节点都在同一级上</returns>
        public static IEnumerable<TreeSelectListItem> FlatTreeSelectList(this IEnumerable<TreeSelectListItem> self, Func<TreeSelectListItem, object> order = null)
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
                var dictinct = children.Where(x => x.Value != self.Value).ToList();
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
