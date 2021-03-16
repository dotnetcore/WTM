<template>
  <WtmFilter
    :PageController="PageController"
    @finish="onFinish"
    @reset="onReset"
  >
    <WtmField name="ITCode" :label="EnumLocaleLabel.ITCode" />
    <WtmField name="Name" :label="EnumLocaleLabel.Name" />
    <WtmField name="Name" :label="EnumLocaleLabel.Name" />
    <WtmField name="Name" :label="EnumLocaleLabel.Name" />
    <WtmField name="Name" :label="EnumLocaleLabel.Name" />
    <WtmField name="Name" :label="EnumLocaleLabel.Name" />
    <!-- <div></div> -->
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
  async onFinish(values) {
    await this.Pagination.onLoading(values);
    this.onText();
  }
  async onReset(values) {
    await this.Pagination.onLoading(values);
  }
  onText() {
    this.Pagination.onSet([
      { ITCode: 123, Name: "测试" },
      { ITCode: 12, Name: "测试" },
      { ITCode: 3, Name: "测试" },
      { ITCode: 4, Name: "测试" },
      { ITCode: 5, Name: "测试" },
    ]);
  }
  mounted() {
  }
}
</script>
<style lang="less">
</style>
