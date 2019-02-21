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
            src: "/frameworkuserbase/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/frameworkuserbase/{ID}",
            method: "get"
        },
        insert: {
            src: "/frameworkuserbase/add",
            method: "post"
        },
        update: {
            src: "/frameworkuserbase/edit",
            method: "put"
        },
        delete: {
            src: "/frameworkuserbase/BatchDelete",
            method: "post"
        },
        import: {
            src: "/frameworkuserbase/import",
            method: "post"
        },
        export: {
            src: "/frameworkuserbase/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/frameworkuserbase/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/frameworkuserbase/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
