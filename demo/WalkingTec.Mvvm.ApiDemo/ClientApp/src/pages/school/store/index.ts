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
            url: "/school/search",
            method: "post"
        },
        details: {
            url: "/school/{ID}",
            method: "get"
        },
        insert: {
            url: "/school/add",
            method: "post"
        },
        update: {
            url: "/school/edit",
            method: "put"
        },
        delete: {
            url: "/school/BatchDelete",
            method: "post"
        },
        import: {
            url: "/school/import",
            method: "post"
        },
        export: {
            url: "/school/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/school/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/school/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
