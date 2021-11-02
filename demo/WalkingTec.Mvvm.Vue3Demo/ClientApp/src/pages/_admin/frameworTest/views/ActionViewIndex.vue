

<template>
<div v-if="isRowAction">

    <a-button @click="to_Admin_FrameworkUser_Edit()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-pencil-square"></span></template>
      <span>修改</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_Details()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-info-circle"></span></template>
      <span>详情</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_BatchDelete()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-trash"></span></template>
      <span>删除</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_Password()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-unlock-alt"></span></template>
      <span>修改密码</span>
    </a-button>
</div>
<div v-else>

    <a-button @click="to_Admin_FrameworkUser_Create()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-plus"></span></template>
      <span>新建</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_Import()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-tasks"></span></template>
      <span>导入</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_BatchDelete()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-trash"></span></template>
      <span>删除</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_BatchEdit()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-pencil-square"></span></template>
      <span>批量修改</span>
    </a-button>
    <a-button @click="to_Admin_FrameworkUser_ExportExcel()">
      <template #icon><span class="_wtmicon _wtmicon-fa fa-arrow-circle-down"></span></template>
      <span>导出</span>
    </a-button>
</div>
</template>



<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends Vue {
  // page Inject 注入 row 为 toRowAction 注入
  @Inject() readonly PageController: PageController;
  get Pagination() {
    return this.PageController.Pagination;
  }
  /**
   * 行 操作 的参数 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  readonly params = {};
  /**
   * 行数据操作 有 aggrid 传入属性
   * @readonly
   * @memberof Action
   */
  get isRowAction() {
    return this.lodash.has(this.params, "node");
  }
  getRowData() {
    if (this.isRowAction) {
      return this.lodash.cloneDeep(this.lodash.get(this, "params.data", {}));
    }
    return this.lodash.cloneDeep(
      this.lodash.head(this.Pagination.selectionDataSource)
    );
  }
  
  to_Admin_FrameworkUser_Create() {
    this.__wtmToDetails({
      _AdminFrameworkUserCreate: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_Edit() {
    this.__wtmToDetails({
      _AdminFrameworkUserEdit: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_Details() {
    this.__wtmToDetails({
      _AdminFrameworkUserDetails: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_Import() {
    this.__wtmToDetails({
      _AdminFrameworkUserImport: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_BatchDelete() {
    this.__wtmToDetails({
      _AdminFrameworkUserBatchDelete: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_BatchEdit() {
    this.__wtmToDetails({
      _AdminFrameworkUserBatchEdit: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_ExportExcel() {
    this.__wtmToDetails({
      _AdminFrameworkUserExportExcel: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  to_Admin_FrameworkUser_Password() {
    this.__wtmToDetails({
      _AdminFrameworkUserPassword: this.lodash.get(
        this.getRowData(),
        this.PageController.key
      )
    });
  }
  mounted() {}
}
</script>

