<template>
  <ViewActionsBasics :PageStore="PageStore" :params="params">
    <template #Insert>
      <Entity-ITCode :decoratorOptions="{initialValue:'aaaaa'}" :disabled="true" />
    </template>
    <template #Update>
      <Entity-Password />
      <Entity-ITCode />
    </template>
  </ViewActionsBasics>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { ICellRendererParams } from "ag-grid-community";
import ViewActionsBasics from "../../../components/pages/actions.vue";
import { createFormItem } from "../../../components/utils/entitiesHelp";
import { Modal } from "ant-design-vue";
import PageStore from "../store";
import Entities from "./entities";
import lodash from "lodash";
@Component({
  components: {
    ViewActionsBasics,
    // 创建 实体 组件
    ...createFormItem({ entities: Entities.editEntities() })
  }
})
export default class ViewAction extends Vue {
  @Prop() private PageStore: PageStore;
  // aggird 组件 自带属性 不可删除
  params = {};
  mounted() {}
}
</script>
<style scoped lang="less">
</style>