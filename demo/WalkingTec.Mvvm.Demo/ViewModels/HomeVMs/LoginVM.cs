using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.HomeVMs
{
    public class LoginVM : BaseVM
    {
        [Display(Name = "账号")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50)]
        public string ITCode { get; set; }

        [Display(Name = "密码")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Password { get; set; }

        [Display(Name = "记录我的登录")]
        public bool RememberLogin { get; set; }

        public string Redirect { get; set; }

        [Display(Name = "验证码")]
        public string VerifyCode { get; set; }

        /// <summary>
        /// 进行登录
        /// </summary>
        /// <param name="ignorePris">外部传递的页面权限</param>
        /// <returns>登录用户的信息</returns>
        public LoginUserInfo DoLogin(bool ignorePris = false)
        {
            //根据用户名和密码查询用户
            var user = DC.Set<FrameworkUserBase>()
                .Include(x => x.UserRoles).Include(x => x.UserGroups)
                .Where(x => x.ITCode.ToLower() == ITCode.ToLower() && x.Password == Utils.GetMD5String(Password) && x.IsValid)
                .SingleOrDefault();

            //如果没有找到则输出错误
            if (user == null)
            {
                MSD.AddModelError("", Localizer["LoginFail"]);
                return null;
            }
            var roleIDs = user.UserRoles.Select(x => x.RoleId).ToList();
            var groupIDs = user.UserGroups.Select(x => x.GroupId).ToList();
            //查找登录用户的数据权限
            var dpris = DC.Set<DataPrivilege>().AsNoTracking()
                .Where(x => x.UserId == user.ID || (x.GroupId != null && groupIDs.Contains(x.GroupId.Value)))
                .Distinct()
                .ToList();
            ProcessTreeDp(dpris);
            //生成并返回登录用户信息
            LoginUserInfo rv = new LoginUserInfo
            {
                Id = user.ID,
                ITCode = user.ITCode,
                Name = user.Name,
                PhotoId = user.PhotoId,
                Roles = DC.Set<FrameworkRole>().Where(x => roleIDs.Contains(x.ID)).ToList(),
                Groups = DC.Set<FrameworkGroup>().Where(x => groupIDs.Contains(x.ID)).ToList(),
                DataPrivileges = dpris
            };
            if (ignorePris == false)
            {
                //查找登录用户的页面权限
                var pris = DC.Set<FunctionPrivilege>().AsNoTracking()
                    .Where(x => x.UserId == user.ID || (x.RoleId != null && roleIDs.Contains(x.RoleId.Value)))
                    .Distinct()
                    .ToList();
                rv.FunctionPrivileges = pris;
            }
            return rv;
        }


        private void ProcessTreeDp(List<DataPrivilege> dps)
        {
            var dpsSetting = GlobalServices.GetService<Configs>().DataPrivilegeSettings;
            foreach (var ds in dpsSetting)
            {
                if (typeof(ITreeData).IsAssignableFrom(ds.ModelType))
                {
                    var ids = dps.Where(x => x.TableName == ds.ModelName).Select(x => x.RelateId).ToList();
                    if (ids.Count > 0 && ids.Contains(null) == false)
                    {
                        List<Guid> tempids = new List<Guid>();
                        foreach (var item in ids)
                        {
                            if(Guid.TryParse(item,out Guid g))
                            {
                                tempids.Add(g);
                            }
                        }

                        var basequery = DC.GetType().GetTypeInfo().GetMethod("Set").MakeGenericMethod(ds.ModelType).Invoke(DC, null) as IQueryable;
                        var skipids = basequery.Cast<ITreeData>().Where(x =>tempids.Contains(x.ID) && x.ParentId != null).Select(x => x.ParentId.Value).ToList();


                        List<Guid> subids = new List<Guid>();
                        subids.AddRange(GetSubIds(tempids.ToList(), ds.ModelType, skipids));
                        subids = subids.Distinct().ToList();
                        subids.ForEach(x => dps.Add(new DataPrivilege {
                          TableName = ds.ModelName,
                          RelateId = x.ToString()
                        }));
                    }
                }
            }
        }

        private IEnumerable<Guid> GetSubIds(List<Guid> p_id,Type modelType, List<Guid> skipids)
        {
            var basequery = DC.GetType().GetTypeInfo().GetMethod("Set").MakeGenericMethod(modelType).Invoke(DC, null) as IQueryable;
            var ids = p_id.Where(x => skipids.Contains(x) == false).ToList();
            var subids = basequery.Cast<ITreeData>().Where(x => ids.Contains(x.ParentId.Value)).Select(x => x.ID).ToList();
            if (subids.Count > 0)
            {
                return subids.Concat(GetSubIds(subids, modelType, skipids));
            }
            else
            {
                return new List<Guid>();
            }
        }
    }
}
