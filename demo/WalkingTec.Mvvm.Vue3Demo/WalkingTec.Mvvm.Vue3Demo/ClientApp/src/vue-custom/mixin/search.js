import { __assign, __extends } from "tslib";
/**
 * 查询复用 混入
 *    需要在组件 methods 添加命名为“privateRequest”的查询方法 查询组件自己的调用接口
 *    TABLE_HEADER: 列表
 *    请求参数从CreateForm中获取
 * 注：
 *    当前方法（privateRequest）放到action-mixin中
 *    目前与CreateForm组件高度依赖，做一需要配合CreateForm组件
 */
import { Component, Vue } from "vue-property-decorator";
// delete ------, LOCAL: any = null
function mixinFunc(TABLE_HEADER) {
    if (TABLE_HEADER === void 0) { TABLE_HEADER = []; }
    var mixin = /** @class */ (function (_super) {
        __extends(mixin, _super);
        function mixin() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            _this.searchRefName = "searchName";
            _this.tableHeader = TABLE_HEADER;
            _this.searchForm = {
                orderByColumn: null,
                isAsc: null // asc desc
            };
            _this.pageDate = {
                pageSizes: [10, 25, 50, 100],
                pageSize: 10,
                currentPage: 1,
                pageTotal: 0
            };
            _this.searchFormClone = {}; // 克隆数据
            _this.loading = false; // 加载中
            _this.tableData = []; // 列表数据
            _this.selectData = []; // 列表选中数据
            return _this;
        }
        Object.defineProperty(mixin.prototype, "searchEvent", {
            /**
             * 返回wtm-table-box事件集合
             * 包含el-table/el-pagination组件事件，命名定义要与组件（el-table，el-pagination）事件相同 可生效
             * el-table：sort-change
             * el-pagination：size-change，current-change，selection-change
             * 注：
             *    1. 需要添加elementui其他事件，可以在组件中写，不用添加此对象里；
             *    2. 可以定义方法覆盖事件；
             * 例如：
             * <wtm-table-box :events="searchEvent" @sort-change="()=>{覆盖方法}" @header-click="()=>{自定义方法}">
             */
            get: function () {
                return {
                    onSearch: this.onSearch,
                    "size-change": this.handleSizeChange,
                    "current-change": this.handleCurrentChange,
                    "selection-change": this.onSelectionChange,
                    "sort-change": this.onSortChange
                };
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(mixin.prototype, "searchAttrs", {
            /**
             * 返回wtm-table-box属性集合
             * 包含el-table/el-pagination组件属性，命名与组件事件相同
             * wtm-table-box：loading组件加载判断。。。
             * el-table：data,is-selection,tb-header
             * el-pagination：pageSizes,pageSize,currentPage,pageTotal
             * 注：
             *    如searchEvent对象 一样；
             */
            get: function () {
                return __assign({ loading: this.loading, data: this.tableData, isSelection: true, tbHeader: this.tableHeader }, this.pageDate);
            },
            enumerable: false,
            configurable: true
        });
        /**
         * 查询
         * @param changePage
         */
        mixin.prototype.fetch = function (changePage) {
            var _this = this;
            this.loading = true;
            // 翻页的时候，请求参数不变。
            if (!changePage) {
                var comp = _.get(this.$refs, this.searchRefName);
                var data = comp ? comp.getFormData() : {};
                this.searchFormClone = __assign(__assign({}, data), this.searchForm);
            }
            var params = __assign(__assign({}, this.searchFormClone), { Page: this.pageDate.currentPage, Limit: this.pageDate.pageSize });
            if (params["isAsc"]) {
                params["SortInfo"] = {
                    Direction: params["isAsc"],
                    Property: params["orderByColumn"]
                };
            }
            for (var key in params) {
                if (params[key] === "" || params[key] === undefined) {
                    delete params[key];
                }
                // 删除自定义字段
                if (["isAsc", "orderByColumn"].includes(key)) {
                    delete params[key];
                }
            }
            // 组件查询中方法
            this["privateRequest"](params, this.pageDate.currentPage)
                .then(function (repData) {
                _this.loading = false;
                _this.pageDate.pageTotal = repData.Count || 0;
                _this.tableData = repData.Data || [];
            })
                .catch(function (error) {
                _this.showResponseValidate(error.response.data.Form);
                _this.loading = false;
            });
        };
        /**
         * 查询
         */
        mixin.prototype.onSearch = function () {
            this.pageDate.currentPage = 1;
            this.fetch();
        };
        /**
         * 保持参数查询
         */
        mixin.prototype.onHoldSearch = function () {
            this.fetch(true);
        };
        /**
         * 页码大小
         * @param size
         */
        mixin.prototype.handleSizeChange = function (size) {
            this.pageDate.currentPage = 1;
            this.pageDate.pageSize = size;
            this.fetch(true);
        };
        /**
         * 翻页
         * @param currentpage
         */
        mixin.prototype.handleCurrentChange = function (currentpage) {
            this.pageDate.currentPage = currentpage;
            this.fetch(true);
        };
        /**
         * 排序
         * @param prop 字段
         * @param order 顺序
         */
        mixin.prototype.onSortChange = function (_a) {
            var prop = _a.prop, order = _a.order;
            this.searchForm.orderByColumn = prop;
            this.pageDate.currentPage = 1;
            if (order === "ascending") {
                this.searchForm.isAsc = "asc";
            }
            else if (order === "descending") {
                this.searchForm.isAsc = "desc";
            }
            else {
                this.searchForm.isAsc = null;
                this.searchForm.isAsc = null;
            }
            this.fetch();
        };
        /**
         * 选中数据
         * @param selectData
         */
        mixin.prototype.onSelectionChange = function (selectData) {
            this.selectData = selectData;
        };
        /**
         * 展示接口 验证错误提示
         */
        mixin.prototype.showResponseValidate = function (resForms) {
            _.get(this.$refs, this.searchRefName).showResponseValidate(resForms);
        };
        mixin.prototype.created = function () {
            this.onSearch();
        };
        mixin.prototype.beforeCreate = function () {
            // if (LOCAL && !this.$i18n.getLocaleMessage('en')[this.$options.name]) {
            //   this.$i18n.mergeLocaleMessage("en", LOCAL.en);
            //   this.$i18n.mergeLocaleMessage("zh", LOCAL.zh);
            // }
        };
        return mixin;
    }(Vue));
    return Component(mixin);
}
export default mixinFunc;
//# sourceMappingURL=search.js.map