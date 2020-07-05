/**
 * 查询复用 混入
 *    需要在组件 methods 添加命名为“privateRequest”的查询方法 查询组件自己的调用接口
 *    TABLE_HEADER: 列表
 *    请求参数从CreateForm中获取
 * 注：
 *    当前方法（privateRequest）放到action-mixin中
 *    目前与CreateForm组件高度依赖，做一需要配合CreateForm组件
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
// delete ------, LOCAL: any = null
function mixinFunc(TABLE_HEADER: Array<object> = []) {
  class mixin extends Vue {
    searchRefName: string = "searchName";
    tableHeader: Array<object> = TABLE_HEADER;
    searchForm: searchFormType = {
      orderByColumn: null, // 排序字段
      isAsc: null // asc desc
    };
    pageDate = {
      pageSizes: [10, 25, 50, 100],
      pageSize: 10,
      currentPage: 1,
      pageTotal: 0
    };
    searchFormClone: object = {}; // 克隆数据
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
    /**
     * 查询
     * @param changePage
     */
    private fetch(changePage?: boolean) {
      this.loading = true;
      // 翻页的时候，请求参数不变。
      if (!changePage) {
        const comp = _.get(this.$refs, this.searchRefName);
        const data = comp ? comp.getFormData() : {};
        this.searchFormClone = { ...data, ...this.searchForm };
      }
      const params = {
        ...this.searchFormClone,
        Page: this.pageDate.currentPage,
        Limit: this.pageDate.pageSize
      };
      if (params["isAsc"]) {
        params["SortInfo"] = {
          Direction: params["isAsc"],
          Property: params["orderByColumn"]
        };
      }
      for (const key in params) {
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
        .then(repData => {
          this.loading = false;
          this.pageDate.pageTotal = repData.Count || 0;
          this.tableData = repData.Data || [];
        })
        .catch(error => {
          this.showResponseValidate(error.response.data.Form)
          this.loading = false;
        });
    }
    /**
     * 查询
     */
    public onSearch() {
      this.pageDate.currentPage = 1;
      this.fetch();
    }
    /**
     * 保持参数查询
     */
    public onHoldSearch() {
      this.fetch(true);
    }
    /**
     * 页码大小
     * @param size
     */
    public handleSizeChange(size) {
      this.pageDate.currentPage = 1;
      this.pageDate.pageSize = size;
      this.fetch(true);
    }
    /**
     * 翻页
     * @param currentpage
     */
    public handleCurrentChange(currentpage) {
      this.pageDate.currentPage = currentpage;
      this.fetch(true);
    }
    /**
     * 排序
     * @param prop 字段
     * @param order 顺序
     */
    public onSortChange({ prop, order }) {
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
    /**
     * 选中数据
     * @param selectData
     */
    public onSelectionChange(selectData: Array<any>) {
      this.selectData = selectData;
    }
    /**
     * 展示接口 验证错误提示
     */
    private showResponseValidate(resForms: {}) {
      _.get(this.$refs, this.searchRefName).showResponseValidate(resForms);
    }
    created() {
      this.onSearch();
    }
    beforeCreate() {
      // if (LOCAL && !this.$i18n.getLocaleMessage('en')[this.$options.name]) {
      //   this.$i18n.mergeLocaleMessage("en", LOCAL.en);
      //   this.$i18n.mergeLocaleMessage("zh", LOCAL.zh);
      // }
    }
  }
  return Component(mixin);
}
export default mixinFunc;
