/**
 * 查询复用 混入
 * tempSearch: 查询列表参数
 * callBack = () => {}
 * 需要在组件 methods 添加命名为“privateRequest”的查询方法 查询组件自己的调用接口 返回
 */
import { Component, Vue, Prop } from "vue-property-decorator";
type searchFormType = {
    orderByColumn: string | null; // 排序字段
    isAsc: string | null; // 排序
    [property: string]: any; //其他属性
};
declare module "vue/types/vue" {
    interface Vue {
        privateRequest: any;
        resetFields: any;
    }
    interface Element {
        resetFields: any;
    }
}
function mixinFunc(tempSearch: any = {}) {
    class mixin extends Vue {
        searchForm: searchFormType = {
            orderByColumn: null, // 排序字段
            isAsc: null, // asc desc
            ...tempSearch
        };
        // elementui 分页参数
        pageDate = {
            pageSizes: [10, 25, 50, 100],
            pageSize: 10,
            currentPage: 1,
            pageTotal: 0
        };
        searchFormClone: Object = {}; // 克隆数据
        loading: boolean = false; // 加载中
        tableData: Array<any> = []; // 列表数据
        selectedList: Array<any> = []; // 列表选中数据
        // 查询
        fetch(changePage?: boolean) {
            this.loading = true;
            // 翻页的时候，请求参数不变。
            if (!changePage) {
                this.searchFormClone = { ...this.searchForm };
            }
            const params = {
                ...this.searchFormClone,
                Page: this.pageDate.currentPage,
                Limit: this.pageDate.pageSize
            };
            for (const key in params) {
                if (
                    params[key] === "" ||
                    // params[key] === null ||
                    params[key] === undefined
                ) {
                    delete params[key];
                } else if (Array.isArray(params[key])) {
                    //数组类型的参数，变为字符串join (',')
                    params[key] = params[key].toString();
                }
            }
            if (params["isAsc"]) {
                params["SortInfo"] = {
                    Direction: params["isAsc"],
                    Property: params["orderByColumn"]
                };
            }
            // 组件查询中方法
            this["privateRequest"](params, this.pageDate.currentPage)
                .then(repData => {
                    this.loading = false;
                    this.pageDate.pageTotal = repData.Count || 0;
                    this.tableData = repData.Data || [];
                })
                .catch(err => {
                    console.log(err);
                    this.loading = false;
                });
        }
        onSearch() {
            this.pageDate.currentPage = 1;
            Object.keys(tempSearch).forEach(key => {
                this.searchForm[key] = tempSearch[key];
            });
            this.fetch();
        }
        onHoldSearch() {
            this.fetch();
        }
        onSearchForm() {
            this.pageDate.currentPage = 1;
            this.fetch();
        }
        // 重置
        onReset(formName) {
            Object.keys(tempSearch).forEach(key => {
                this.searchForm[key] = tempSearch[key];
            });
            this.pageDate.currentPage = 1;
            if (formName) {
                //去除搜索中的error信息
                _.get(this, `$refs[${formName}]`).resetFields();
            }
            this.onSearch();
        }
        // 页码大小
        handleSizeChange(size) {
            this.pageDate.currentPage = 1;
            this.pageDate.pageSize = size;
            this.fetch(true);
        }
        // 翻页
        handleCurrentChange(currentpage) {
            this.pageDate.currentPage = currentpage;
            console.log("currentpage", currentpage);
            this.fetch(true);
        }
        // 排序
        onSortChange({ prop, order }) {
            this.searchForm.orderByColumn = prop;
            this.pageDate.currentPage = 1;
            if (order === "ascending") {
                this.searchForm.isAsc = "asc";
            } else if (order === "descending") {
                this.searchForm.isAsc = "desc";
            } else {
                this.searchForm.isAsc = null;
                this.searchForm.isAsc = null;
            }
            this.fetch();
        }
        onSelectionChange(selectData: Array<any>) {
            this.selectedList = selectData;
        }
    }
    return Component(mixin);
}
export default mixinFunc;
