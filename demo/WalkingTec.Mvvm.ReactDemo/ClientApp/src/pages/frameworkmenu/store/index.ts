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
                    url: "/_frameworkmenu/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkmenu/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/_frameworkmenu/add",
                    method: "post"
                },
                update: {
                    url: "/_frameworkmenu/edit",
                    method: "put"
                },
                delete: {
                    url: "/_frameworkmenu/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/_frameworkmenu/import",
                    method: "post"
                },
                export: {
                    url: "/_frameworkmenu/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/_frameworkmenu/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/_frameworkmenu/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();