import { BindAll } from 'lodash-decorators';
import DataSource from 'store/dataSource';
import { observable, action } from 'mobx';
@BindAll()
export class Store extends DataSource {
    constructor() {
        super({
            // IdKey: "ID", 默认 ID
            // Target: "/api", 默认 /api
            Apis: {
                search: {
                    url: "/_frameworkuserbase/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkuserbase/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_frameworkuserbase/add",
                    method: "post"
                },
                update: {
                    url: "/_frameworkuserbase/edit",
                    method: "put"
                },
                delete: {
                    url: "/_frameworkuserbase/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_frameworkuserbase/import",
                    method: "post"
                },
                export: {
                    url: "/_frameworkuserbase/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_frameworkuserbase/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_frameworkuserbase/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();