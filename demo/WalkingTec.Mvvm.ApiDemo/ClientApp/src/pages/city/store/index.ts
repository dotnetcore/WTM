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
            url: "/city/search",
            method: "post"
        },
        details: {
            url: "/city/{ID}",
            method: "get"
        },
        insert: {
            url: "/city/add",
            method: "post"
        },
        update: {
            url: "/city/edit",
            method: "put"
        },
        delete: {
            url: "/city/BatchDelete",
            method: "post"
        },
        import: {
            url: "/city/import",
            method: "post"
        },
        export: {
            url: "/city/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/city/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/city/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
