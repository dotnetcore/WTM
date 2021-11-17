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
import chart from "@/components/page/echarts.vue"
@Options({ components: {
  chart
} })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) readonly formState = {
    ITCode: "",
    Name: "",
  };
  readonly EnumLocaleLabel = EnumLocaleLabel;
  get Pagination() {
    return this.PageController.Pagination;
  }
  async onFinish(values) {
    eval("this.$parent.$refs.C1.refresh()");
    await this.Pagination.onLoading(values);
    // this.onText();
  }
  async onReset(values) {
    await this.Pagination.onLoading(values);
  }
  mounted() { }
}
</script>
<style lang="less">
</style>
