<template>
  <a-modal
    class="w-view"
    v-if="isModal"
    @cancel="onCancel"
    footer=""
    :visible="visible"
    :title="modalTitle"
  >
    <slot />
  </a-modal>
  <a-drawer
    class="w-view"
    v-else
    :width="width"
    :visible="visible"
    :title="modalTitle"
    placement="right"
    @close="onCancel"
  >
    <slot />
  </a-drawer>
</template>

<script lang="ts">
import { Options, Prop, Vue, Provide, Ref } from "vue-property-decorator";
@Options({
  components: {},
})
export default class extends Vue {
  /** query参数的Key 默认取 detailsVisible */
  @Prop() queryKey: string;
  /** 标题  */
  @Prop() title: string;
  get visibleKey() {
    return this.queryKey || this.$WtmConfig.detailsVisible;
  }
  get isModal() {
    return false;
  }
  get query() {
    return this.lodash.clone(this.$route.query);
  }
  get visible() {
    return this.lodash.has(this.query, this.visibleKey);
  }
  get modalTitle() {
    if (this.title) {
      return this.title;
    }
    return this.lodash.get(this.query, this.visibleKey)
      ? this.$t("action.update")
      : this.$t("action.insert");
  }
  get width() {
    return this.lodash.max([500, window.innerWidth * 0.5]);
  }
  onCancel() {
    this.__wtmBackDetails();
  }
  created() {}
}
</script>
<style  lang="less">
.w-view {
  &.ant-drawer {
    .ant-drawer-content-wrapper {
      max-width: 100vw;
    }
  }
}
</style>
