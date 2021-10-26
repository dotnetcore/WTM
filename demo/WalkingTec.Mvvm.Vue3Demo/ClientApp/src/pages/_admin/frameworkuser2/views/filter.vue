<template>
  <WtmFilter :PageController="PageController" @finish="onFinish" @reset="onReset">
    <WtmField entityKey="ITCode_Filter" />
    <WtmField entityKey="Name_Filter" />
  </WtmFilter>
</template>
<script lang="ts">
import { Vue, Options, Provide, Inject } from "vue-property-decorator";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
@Options({ components: {} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  @Inject({ from: "LogPage" }) readonly LogPage: PageController;
  @Provide({ reactive: true }) readonly formState = {
    ITCode: "",
    Name: "",
  };
  readonly EnumLocaleLabel = EnumLocaleLabel;
  get Pagination() {
    return this.PageController.Pagination;
  }
  get LogPagination() {
    return this.LogPage.Pagination;
  }
  async onFinish(values) {
     this.Pagination.onLoading(values);
     this.LogPagination.onLoading(values)
    // this.onText();
  }
  async onReset(values) {
     this.Pagination.onLoading(values);
     this.LogPagination.onLoading(values);
  }
  mounted() { }
}
</script>
<style lang="less">
</style>
