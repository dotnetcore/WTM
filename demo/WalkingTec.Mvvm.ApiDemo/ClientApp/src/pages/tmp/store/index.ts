import DataSource from 'store/dataSource';
export class Store extends DataSource {
    constructor() {
        super();
    }
    /** 数据 ID 索引 */
    IdKey = 'ID';
    /** 地址配置列表 */
    Urls: WTM.IUrls = {
        search: {
            url: "/frameworkuser/search",
            method: "post"
        },
        details: {
            // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
            url: "/frameworkuser/{ID}",
            method: "get"
        },
        insert: {
            url: "/frameworkuser/add",
            method: "post"
        },
        update: {
            url: "/frameworkuser/edit",
            method: "put"
        },
        delete: {
            url: "/frameworkuser/BatchDelete",
            method: "post"
        },
        import: {
            url: "/frameworkuser/import",
            method: "post"
        },
        export: {
            url: "/frameworkuser/ExportExcel",
            method: "post"
        },
        exportIds: {
            url: "/frameworkuser/ExportExcelByIds",
            method: "post"
        },
        template: {
            url: "/frameworkuser/GetExcelTemplate",
            method: "get"
        }
    }

}
export default new Store();