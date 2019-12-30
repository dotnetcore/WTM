<template>
  <ViewActionsBasics :PageStore="PageStore" :params="params">
    <template #Insert>
      <Entity-ITCode :options="{initialValue:'aaaaa'}" />
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
  params = {};
  mounted() {}
}
</script>
<style scoped lang="less">
</style>