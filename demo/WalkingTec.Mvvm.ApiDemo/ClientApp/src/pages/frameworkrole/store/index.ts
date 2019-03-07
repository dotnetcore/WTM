import DataSource from 'store/dataSource';
export class Store extends DataSource {
    constructor() {
        super();
    }
    /** 数据 ID 索引 */
    IdKey = 'ID';
    /** 地址配置列表 */
    Urls: WTM.IUrls = {
        search: {
            url: "/_frameworkrole/search",
            method: "post"
        },
        details: {
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
}
export default new Store();
