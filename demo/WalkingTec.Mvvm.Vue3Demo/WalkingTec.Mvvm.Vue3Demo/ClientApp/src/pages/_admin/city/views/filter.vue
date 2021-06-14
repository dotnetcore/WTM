<template>
  <a-row :gutter="16">
    <a-col :span="6">
      <ViewTree @select="onTreeSelect" />
    </a-col>
    <a-col :span="18">
      <WtmFilter
        :PageController="PageController"
        @finish="onFinish"
        @reset="onReset"
      >
        <WtmField entityKey="ParentId_Filter" />
      </WtmFilter>
      <slot />
    </a-col>
  </a-row>
</template>
<script lang="ts">
import { Vue, Options, Provide, Inject } from "vue-property-decorator";
import ViewTree from "./tree.vue";
import { PageController } from "../controller";
import { EnumLocaleLabel } from "../locales";
@Options({ components: { ViewTree } })
export default class extends Vue {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) readonly formState = {
    ParentId: null
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
  onTreeSelect(ParentId) {
    this.formState.ParentId = ParentId;
    this.onFinish(this.formState);
  }
  mounted() {}
}
</script>
<style lang="less"></style>
