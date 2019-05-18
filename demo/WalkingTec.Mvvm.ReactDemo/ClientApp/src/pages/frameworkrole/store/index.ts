import { BindAll } from 'lodash-decorators';
import DataSource from 'store/dataSource';
import lodash from 'lodash';
@BindAll()
export class Store extends DataSource {
    constructor() {
        super({
            // IdKey: "ID", 默认 ID
            // Target: "/api", 默认 /api
            Apis: {
                search: {
                    url: "/_frameworkrole/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkrole/{ID}",
                    method: "get"
                },
                pages: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkrole/GetPageActions/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_frameworkrole/add",
                    method: "post"
                },
                update: {
                    url: "/_frameworkrole/edit",
                    method: "put"
                },
                updatepage: {
                    url: "/_frameworkrole/EditPrivilege",
                    method: "put"
                },
                delete: {
                    url: "/_frameworkrole/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_frameworkrole/import",
                    method: "post"
                },
                export: {
                    url: "/_frameworkrole/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_frameworkrole/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_frameworkrole/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }

    /** 读取权限 */
    async onPages(params) {
        if (lodash.isString(params)) {
            params = lodash.set({}, this.options.IdKey, params);
        }
        const res = await this.Observable.Request.ajax({ ...this.options.Apis.pages, body: params }).toPromise();
        this.DataSource.details = res;
        return res;
    }

    /** 设置权限 */
    async onUpdatePages(params) {
        if (lodash.isString(params)) {
            params = lodash.set({}, this.options.IdKey, params);
        }
        const res = await this.Observable.Request.ajax({ ...this.options.Apis.updatepage, body: params }).toPromise();
        this.DataSource.details = res;
        return res;
    }
}
export default new Store();