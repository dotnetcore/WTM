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
            url: "/_dataprivilege/search",
            method: "post"
        },
        details: {
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
}
export default new Store();
