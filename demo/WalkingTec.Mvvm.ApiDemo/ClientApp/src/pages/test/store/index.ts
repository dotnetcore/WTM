import DataSource from 'store/dataSource';
export class Store extends DataSource {
    constructor() {
        super();
    }
    /** 数据 ID 索引 */
    IdKey = 'ID';
    Urls = {
        search: {
            src: "/user/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/user/{ID}",
            method: "get"
        },
        insert: {
            src: "/user/add",
            method: "post"
        },
        update: {
            src: "/user/edit",
            method: "put"
        },
        delete: {
            src: "/user/delete",
            method: "get"
        },
        import: {
            src: "/user/import",
            method: "post"
        },
        export: {
            src: "/user/ExportExcel",
            method: "post"
        },
        template: {
            src: "/user/template",
            method: "post"
        }
    }
}
export default new Store();