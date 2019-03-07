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
            url: "/_frameworkuserbase/search",
            method: "post"
        },
        details: {
            url: "/_frameworkuserbase/{ID}",
            method: "get"
        },
        insert: {
            url: "/_frameworkuserbase/add",
            method: "post"
        },
        update: {
            url: "/_frameworkuserbase/edit",
            method: "put"
        },
        delete: {
            url: "/_frameworkuserbase/BatchDelete",
            method: "post"
        },
        import: {
            url: "/_frameworkuserbase/import",
            method: "post"
        },
        export: {
            url: "/_frameworkuserbase/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/_frameworkuserbase/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/_frameworkuserbase/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
