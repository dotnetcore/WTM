<template>
  <WtmFilter
    :PageController="PageController"
    @finish="onFinish"
    @reset="onReset"
  >
    <WtmField entityKey="ITCode_Filter" />
    <WtmField entityKey="ActionUrl_Filter" />
    <WtmField entityKey="ActionTime_Filter" />
    <!-- 更多搜索条件 默认隐藏 -->
    <template #more>
      <WtmField entityKey="IP_Filter" />
      <WtmField entityKey="LogType_Filter" />
    </template>
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
    ITCode: "",
    Name: ""
  };
  readonly EnumLocaleLabel = EnumLocaleLabel;
  get Pagination() {
    return this.PageController.Pagination;
  }
  async onFinish(values) {
    await this.Pagination.onLoading(values);
    // this.onText();
  }
  async onReset(values) {
    await this.Pagination.onLoading(values);
  }
  mounted() {}
}
</script>
<style lang="less"></style>
