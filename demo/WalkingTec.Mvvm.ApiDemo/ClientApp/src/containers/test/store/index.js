var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
import StoreBasice from 'store/table';
var Store = /** @class */ (function (_super) {
    __extends(Store, _super);
    function Store() {
        var _this = _super.call(this) || this;
        /** 数据 ID 索引 */
        _this.IdKey = 'id';
        _this.Url = {
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
        };
        return _this;
    }
    return Store;
}(StoreBasice));
export { Store };
export default new Store();
//# sourceMappingURL=index.js.map