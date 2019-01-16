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
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
import DataSource from 'store/dataSource';
var Store = /** @class */ (function (_super) {
    __extends(Store, _super);
    function Store() {
        var _this = _super.call(this) || this;
        // 动作权限  可在路由进入的时候注入
        _this.Actions = {
            insert: true,
            update: true,
            delete: true,
            import: true,
            export: true,
        };
        /** 数据 ID 索引 */
        _this.IdKey = 'ID';
        _this.Urls = __assign({}, _this.Urls, { search: {
                src: "/user/search",
                method: "post"
            }, details: {
                // 支持 嵌套 参数 /user/{ID}/{AAA}/{BBB}
                src: "/user/{ID}",
                method: "get"
            }, insert: {
                src: "/user/add",
                method: "post"
            }, update: {
                src: "/user/edit",
                method: "put"
            }, delete: {
                src: "/user/delete",
                method: "get"
            }, import: {
                src: "/user/import",
                method: "post"
            }, export: {
                src: "/user/ExportExcel",
                method: "post"
            }, exportIds: {
                src: "/user/ExportExcelByIds",
                method: "post"
            }, template: {
                src: "/user/GetExcelTemplate",
                method: "get"
            } });
        return _this;
    }
    return Store;
}(DataSource));
export { Store };
export default new Store();
//# sourceMappingURL=index.js.map