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
            url: "/_actionlog/search",
            method: "post"
        },
        details: {
            url: "/_actionlog/{ID}",
            method: "get"
        },
        insert: {
            url: "/_actionlog/add",
            method: "post"
        },
        update: {
            url: "/_actionlog/edit",
            method: "put"
        },
        delete: {
            url: "/_actionlog/BatchDelete",
            method: "post"
        },
        import: {
            url: "/_actionlog/import",
            method: "post"
        },
        export: {
            url: "/_actionlog/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/_actionlog/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/_actionlog/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
