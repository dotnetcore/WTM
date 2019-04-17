using NPOI.HSSF.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WalkingTec.Mvvm.Core
{
    public class Utils
    {

        public static string GetCurrentComma()
        {
            if (CultureInfo.CurrentUICulture.Name == "zh-cn")
            {
                return "：";
            }
            else
            {
                return ":";
            }
        }

        public static List<Assembly> GetAllAssembly()
        {
            var rv = new List<Assembly>();
            var path = Assembly.GetEntryAssembly().Location;
            var dir = new DirectoryInfo(Path.GetDirectoryName(path));

            var dlls = dir.GetFiles("*.dll", SearchOption.AllDirectories);
            
            foreach (var dll in dlls)
            {
                try
                {
                    if (dll.Name.StartsWith("Microsoft.") == false)
                    {
                        rv.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName));
                    }
                }
                catch { }
            }
            return rv;
        }

        public static FrameworkMenu FindMenu(string url)
        {
            var menus = GlobalServices.GetRequiredService<GlobalData>()?.AllMenus;
            if(menus == null)
            {
                return null;
            }
            //寻找菜单中是否有与当前判断的url完全相同的
            var menu = menus.Where(x => x.Url != null && x.Url.ToLower() == url.ToLower()).FirstOrDefault();

            //如果没有，抹掉当前url的参数，用不带参数的url比对
            if (menu == null)
            {
                var pos = url.IndexOf("?");
                if (pos > 0)
                {
                    url = url.Substring(0, pos);
                    menu = menus.Where(x => x.Url != null && x.Url.ToLower() == url.ToLower()).FirstOrDefault();
                }
            }

            //如果还没找到，则判断url是否为/controller/action/id这种格式，如果是则抹掉/id之后再对比
            if (menu == null)
            {
                var split = url.Split('/');
                if (split.Length >= 2 && Guid.TryParse(split.Last(), out Guid longTest))
                {
                    var pos = url.LastIndexOf("/");
                    url = url.Substring(0, pos);
                    menu = menus.Where(x => x.Url != null && x.Url.ToLower() == url.ToLower()).FirstOrDefault();
                }
            }
            return menu;
        }


        public static string GetIdByName(string fieldName)
        {
            return fieldName == null ? "" : fieldName.Replace(".", "_").Replace("[", "_").Replace("]", "_");
        }

        public static void CheckDifference<T>(IEnumerable<T> oldList, IEnumerable<T> newList, out IEnumerable<T> ToRemove, out IEnumerable<T> ToAdd) where T : TopBasePoco
        {
            List<T> tempToRemove = new List<T>();
            List<T> tempToAdd = new List<T>();
            oldList = oldList ?? new List<T>();
            newList = newList ?? new List<T>();
            foreach (var oldItem in oldList)
            {
                bool exist = false;
                foreach (var newItem in newList)
                {
                    if (oldItem.ID == newItem.ID)
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    tempToRemove.Add(oldItem);
                }
            }
            foreach (var newItem in newList)
            {
                bool exist = false;
                foreach (var oldItem in oldList)
                {
                    if (newItem.ID == oldItem.ID)
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist == false)
                {
                    tempToAdd.Add(newItem);
                }
            }
            ToRemove = tempToRemove.AsEnumerable();
            ToAdd = tempToAdd.AsEnumerable();
        }

        public static short GetExcelColor(string color)
        {
            var colors = typeof(HSSFColor).GetNestedTypes().ToList();
            foreach (var col in colors)
            {
                var pro = col.GetField("hexString");
                if (pro == null)
                {
                    continue;
                }
                var hex = pro.GetValue(null);
                var rgb = hex.ToString().Split(':');
                for (int i = 0; i < rgb.Length; i++)
                {
                    if (rgb[i].Length > 2)
                    {
                        rgb[i] = rgb[i].Substring(0, 2);
                    }
                }
                int r = Convert.ToInt16(rgb[0], 16);
                int g = Convert.ToInt16(rgb[1], 16);
                int b = Convert.ToInt16(rgb[2], 16);

                if (color.Length == 8)
                {
                    color = color.Substring(2);
                }
                string c1 = color.Substring(0, 2);
                string c2 = color.Substring(2, 2);
                string c3 = color.Substring(4, 2);

                int r1 = Convert.ToInt16(c1, 16);
                int g1 = Convert.ToInt16(c2, 16);
                int b1 = Convert.ToInt16(c3, 16);


                if (r == r1 && g == g1 && b == b1)
                {
                    return (short)col.GetField("index").GetValue(null);
                }
            }
            return HSSFColor.COLOR_NORMAL;
        }

        /// <summary>
        /// 获取Bool类型的下拉框
        /// </summary>
        /// <param name="boolType"></param>
        /// <param name="defaultValue"></param>
        /// <param name="trueText"></param>
        /// <param name="falseText"></param>
        /// <param name="selectText"></param>
        /// <returns></returns>
        public static List<ComboSelectListItem> GetBoolCombo(BoolComboTypes boolType, bool? defaultValue = null, string trueText = null, string falseText = null, string selectText = null)
        {
            List<ComboSelectListItem> rv = new List<ComboSelectListItem>();
            string yesText = "";
            string noText = "";
            switch (boolType)
            {
                case BoolComboTypes.YesNo:
                    yesText = "是";
                    noText = "否";
                    break;
                case BoolComboTypes.ValidInvalid:
                    yesText = "有效";
                    noText = "无效";
                    break;
                case BoolComboTypes.MaleFemale:
                    yesText = "男";
                    noText = "女";
                    break;
                case BoolComboTypes.HaveNotHave:
                    yesText = "有";
                    noText = "无";
                    break;
                case BoolComboTypes.Custom:
                    yesText = trueText ?? "是";
                    noText = falseText ?? "否";
                    break;
                default:
                    break;
            }
            ComboSelectListItem yesItem = new ComboSelectListItem()
            {
                Text = yesText,
                Value = "true"
            };
            if (defaultValue == true)
            {
                yesItem.Selected = true;
            }
            ComboSelectListItem noItem = new ComboSelectListItem()
            {
                Text = noText,
                Value = "false"
            };
            if (defaultValue == false)
            {
                noItem.Selected = true;
            }
            rv.Add(yesItem);
            rv.Add(noItem);
            return rv;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ZipAndBase64Encode(string input)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(input);
            MemoryStream inputms = new MemoryStream(buffer);
            MemoryStream outputms = new MemoryStream();
            using (GZipStream zip = new GZipStream(outputms, CompressionMode.Compress))
            {
                inputms.CopyTo(zip);
            }
            byte[] rv = outputms.ToArray();
            inputms.Dispose();
            return Convert.ToBase64String(rv);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UnZipAndBase64Decode(string input)
        {
            byte[] inputstr = Convert.FromBase64String(input);
            MemoryStream inputms = new MemoryStream(inputstr);
            MemoryStream outputms = new MemoryStream();
            using (GZipStream zip = new GZipStream(inputms, CompressionMode.Decompress))
            {
                zip.CopyTo(outputms);
            }
            byte[] rv = outputms.ToArray();
            outputms.Dispose();
            return Encoding.UTF8.GetString(rv);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncodeScriptJson(string input)
        {
            if (input == null)
            {
                return "";
            }
            else
            {
                return input.Replace(Environment.NewLine, "").Replace("\"", "\\\\\\\"").Replace("'", "\\'");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            try
            {
                System.IO.File.Delete(path);
            }
            catch { }
        }

        #region 格式化文本  add by wuwh 2014.6.12
        /// <summary>
        /// 格式化文本
        /// </summary>
        /// <param name="text">要格式化的字符串</param>
        /// <param name="isCode">是否是纯代码</param>
        /// <returns></returns>
        public static string FormatText(string text, bool isCode = false)
        {

            if (isCode)
            {
                return FormatCode(text);
            }
            else
            {
                #region 截取需要格式化的代码段
                List<int> listInt = new List<int>();
                int index = 0;
                int _index;
                while (true)
                {
                    _index = text.IndexOf("&&", index);
                    index = _index + 1;
                    if (_index >= 0 && _index <= text.Length)
                    {
                        listInt.Add(_index);
                    }
                    else
                    {
                        break;
                    }
                }

                List<string> listStr = new List<string>();
                for (int i = 0; i < listInt.Count; i++)
                {
                    string temp = text.Substring(listInt[i] + 2, listInt[i + 1] - listInt[i] - 2);

                    listStr.Add(temp);
                    i++;
                }
                #endregion

                #region 格式化代码段
                //先将 <  >以及空格替换掉，防止下面替换出现 html标签后出现问题
                for (int i = 0; i < listStr.Count; i++)
                {
                    //将 &&代码&&  替换成&&1&&
                    text = text.Replace("&&" + listStr[i] + "&&", FormatCode(listStr[i]));
                }
                #endregion

                return text;
            }
        }
        #endregion

        #region 格式化代码  edit by wuwh
        /// <summary>
        /// 格式化代码 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string FormatCode(string code)
        {
            //先将 <  >以及空格替换掉，防止下面替换出现 html标签后出现问题
            code = code.Replace("<", "&lt;").Replace(">", "&gt;").Replace(" ", "&nbsp;");
            string csKeyWords = "abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|from|get|goto|group|if|implicit|in|int|interface|internal|into|is|join|let|lock|long|namespace|new|null|object|operator|orderby|out|override|params|partial|private|protected|public|readonly|ref|return|sbyte|sealed|select|set|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|value|var|virtual|void|volatile|where|while|yield";

            string r1 = "(#if DBG[\\s\\S]+?#endif)";
            string r2 = "(#[a-z ]*)";
            string r3 = "(///\\ *<[/\\w]+>)";
            string r4 = "(/\\*[\\s\\S]*?\\*/)";//匹配三杠注释
            string r5 = "(//.*)";//匹配双杠注释
            string r6 = @"(@?"".*?"")";//匹配字符串
            string r7 = "('.*?')";//匹配字符串
            string r8 = "\\b(" + csKeyWords + ")\\b";//匹配关键字
            //string r9 = "class&nbsp;(.+)&nbsp;";//匹配类
            //string r10 = "&lt;(.+)&gt;";//匹配泛型类

            string rs = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", r1, r2, r3, r4, r5, r6, r7, r8);
            //string rs = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", r1, r2, r3, r4, r5, r6, r7, r8, r9,r10);

            //<font color=#44C796>$9$10</font>
            string rr = "<font color=#808080>$1$2$3</font><font color=#008000>$4$5</font><font color=#A31515>$6$7</font><font color=#0000FF>$8</font>";

            Regex re = new Regex(rs, RegexOptions.None);
            code = Regex.Replace(code, rs, rr);
            //替换换行符"\r\n"   以及"\r"  "\n"  
            code = code.Replace("\r\n", "<br>").Replace("\n", "").Replace("\r", "<br>");
            //取消空标签
            //|<font color=#44C796></font>C#类的颜色
            code = Regex.Replace(code, "<font color=#808080></font>|<font color=#008000></font>|<font color=#A31515></font>|<font color=#0000FF></font>", "");

            return code;
        }
        #endregion

        #region 读取txt文件
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">文件路径绝对</param>
        /// <returns></returns>
        public static string ReadTxt(string path)
        {
            string result = string.Empty;

            if (File.Exists(path))
            {
                using (Stream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (TextReader sr = new StreamReader(fs, UnicodeEncoding.UTF8))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 得到目录下所有文件
        /// </summary>
        /// <param name="dirpath"></param>
        /// <returns></returns>
        public static List<string> GetAllFileName(string dirpath)
        {
            DirectoryInfo dir = new DirectoryInfo(dirpath);
            var files = dir.GetFileSystemInfos();
            return files.Select(x => x.Name).ToList();
        }

        #region add by wuwh 2014.10.18  递归获取目录下所有文件
        /// <summary>
        /// 递归获取目录下所有文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="allFiles"></param>
        /// <returns></returns>
        public static List<string> GetAllFilePathRecursion(string dirPath, List<string> allFiles)
        {
            if (allFiles == null)
            {
                allFiles = new List<string>();
            }
            string[] subPaths = Directory.GetDirectories(dirPath);
            foreach (var item in subPaths)
            {
                GetAllFilePathRecursion(item, allFiles);
            }
            allFiles.AddRange(Directory.GetFiles(dirPath).ToList());

            return allFiles;
        }
        #endregion


        /// <summary>
        /// ConvertToColumnXType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ConvertToColumnXType(Type type)
        {
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return "checkcolumn";
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return "datecolumn";
            }
            else if (type == typeof(decimal) || type == typeof(decimal?) || type == typeof(double) || type == typeof(double?) || type == typeof(int) || type == typeof(int?) || type == typeof(long) || type == typeof(long?))
            {
                return "numbercolumn";
            }
            return "textcolumn";
        }


        public static string GetCS(string cs, string mode, Configs config)
        {
            if (string.IsNullOrEmpty(cs) || config.ConnectionStrings.Any(x=>x.Key.ToLower() == cs.ToLower()) == false)
            {
                cs = "default";
            }
            int index = cs.LastIndexOf("_");
            if (index > 0)
            {
                cs = cs.Substring(0, index);
            }
            if (mode?.ToLower() == "read")
            {
                var reads = config.ConnectionStrings.Where(x => x.Key.StartsWith(cs + "_")).Select(x=>x.Key).ToList();
                if (reads.Count > 0)
                {
                    Random r = new Random();
                    var v = r.Next(0, reads.Count);
                    cs = reads[v];
                }
            }
            return cs;
        }

        public static string GetUrlByFileAttachmentId(IDataContext dc, Guid? fileAttachmentId, bool isIntranetUrl = false, string urlHeader = null)
        {
            string url = string.Empty;
            var fileAttachment = dc.Set<FileAttachment>().Where(x => x.ID == fileAttachmentId.Value).FirstOrDefault();
            if (fileAttachment != null)
            {
                    url = "/_Framework/GetFile/" + fileAttachmentId.ToString();
                
            }
            return url;
        }

        #region 加解密
        /// <summary>
        /// 通过密钥将内容加密
        /// </summary>
        /// <param name="stringToEncrypt">要加密的字符串</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <returns></returns>
        public static string EncryptString(string stringToEncrypt, string encryptKey)
        {
            if (string.IsNullOrEmpty(stringToEncrypt))
            {
                return "";
            }

            string stringEncrypted = string.Empty;
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(stringToEncrypt);
            MemoryStream encryptStream = new System.IO.MemoryStream();
            CryptoStream encStream = new CryptoStream(encryptStream, GenerateDESCryptoServiceProvider(encryptKey).CreateEncryptor(), CryptoStreamMode.Write);

            try
            {
                encStream.Write(bytIn, 0, bytIn.Length);
                encStream.FlushFinalBlock();
                stringEncrypted = Convert.ToBase64String(encryptStream.ToArray(), 0, (int)encryptStream.Length);
            }
            catch
            {
                return "";
            }
            finally
            {
                encryptStream.Close();
                encStream.Close();
            }

            return stringEncrypted;
        }

        /// <summary>
        /// 通过密钥讲内容解密
        /// </summary>
        /// <param name="stringToDecrypt">要解密的字符串</param>
        /// <param name="encryptKey">密钥</param>
        /// <returns></returns>
        public static string DecryptString(string stringToDecrypt, string encryptKey)
        {
            if (String.IsNullOrEmpty(stringToDecrypt))
            {
                return "";
            }

            string stringDecrypted = string.Empty;
            byte[] bytIn = Convert.FromBase64String(stringToDecrypt.Replace(" ", "+"));
            MemoryStream decryptStream = new MemoryStream();
            CryptoStream encStream = new CryptoStream(decryptStream, GenerateDESCryptoServiceProvider(encryptKey).CreateDecryptor(), CryptoStreamMode.Write);

            try
            {
                encStream.Write(bytIn, 0, bytIn.Length);
                encStream.FlushFinalBlock();
                stringDecrypted = Encoding.Default.GetString(decryptStream.ToArray());
            }
            catch
            {
                return "";
            }
            finally
            {
                decryptStream.Close();
                encStream.Close();
            }

            return stringDecrypted;
        }

        private static DESCryptoServiceProvider GenerateDESCryptoServiceProvider(string key)
        {
            DESCryptoServiceProvider dCrypter = new DESCryptoServiceProvider();

            string sTemp;
            if (dCrypter.LegalKeySizes.Length > 0)
            {
                int moreSize = dCrypter.LegalKeySizes[0].MinSize;
                while (key.Length > 8)
                {
                    key = key.Substring(0, 8);
                }
                sTemp = key.PadRight(moreSize / 8, ' ');
            }
            else
            {
                sTemp = key;
            }
            byte[] bytKey = UTF8Encoding.UTF8.GetBytes(sTemp);

            dCrypter.Key = bytKey;
            dCrypter.IV = bytKey;

            return dCrypter;
        }

        #endregion

        #region MD5加密
        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回大写32位MD5值</returns>
        public static string GetMD5String(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);

            return MD5String(buffer);
        }

        /// <summary>
        /// 流MD5加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMD5Stream(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return MD5String(buffer);
        }

        /// <summary>
        /// 文件MD5加密
        /// </summary>
        /// <param name="path"></param>
        /// <returns>返回大写32位MD5值</returns>
        public static string GetMD5File(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    return GetMD5Stream(fs);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private static string MD5String(byte[] buffer)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] cryptBuffer = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            foreach (byte item in cryptBuffer)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion

        public static string GetNugetVersion()
        {
            string v = APIHelper.CallAPI("https://api.nuget.org/v3-flatcontainer/walkingtec.mvvm.mvc/index.json").Result;
            var i = v.LastIndexOf("\"");
            v = v.Substring(0, i);
            i = v.LastIndexOf("\"");
            v = v.Substring(i+1);
            return v;
        }
    }
}
