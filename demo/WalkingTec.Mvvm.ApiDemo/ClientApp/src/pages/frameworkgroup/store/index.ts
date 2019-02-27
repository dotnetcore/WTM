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
            src: "/_frameworkgroup/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/_frameworkgroup/{ID}",
            method: "get"
        },
        insert: {
            src: "/_frameworkgroup/add",
            method: "post"
        },
        update: {
            src: "/_frameworkgroup/edit",
            method: "put"
        },
        delete: {
            src: "/_frameworkgroup/BatchDelete",
            method: "post"
        },
        import: {
            src: "/_frameworkgroup/import",
            method: "post"
        },
        export: {
            src: "/_frameworkgroup/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/_frameworkgroup/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/_frameworkgroup/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
