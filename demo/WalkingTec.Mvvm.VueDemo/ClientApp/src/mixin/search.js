/**
 * 查询复用 混入
 * tempSearch: 查询列表参数
 * callBack = () => {}
 * 需要在组件 methods 添加命名为“privateRequest”的查询方法 查询组件自己的调用接口 返回
 */
const mixin = (tempSearch = {}) => {
    return {
        data() {
            return {
                // 查询条件
                searchForm: {
                    orderByColumn: null, // 排序字段
                    isAsc: null, // asc desc
                    ...tempSearch
                },
                searchFormClone: {}, // 真正查询的条件
                // 分页
                pageDate: {
                    pageSizes: [10, 25, 50, 100],
                    pageSize: 15,
                    currentPage: 1,
                    pageTotal: 0
                },
                loading: false
            };
        },
        mounted() {},
        methods: {
            // 接口调用 offset: 分页，count 条数
            fetch(changePage) {
                this.loading = true;
                // 翻页的时候，请求参数不变。
                if (!changePage) {
                    this.searchFormClone = { ...this.searchForm };
                }
                const params = {
                    ...this.searchFormClone,
                    Page: this.pageDate.currentPage - 1,
                    Limit: this.pageDate.pageSize
                };
                for (const key in params) {
                    if (
                        params[key] === "" ||
                        // params[key] === null ||
                        params[key] === undefined
                    ) {
                        delete params[key];
                    }
                }
                // Page: 0,
                // Limit: 0,
                // Count: 0,
                // PageCount: 0,
                // SortInfo: {
                //     Property: "string",
                //     Direction: 0
                // },
                // 组件查询中方法
                this.privateRequest(params, this.pageDate.currentPage)
                    .then(repData => {
                        console.log("repData", repData);
                        this.loading = false;
                        this.pageDate.pageTotal = repData.total || 0;
                    })
                    .catch(err => {
                        console.log(err);
                        this.loading = false;
                        this.$message.error(err && err.message);
                    });
            },
            // 查询 重置条件查询
            onSearch() {
                this.pageDate.currentPage = 1;
                Object.keys(tempSearch).forEach(key => {
                    this.searchForm[key] = tempSearch[key];
                });
                this.fetch();
            },
            // 保持现状
            onHoldSearch() {
                this.fetch();
            },
            // 返回第一页查询
            onSearchForm() {
                this.pageDate.currentPage = 1;
                this.fetch();
            },
            // 重置
            onReset(formName) {
                Object.keys(tempSearch).forEach(key => {
                    this.searchForm[key] = tempSearch[key];
                });
                this.pageDate.currentPage = 1;
                if (formName) {
                    //去除搜索中的error信息
                    this.$refs[formName].resetFields();
                }
                this.onSearch("");
            },
            // 页码大小
            handleSizeChange(size) {
                this.pageDate.currentPage = 1;
                this.pageDate.pageSize = size;
                this.fetch(true);
            },
            // 翻页
            handleCurrentChange(currentpage) {
                this.pageDate.currentPage = currentpage;
                console.log("currentpage", currentpage);
                this.fetch(true);
            },
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
        }
    };
};
export default mixin;
