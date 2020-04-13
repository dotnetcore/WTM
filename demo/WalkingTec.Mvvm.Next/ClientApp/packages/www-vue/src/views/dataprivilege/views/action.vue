<template>
  <w-actions
    :PageStore="PageStore"
    :Entities="Entities"
    :params="params"
    @fieldsChange="onFieldsChange"
    width="500px"
  >
    <!-- @submit="onSubmit" -->
    <template #Insert>
      <DpType />
      <IsAll />
      <Entity-TableName />
      <SelectedItemsID v-if="!IsAll" />
      <Entity-GroupId v-if="DpType===0"/>
      <UserItCode v-if="DpType===1"/>
    </template>
    <template #Update>
      <Entity-ID v-show="false" />
      <DpType />
      <IsAll />
      <Entity-TableName />
      <SelectedItemsID v-if="!IsAll" />
      <Entity-GroupId v-if="DpType===0"/>
      <UserItCode v-if="DpType===1"/>
    </template>
    <template #Details>
      <DpType display />
      <Entity-TableName display />
      <SelectedItemsID display />
      <IsAll display />
      <UserItCode display />
      <Entity-GroupId display />
    </template>
  </w-actions>
</template> 
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue, Inject } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subject } from "rxjs";
import wtm from "../../../components";
import PageStore from "../store";
import Entities from "./entities";
const entities = Entities.editEntities();
@Component({
  components: wtm.createFormItem({ entities })
})
export default class ViewAction extends Vue {
  @Inject("PageStore")
  PageStore: PageStore;
  Entities = entities;
  // FieldsChange = new Subject<{
  //   props: any;
  //   fields: any;
  //   form: WrappedFormUtils;
  // }>();
  // aggird 组件 自带属性 不可删除
  params = {};
  IsAll = true;
  DpType = 0;
  /** form onFieldsChange 事件 */
  onFieldsChange(props, fields) {
    if (lodash.hasIn(fields, "IsAll")) {
      this.IsAll = lodash.get(fields, "IsAll.value");
    }
    if (lodash.hasIn(fields, "DpType")) {
      this.DpType = lodash.get(fields, "DpType.value");
    }
  }
  /** @submit="onSubmit" 替换 默认 提交函数 */
  onSubmit(value, type, v) {}
  mounted() {}
}
</script>
<style scoped lang="less">
</style>