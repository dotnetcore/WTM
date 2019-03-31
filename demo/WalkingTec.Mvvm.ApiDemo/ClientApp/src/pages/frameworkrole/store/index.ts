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
                    url: "/_frameworkrole/search",
                    method: "post"
                },
                details: {
                    // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                    url: "/_frameworkrole/{ID}",
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
}
export default new Store();