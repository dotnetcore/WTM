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
            src: "/_frameworkuserbase/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/_frameworkuserbase/{ID}",
            method: "get"
        },
        insert: {
            src: "/_frameworkuserbase/add",
            method: "post"
        },
        update: {
            src: "/_frameworkuserbase/edit",
            method: "put"
        },
        delete: {
            src: "/_frameworkuserbase/BatchDelete",
            method: "post"
        },
        import: {
            src: "/_frameworkuserbase/import",
            method: "post"
        },
        export: {
            src: "/_frameworkuserbase/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/_frameworkuserbase/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/_frameworkuserbase/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
