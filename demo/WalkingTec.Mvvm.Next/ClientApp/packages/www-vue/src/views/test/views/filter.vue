<template>
  <w-filter :PageStore="PageStore" :FieldsChange="FieldsChange">
    <ITCode />
    <Name />
  </w-filter>
</template>
<script lang="ts">
import lodash from "lodash";
import { Subject } from "rxjs";
import { Component, Prop, Vue } from "vue-property-decorator";
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import wtm from "../../../components";
import PageStore from "../store";
import Entities from "./entities";
const entities = Entities.filterEntities();
@Component({
  components: wtm.createFormItem({
    entities,
    colProps: { xs: 24, sm: 24, md: 12, lg: 8 }
  })
})
export default class ViewGrid extends Vue {
  @Prop() PageStore: PageStore;
  Entities = entities;
  FieldsChange = new Subject<{
    props: any;
    fields: any;
    form: WrappedFormUtils;
  }>();
}
</script>
<style scoped lang="less">
</style>