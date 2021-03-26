<template>
  <WtmFilter
    :PageController="PageController"
    @finish="onFinish"
    @reset="onReset"
  >
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
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) readonly formState = {
    ITCode: "2",
    Name: "",
  };
  readonly EnumLocaleLabel = EnumLocaleLabel;
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
    this.Pagination.onSet(
      this.lodash.range(1, 50).map((x) => ({ ITCode: x, Name: "测试" + x }))
    );
  }
  mounted() {}
}
</script>
<style lang="less">
</style>
