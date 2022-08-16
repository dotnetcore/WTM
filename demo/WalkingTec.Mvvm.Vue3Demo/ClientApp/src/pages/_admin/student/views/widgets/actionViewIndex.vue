<template>
<div v-if="isRowAction">
    <a-button v-if="__wtmAuthority('update', PageController)" type='link' @click="toModels_Student_Edit()">
      <span><i18n-t keypath="修改" /></span>
    </a-button>
    <a-button v-if="__wtmAuthority('details', PageController)" type='link' @click="toModels_Student_Details()">
      <span><i18n-t keypath="详情" /></span>
    </a-button>
    <WtmActionDelete :PageController="PageController" :params="params">
      <template #icon><span class="fa fa-trash" style='margin-right:5px'></span></template>
        <span><i18n-t keypath="删除" /></span>
    </WtmActionDelete>
</div>

<div v-else class="textr">
    <a-button v-if="__wtmAuthority('insert', PageController)" @click="toModels_Student_Create()">
      <template #icon><span class="fa fa-plus" style='margin-right:5px'></span></template>
      <span><i18n-t keypath="新建" /></span>
    </a-button>
    <WtmActionDelete :PageController="PageController" :params="params">
      <template #icon><span class="fa fa-trash" style='margin-right:5px'></span></template>
        <span><i18n-t keypath="删除" /></span>
    </WtmActionDelete>
    <a-button @click="toModels_Student_BatchEdit()">
      <template #icon><span class="fa fa-pencil-square" style='margin-right:5px'></span></template>
      <span><i18n-t keypath="批量修改" /></span>
    </a-button>
    <WtmActionImport :PageController="PageController" :params="params">
      <template #icon><span class="fa fa-tasks" style='margin-right:5px'></span></template>
        <span><i18n-t keypath="导入" /></span>
    </WtmActionImport>
    <WtmActionExport :PageController="PageController" :params="params">
      <template #icon><span class="fa fa-arrow-circle-down" style='margin-right:5px'></span></template>
        <span><i18n-t keypath="导出" /></span>
    </WtmActionExport>
</div>
</template>
<script lang="ts">
import { Inject, Options, Provide, Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { StudentPageController,ExStudentPageController } from "../../controller";
@Options({ components: {} })
export default class extends Vue {
  // page Inject 注入 row 为 toRowAction 注入
  @Provide({ reactive: true }) readonly PageController = ExStudentPageController;
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
  toModels_Student_Create() {
    this.__wtmToDetails(
       this.lodash.assign(
         {
             modelsstudentcreate:''
         },
         { type: 'Dialog' },
         { editType:'create'}
        )
    );
  }
  toModels_Student_Edit() {
    this.__wtmToDetails(
       this.lodash.assign(
         {
             modelsstudentedit: this.lodash.get(
                 this.getRowData(),
                 this.PageController.key
             )
         },
         { type: 'Self' },
         { editType:'edit'}
        )
    );
  }
  toModels_Student_Details() {
    this.__wtmToDetails(
       this.lodash.assign(
         {
             modelsstudentdetails: this.lodash.get(
                 this.getRowData(),
                 this.PageController.key
             )
         },
         { _readonly: '' }
        )
    );
  }
  toModels_Student_BatchEdit() {
    let ids = this.lodash.map(this.PageController.Pagination.selectionDataSource, this.PageController.key).join("|")
    this.__wtmToDetails(
       this.lodash.assign(
         {
             modelsstudentbatchedit: this.lodash.get(
                 this.getRowData(),
                 this.PageController.key
             )
         },
         { ids:ids},
         { type: 'Blank'},
         { editType:'BatchEdit'}
        )
    );
  }
  mounted() {}
}
</script>

