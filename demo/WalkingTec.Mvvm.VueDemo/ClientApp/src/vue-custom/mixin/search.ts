/**
 * 查询复用 混入
 * SEARCH_DATA: 查询列表参数
 * TABLE_HEADER: 列表
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
function mixinFunc(SEARCH_DATA: any = {}, TABLE_HEADER: any = {}) {
  class mixin extends Vue {
    tableHeader = TABLE_HEADER;
    searchForm: searchFormType = {
      orderByColumn: null, // 排序字段
      isAsc: null, // asc desc
      ...SEARCH_DATA
    };
    pageDate = {
      pageSizes: [10, 25, 50, 100],
      pageSize: 10,
      currentPage: 1,
      pageTotal: 0
    };
    searchFormClone: Object = {}; // 克隆数据
    loading: boolean = false; // 加载中
    tableData: Array<any> = []; // 列表数据
    selectData: Array<any> = []; // 列表选中数据
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
    get searchEvent() {
      return {
        onSearch: this.onSearch,
        onReset: this.onReset,
        "size-change": this.handleSizeChange,
        "current-change": this.handleCurrentChange,
        "selection-change": this.onSelectionChange,
        "sort-change": this.onSortChange
      };
    }
    /**
     * 返回wtm-table-box属性集合
     * 包含el-table/el-pagination组件属性，命名与组件事件相同
     * wtm-table-box：loading组件加载判断。。。
     * el-table：data,is-selection,tb-header
     * el-pagination：pageSizes,pageSize,currentPage,pageTotal
     * 注：
     *    如searchEvent对象 一样；
     */
    get searchAttrs() {
      return {
        loading: this.loading,
        data: this.tableData,
        isSelection: true,
        tbHeader: this.tableHeader,
        ...this.pageDate
      };
    }
    // 查询 ★★★★★
    created() {
      this.onSearch();
    }
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
      this.fetch();
    }
    onHoldSearch() {
      this.fetch();
    }
    onSearchForm() {
      this.pageDate.currentPage = 1;
      Object.keys(SEARCH_DATA).forEach(key => {
        this.searchForm[key] = SEARCH_DATA[key];
      });
      this.fetch();
    }
    // 重置
    onReset(formName) {
      Object.keys(SEARCH_DATA).forEach(key => {
        this.searchForm[key] = SEARCH_DATA[key];
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
      console.log("handleCurrentChange", currentpage);
      this.pageDate.currentPage = currentpage;
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
      console.log("selectData", selectData);
      this.selectData = selectData;
    }
  }
  return Component(mixin);
}
export default mixinFunc;
