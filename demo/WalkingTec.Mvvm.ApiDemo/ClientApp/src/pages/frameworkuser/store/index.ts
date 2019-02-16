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
            src: "/frameworkuser/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/frameworkuser/{ID}",
            method: "get"
        },
        insert: {
            src: "/frameworkuser/add",
            method: "post"
        },
        update: {
            src: "/frameworkuser/edit",
            method: "put"
        },
        delete: {
            src: "/frameworkuser/delete",
            method: "get"
        },
        import: {
            src: "/frameworkuser/import",
            method: "post"
        },
        export: {
            src: "/frameworkuser/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/frameworkuser/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/frameworkuser/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();