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
            src: "/frameworkrole/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/frameworkrole/{ID}",
            method: "get"
        },
        insert: {
            src: "/frameworkrole/add",
            method: "post"
        },
        update: {
            src: "/frameworkrole/edit",
            method: "put"
        },
        delete: {
            src: "/frameworkrole/BatchDelete",
            method: "post"
        },
        import: {
            src: "/frameworkrole/import",
            method: "post"
        },
        export: {
            src: "/frameworkrole/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/frameworkrole/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/frameworkrole/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
