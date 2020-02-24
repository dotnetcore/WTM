<template>
  <w-actions
    :PageStore="PageStore"
    :Entities="Entities"
    :params="params"
  >
    <!-- :PageActions="{insert:false,update:false,import:false}" -->
    <template #Details>
      <Entity-ModuleName display/>
      <Entity-ActionName display/>
      <Entity-ITCode display/>
      <Entity-ActionUrl display/>
      <Entity-ActionTime display/>
      <Entity-Duration display/>
      <Entity-Remark display/>
      <Entity-IP display/>
      <Entity-LogType display/>
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
  mounted() {}
}
</script>
<style scoped lang="less">
</style>