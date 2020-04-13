<template>
  <w-filter :PageStore="PageStore" :Entities="Entities">
  <GroupCode/>
  <GroupName/>
  </w-filter>
</template>
<script lang="ts">
import lodash from "lodash";
import { Subject } from "rxjs";
import { Component, Prop, Vue, Inject } from "vue-property-decorator";
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import wtm from "../../../components";
import PageStore from "../store";
import Entities from "./entities";
const entities = Entities.filterEntities();
@Component({
  components: wtm.createFilterFormItem({
    entities
  })
})
export default class ViewGrid extends Vue {
  @Inject("PageStore")
  PageStore: PageStore;
  Entities = entities;
  // FieldsChange = new Subject<{
  //   props: any;
  //   fields: any;
  //   form: WrappedFormUtils;
  // }>();
  mounted() {}
}
</script>
<style scoped lang="less">
</style>