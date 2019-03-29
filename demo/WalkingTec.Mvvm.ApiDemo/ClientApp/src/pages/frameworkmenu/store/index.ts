import DataSource from 'store/dataSource';
import RequestFiles from 'utils/RequestFiles';
export class Store extends DataSource {
    constructor() {
        super();
    }

    /** 数据 ID 索引 */
    IdKey = 'ID';
    /** 地址配置列表 */
    Urls: WTM.IUrls = {
        search: {
            url: "/_FrameworkMenu/search",
            method: "post"
        },
        details: {
            url: "/_FrameworkMenu/{ID}",
            method: "get"
        },
        insert: {
            url: "/_FrameworkMenu/add",
            method: "post"
        },
        update: {
            url: "/_FrameworkMenu/edit",
            method: "put"
        },
        delete: {
            url: "/_FrameworkMenu/BatchDelete",
            method: "post"
        },
        import: {
            url: "/_FrameworkMenu/import",
            method: "post"
        },
        export: {
            url: "/_FrameworkMenu/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/_FrameworkMenu/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/_FrameworkMenu/GetExcelTemplate",
            method: "get"
        },
        syncmodel: {
            url: "/_FrameworkMenu/SyncModel",
            method:"get"
        },
        unset: {
            url: "/_FrameworkMenu/UnsetPages",
            method: "get"
        },
        refresh: {
            url: "/_FrameworkMenu/RefreshMenu",
            method: "get"
        }
    }
}
export default new Store();
