using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;


namespace WalkingTec.Mvvm.Demo._Admin.ViewModels.FrameworkTenantVMs
{
    public partial class FrameworkTenantVM : BaseCRUDVM<FrameworkTenant>
    {

        public FrameworkTenantVM()
        {
        }

        protected override void InitVM()
        {
        }

        public override void DoAdd()
        {           
            base.DoAdd();
            if (MSD.IsValid)
            {
                if (Entity.IsUsingDB == false)
                {
                    var itcode = Entity.TCode + "_admin";
                    var role = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001" && x.TenantCode == Entity.TCode).FirstOrDefault();
                    if (role == null)
                    {
                        role = new FrameworkRole() { RoleCode = "001", RoleName = CoreProgram._localizer?["Sys.Admin"], TenantCode = Entity.TCode };
                        DC.AddEntity(role);
                    }
                    var admin = DC.Set<FrameworkUser>().Where(x => x.ITCode == itcode && x.TenantCode == Entity.TCode).FirstOrDefault();
                    if(admin == null)
                    {
                        admin = new FrameworkUser() { ITCode = "admin", Name = Entity.TCode+"_admin", Password = Utils.GetMD5String("000000"), TenantCode = Entity.TCode };
                        DC.AddEntity(admin);
                    }
                    var middle = DC.Set<FrameworkUserRole>().Where(x => x.UserCode == "admin" && x.RoleCode == "001" && x.TenantCode == Entity.TCode).FirstOrDefault();
                    if(middle == null)
                    {
                        middle = new FrameworkUserRole{UserCode = "admin",RoleCode = "001", TenantCode = Entity.TCode };
                        DC.AddEntity(middle);
                    }
                    DC.SaveChanges();
                }
            }
            Cache.Delete("tenants");
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
            if (MSD.IsValid)
            {
                if (Entity.IsUsingDB == false)
                {
                    var itcode = Entity.TCode + "_admin";
                    var role = DC.Set<FrameworkRole>().Where(x => x.RoleCode == "001" && x.TenantCode == Entity.TCode).FirstOrDefault();
                    if (role == null)
                    {
                        role = new FrameworkRole() { RoleCode = "001", RoleName = CoreProgram._localizer?["Sys.Admin"], TenantCode = Entity.TCode };
                        DC.AddEntity(role);
                    }
                    var admin = DC.Set<FrameworkUser>().Where(x => x.ITCode == itcode && x.TenantCode == Entity.TCode).FirstOrDefault();
                    if (admin == null)
                    {
                        admin = new FrameworkUser() { ITCode = "admin", Name = Entity.TCode + "_admin", TenantCode = Entity.TCode };
                        DC.AddEntity(admin);
                    }
                    var middle = DC.Set<FrameworkUserRole>().Where(x => x.UserCode == "admin" && x.RoleCode == "001" && x.TenantCode == Entity.TCode).FirstOrDefault();
                    if (middle == null)
                    {
                        middle = new FrameworkUserRole { UserCode = "admin", RoleCode = "001", TenantCode = Entity.TCode };
                        DC.AddEntity(middle);
                    }
                    DC.SaveChanges();
                }
            }
            Cache.Delete("tenants");
        }

        public override void DoDelete()
        {
            base.DoDelete();
            Cache.Delete("tenants");
        }
    }
}
