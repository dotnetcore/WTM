<template>
  <w-actions :PageStore="PageStore" :Entities="Entities" :params="params" @fieldsChange="onFieldsChange">
    <template #Insert>
      <DpType />
      <IsAll />
      <Entity-TableName />
      <SelectedItemsID v-if="!IsAll" />
      <UserItCode />
      <Entity-GroupId />
    </template>
    <template #Update>
      <Entity-ID v-show="false"/>
      <DpType />
      <IsAll />
      <Entity-TableName />
      <SelectedItemsID v-if="!IsAll" />
      <UserItCode />
      <Entity-GroupId />
    </template>
    <template #Details>
      <DpType display/>
      <Entity-TableName display/>
      <SelectedItemsID display/>
      <IsAll display/>
      <UserItCode display/>
      <Entity-GroupId display/>
    </template>
  </w-actions>
</template> 
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue } from "vue-property-decorator";
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
  @Prop() private PageStore: PageStore;
  Entities = entities;
  // FieldsChange = new Subject<{
  //   props: any;
  //   fields: any;
  //   form: WrappedFormUtils;
  // }>();
  // aggird 组件 自带属性 不可删除
  params = {};
  IsAll=true;
  onFieldsChange(props,fields){
    if(lodash.hasIn(fields, 'IsAll.value')){
        this.IsAll=lodash.get(fields, 'IsAll.value');
    }
  }
  mounted() {}
}
</script>
<style scoped lang="less">
</style>