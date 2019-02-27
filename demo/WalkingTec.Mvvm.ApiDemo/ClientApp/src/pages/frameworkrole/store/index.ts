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
            src: "/_frameworkrole/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/_frameworkrole/{ID}",
            method: "get"
        },
        insert: {
            src: "/_frameworkrole/add",
            method: "post"
        },
        update: {
            src: "/_frameworkrole/edit",
            method: "put"
        },
        delete: {
            src: "/_frameworkrole/BatchDelete",
            method: "post"
        },
        import: {
            src: "/_frameworkrole/import",
            method: "post"
        },
        export: {
            src: "/_frameworkrole/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/_frameworkrole/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/_frameworkrole/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
