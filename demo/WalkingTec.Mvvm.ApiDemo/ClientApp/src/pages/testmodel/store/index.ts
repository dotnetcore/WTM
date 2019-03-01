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
            src: "/testmodel/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            src: "/testmodel/{ID}",
            method: "get"
        },
        insert: {
            src: "/testmodel/add",
            method: "post"
        },
        update: {
            src: "/testmodel/edit",
            method: "put"
        },
        delete: {
            src: "/testmodel/BatchDelete",
            method: "post"
        },
        import: {
            src: "/testmodel/import",
            method: "post"
        },
        export: {
            src: "/testmodel/ExportExcel",
            method: "post"
        },
        exportIds: {
            src: "/testmodel/ExportExcelByIds",
            method: "post"
        },
        template: {
            src: "/testmodel/GetExcelTemplate",
            method: "get"
        }
    }
}
export default new Store();
