using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkTenantVMs
{
    public partial class FrameworkTenantVM : BaseCRUDVM<FrameworkTenant>
    {

        [Display(Name = "_Admin.Role")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string AdminRoleCode { get; set; }

        public FrameworkTenantVM()
        {
        }
        public override DuplicatedInfo<FrameworkTenant> SetDuplicatedCheck()
        {
            var rv = CreateFieldsInfo(SimpleField(x => x.TCode));
            rv.Groups.ForEach(g => g.UseTenant = false);
            return rv;
        }

        protected override void InitVM()
        {
        }

        public override void DoAdd()
        {
            base.DoAdd();
            if (MSD.IsValid)
            {
                TenantOperation();
                Cache.Delete(nameof(GlobalData.AllTenant));
            }
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
            if (MSD.IsValid)
            {
                if (MSD.IsValid)
                {
                    TenantOperation();
                    Cache.Delete(nameof(GlobalData.AllTenant));
                }
            }
        }

        private void TenantOperation()
        {
            List<FunctionPrivilege> fps = new List<FunctionPrivilege>();
            using (var userdc = LoginUserInfo.GetUserDC(Wtm))
            {
                fps = userdc.Set<FunctionPrivilege>().AsNoTracking().Where(x => x.RoleCode == AdminRoleCode).ToList();
                List<Guid> tenantmenus = new List<Guid>();
                using (var defaultdc = Wtm.CreateDC(false, "default", false))
                {
                    tenantmenus = defaultdc.Set<FrameworkMenu>().Where(x => x.TenantAllowed == false || (x.Url.ToLower().Contains("frameworktenant") && Entity.EnableSub == false) || x.Url.ToLower().Contains("frameworkmenu")).Select(x => x.ID).ToList();
                }
                fps = fps.Where(x => tenantmenus.Contains(x.MenuItemId) == false).ToList();

                if (Entity.IsUsingDB == false)
                {
                    AddTenantData(userdc, fps);
                }
            }
            if (Entity.IsUsingDB == true)
            {
                try
                {
                    var tenantdc = Entity.CreateDC(Wtm);
                    if (tenantdc.Database.EnsureCreated() == true)
                    {
                        tenantdc.SetTenantCode(Entity.TCode);
                        tenantdc.DataInit(Wtm.GlobaInfo.AllModule, Wtm.GlobaInfo.IsSpa).Wait();
                    }
                    AddTenantData(tenantdc, fps);
                }
                catch
                {
                    MSD.AddModelError(" TenantDbError", Localizer["_Admin.TenantDbError"]);
                }
            }

        }

        private void AddTenantData(IDataContext dc, List<FunctionPrivilege> fps)
        {
            var itcode = "admin";
            var role = dc.Set<FrameworkRole>().AsNoTracking().IgnoreQueryFilters().Where(x => x.RoleCode == "001" && x.TenantCode == Entity.TCode).FirstOrDefault();
            if (role == null)
            {
                role = new FrameworkRole() { RoleCode = "001", RoleName = CoreProgram._localizer?["Sys.Admin"], TenantCode = Entity.TCode };
                dc.AddEntity(role);
            }
            var admin = dc.Set<FrameworkUser>().AsNoTracking().IgnoreQueryFilters().Where(x => x.ITCode == itcode && x.TenantCode == Entity.TCode).FirstOrDefault();
            if (admin == null)
            {
                admin = new FrameworkUser() { ITCode = "admin", Name = Entity.TCode + "_admin", Password = Utils.GetMD5String("000000"), TenantCode = Entity.TCode, IsValid = true };
                dc.AddEntity(admin);
            }
            var middle = dc.Set<FrameworkUserRole>().AsNoTracking().IgnoreQueryFilters().Where(x => x.UserCode == "admin" && x.RoleCode == "001" && x.TenantCode == Entity.TCode).FirstOrDefault();
            if (middle == null)
            {
                middle = new FrameworkUserRole { UserCode = "admin", RoleCode = "001", TenantCode = Entity.TCode };
                dc.AddEntity(middle);
            }
            dc.SaveChanges();
            var toremove = dc.Set<FunctionPrivilege>().IgnoreQueryFilters().Where(x => x.RoleCode == "001" && x.TenantCode == Entity.TCode).ToList();
            toremove.ForEach(x => dc.DeleteEntity(x));
            foreach (var item in fps)
            {
                dc.Set<FunctionPrivilege>().Add(new FunctionPrivilege
                {
                    MenuItemId = item.MenuItemId,
                    Allowed = item.Allowed,
                    RoleCode = "001",
                    TenantCode = Entity.TCode
                });
            }
            dc.SaveChanges();
            var key = $"{GlobalConstants.CacheKey.UserInfo}:{"admin" + "$`$" + Entity.TCode}";
            Cache.DeleteAsync(key).Wait();
        }
        public override void DoDelete()
        {
            base.DoDelete();
            Cache.Delete(nameof(GlobalData.AllTenant));
        }
    }
}
