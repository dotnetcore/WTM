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
            url: "/_frameworkgroup/search",
            method: "post"
        },
        details: {
            url: "/_frameworkgroup/{ID}",
            method: "get"
        },
        insert: {
            url: "/_frameworkgroup/add",
            method: "post"
        },
        update: {
            url: "/_frameworkgroup/edit",
            method: "put"
        },
        delete: {
            url: "/_frameworkgroup/BatchDelete",
            method: "post"
        },
        import: {
            url: "/_frameworkgroup/import",
            method: "post"
        },
        export: {
            url: "/_frameworkgroup/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/_frameworkgroup/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/_frameworkgroup/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
