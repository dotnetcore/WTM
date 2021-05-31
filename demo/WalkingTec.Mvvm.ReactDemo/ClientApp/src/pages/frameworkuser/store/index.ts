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
                    url: "/_frameworkuser/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkuser/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_frameworkuser/add",
                    method: "post"
                },
                update: {
                    url: "/_frameworkuser/edit",
                    method: "put"
                },
                delete: {
                    url: "/_frameworkuser/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_frameworkuser/import",
                    method: "post"
                },
                export: {
                    url: "/_frameworkuser/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_frameworkuser/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_frameworkuser/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();
