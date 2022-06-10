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
                    url: "/frameworktenant/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/frameworktenant/{ID}",
                    method: "get"
                },
                insert: {
                    url: "/frameworktenant/add",
                    method: "post"
                },
                update: {
                    url: "/frameworktenant/edit",
                    method: "put"
                },
                delete: {
                    url: "/frameworktenant/BatchDelete",
                    method: "post"
                },
                import: {
                    url: "/frameworktenant/import",
                    method: "post"
                },
                export: {
                    url: "/frameworktenant/ExportExcel",
                    method: "post"
                },
                exportIds: {
                    url: "/frameworktenant/ExportExcelByIds",
                    method: "post"
                },
                template: {
                    url: "/frameworktenant/GetExcelTemplate",
                    method: "get"
                }
            }
        });
    }
}
export default new Store();