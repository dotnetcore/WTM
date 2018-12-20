import StoreBasice from 'store/table';
export class Store extends StoreBasice {
    constructor() {
        super();
    }
    /** 数据 ID 索引 */
    IdKey = 'id';
    Url = {
        search: {
            src: "/test/search",
            method: "post"
        },
        details: {
            src: "/test/details/{id}",
            method: "get"
        },
        insert: {
            src: "/test/insert",
            method: "post"
        },
        update: {
            src: "/test/update",
            method: "post"
        },
        delete: {
            src: "/test/delete",
            method: "post"
        },
        import: {
            src: "/test/import",
            method: "post"
        },
        export: {
            src: "/test/export",
            method: "post"
        },
        template: {
            src: "/test/template",
            method: "post"
        }
    }
}
export default new Store();