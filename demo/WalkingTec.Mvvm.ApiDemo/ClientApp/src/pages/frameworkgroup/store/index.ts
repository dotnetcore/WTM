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
            src: "/frameworkgroup/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/frameworkgroup/{ID}",
            method: "get"
        },
        insert: {
            src: "/frameworkgroup/add",
            method: "post"
        },
        update: {
            src: "/frameworkgroup/edit",
            method: "put"
        },
        delete: {
            src: "/frameworkgroup/BatchDelete",
            method: "post"
        },
        import: {
            src: "/frameworkgroup/import",
            method: "post"
        },
        export: {
            src: "/frameworkgroup/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/frameworkgroup/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/frameworkgroup/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
