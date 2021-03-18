<template>
  <WtmFilter :PageController="PageController" @finish="onFinish">
    <WtmField name="ITCode" :label="EnumLocaleLabel.ITCode" />
    <WtmField name="Name" :label="EnumLocaleLabel.Name" />
  </WtmFilter>
</template>
<script lang="ts">
import { Vue, Options, Provide, Inject } from "vue-property-decorator";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
@Options({ components: {} })
export default class extends Vue {
  @Inject() PageController: PageController;
  @Provide() formState = {
    ITCode: "2",
    Name: "",
  };
  EnumLocaleLabel = EnumLocaleLabel;
  get Pagination() {
    return this.PageController.Pagination;
  }
  onFinish(values) {
    this.Pagination.onLoading(values);
  }
  mounted() {
    console.log("LENG ~ Filter", this);
  }
}
</script>
<style lang="less">
</style>
