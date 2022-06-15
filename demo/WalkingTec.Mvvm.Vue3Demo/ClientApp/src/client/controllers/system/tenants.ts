/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-04-02 11:49:08
 * @modify date 2021-04-02 11:49:08
 * @desc 租客
 */
import { AjaxBasics, globalProperties } from "@/client";
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
@BindAll()
export class TenantsController {
    $ajax: AjaxBasics;
    options = {
        TenantsList: {
            url: "/api/_frameworktenant/GetFrameworkTenants",
            method: "get"
        },
        SetTenant: {
            url: "/api/_account/SetTenant",
            method: "get"
        }
    }
    async onInit() {
        this.$ajax = globalProperties.$Ajax;
    }

    TenantsList(parent){
        return this.$ajax.request(lodash.assign({body:{ parent:parent }}, this.options.TenantsList)).toPromise()
    }

    async SetTenant(tenantcode){
        const res: any = await this.$ajax.request(lodash.assign({body:{ tenant:tenantcode }}, this.options.SetTenant)).toPromise()
    }
}

