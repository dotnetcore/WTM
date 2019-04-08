import { BindAll } from 'lodash-decorators';
import DataSource from 'store/dataSource';
@BindAll()
export class Store extends DataSource {
    constructor() {
        super({
            // IdKey: "ID", 默认 ID
            // Target: "/api", 默认 /api
            Apis: {
                search: {
                    url: "/_dataprivilege/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_dataprivilege/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_dataprivilege/add",
                    method: "post"
                },
                update: {
                    url: "/_dataprivilege/edit",
                    method: "put"
                },
                delete: {
                    url: "/_dataprivilege/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_dataprivilege/import",
                    method: "post"
                },
                export: {
                    url: "/_dataprivilege/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_dataprivilege/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_dataprivilege/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();