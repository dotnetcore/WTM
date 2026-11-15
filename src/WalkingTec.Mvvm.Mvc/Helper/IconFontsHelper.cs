using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 图标字体辅助类，扫描站点目录下的 CSS 文件，
    /// 解析其中自定义图标字体的定义并生成下拉框与菜单可用的数据源
    /// </summary>
    public static class IconFontsHelper
    {
        // 图标字体下拉框数据源缓存
        private static List<ComboSelectListItem> _iconFontItems;
        /// <summary>
        /// 获取图标字体的下拉框数据源列表
        /// </summary>
        public static List<ComboSelectListItem> IconFontItems
        {
            get
            {
                // 每次获取前重置选中状态，避免上一次的选中项残留
                foreach (var item in _iconFontItems.Where(x => x.Selected == true))
                    item.Selected = false;
                return _iconFontItems;
            }
        }

        // 按图标字体名称分类的菜单数据源缓存
        private static Dictionary<string, List<MenuItem>> _iconFontDicItems;
        /// <summary>
        /// 获取按图标字体名称分类的图标菜单数据源字典
        /// </summary>
        public static Dictionary<string, List<MenuItem>> IconFontDicItems => _iconFontDicItems;

        /// <summary>
        /// 扫描指定目录（默认为 wwwroot/font 与 wwwroot/layui）下的 CSS 文件，
        /// 解析其中的图标字体定义并生成下拉框与菜单数据源
        /// </summary>
        /// <param name="dirs">要扫描的目录数组，不传时使用默认目录</param>
        public static void GenerateIconFont(params string[] dirs)
        {
            // 未显式指定目录时使用默认的字体目录
            var baseDirs = new string[] { "wwwroot/font", "wwwroot/layui" };
            if(dirs.Length > 0)
            {
                baseDirs = dirs;
            }
            // 收集全部图标字体名称，并按字体分类记录其图标项
            var iconFontHashSet = new HashSet<string>();
            var IconFontDic = new Dictionary<string, string[]>();
            // 逐个目录递归扫描，目录不存在时跳过
            foreach (var dir in baseDirs)
            {
                if (Directory.Exists(dir))
                {
                    RecursiveDir(dir, iconFontHashSet, IconFontDic);
                }
            }
            var iconFonts = iconFontHashSet.ToArray();

            // 将图标字体名称转换为下拉框数据源并缓存
            _iconFontItems = iconFontHashSet.Select(x => new ComboSelectListItem
            {
                Text = x,
                Value = x,
                Icon = x
            }).ToList();

            // 按字体分类生成菜单数据源，Value 为"字体名 图标名"形式的组合样式类
            _iconFontDicItems = new Dictionary<string, List<MenuItem>>();
            foreach (var key in IconFontDic.Keys)
            {
                IconFontDicItems.Add(key, IconFontDic[key].Select(x => new MenuItem
                {
                    Text = x,
                    Value = x,
                    Icon = $"{key} {x}"
                }).ToList());
            }
        }

        // 递归遍历目录及其子目录，对其中所有 CSS 文件执行图标字体解析
        private static void RecursiveDir(string dirPath, HashSet<string> iconFonts, Dictionary<string, string[]> iconFontDic)
        {
            var dirs = Directory.GetDirectories(dirPath);
            foreach (var dir in dirs)
            {
                RecursiveDir(dir, iconFonts, iconFontDic);
            }

            var files = Directory.GetFiles(dirPath, "*.css");
            foreach (var cssPath in files)
            {
                ResolveIconfont(cssPath, iconFonts, iconFontDic);
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        private static void ResolveIconfont(string cssPath, HashSet<string> iconFonts, Dictionary<string, string[]> iconFontDic)
        {
            var file = File.ReadAllText(cssPath).Replace("\r\n", string.Empty);

            // 找到自定义的 iconfont
            var regex = new Regex("@font-face\\s{0,}{\\s{0,}('|\"|)font-family(\\1)\\s{0,}:\\s{0,}('|\"|)([a-zA-Z0-9-_.#]{1,})(\\3)\\s{0,};");
            var iconMatchs = regex.Matches(file);
            foreach (Match iconfontItem in iconMatchs)
            {
                // icon family name
                var iconName = iconfontItem.Groups[4].ToString();

                var itemRegex = new Regex($".({iconName}-([a-zA-Z0-9-_.#]{{1,}}))\\s{{0,}}:before\\s{{0,}}{{");
                var itemMatchs = itemRegex.Matches(file);

                var iconFontItems = new List<string>();
                foreach (Match item in itemMatchs)
                {
                    iconFontItems.Add(item.Groups[1].ToString());
                }

                if (iconFontItems.Count > 0)
                {
                    iconFonts.Add(iconName);
                    iconFontDic.Add(iconName, iconFontItems.OrderBy(x => x).ToArray());
                }
            }
        }
    }
}
