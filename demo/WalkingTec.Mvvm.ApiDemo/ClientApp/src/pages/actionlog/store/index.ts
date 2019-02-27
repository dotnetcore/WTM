 import DataSource from 'store/dataSource';
export class Store extends DataSource {
    constructor() {
        super();
    }
    // 动作权限  可在路由进入的时候注入
    Actions = {
        insert: true,
        update: true,
        delete: true,
        import: true,
        export: true,
    }
    /** 数据 ID 索引 */
    IdKey = 'ID';
    Urls = {
        ...this.Urls,
        search: {
            src: "/_actionlog/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/_actionlog/{ID}",
            method: "get"
        },
        insert: {
            src: "/_actionlog/add",
            method: "post"
        },
        update: {
            src: "/_actionlog/edit",
            method: "put"
        },
        delete: {
            src: "/_actionlog/BatchDelete",
            method: "post"
        },
        import: {
            src: "/_actionlog/import",
            method: "post"
        },
        export: {
            src: "/_actionlog/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/_actionlog/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/_actionlog/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();