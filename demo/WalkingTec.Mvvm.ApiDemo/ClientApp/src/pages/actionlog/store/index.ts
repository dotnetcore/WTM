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
            src: "/actionlog/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/actionlog/{ID}",
            method: "get"
        },
        insert: {
            src: "/actionlog/add",
            method: "post"
        },
        update: {
            src: "/actionlog/edit",
            method: "put"
        },
        delete: {
            src: "/actionlog/delete",
            method: "get"
        },
        import: {
            src: "/actionlog/import",
            method: "post"
        },
        export: {
            src: "/actionlog/exportExcel",
            method: "post"
        },
        exportIds: {
            src: "/actionlog/exportExcelByIds",
            method: "post"
        },
        template: {
            src: "/actionlog/getExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();