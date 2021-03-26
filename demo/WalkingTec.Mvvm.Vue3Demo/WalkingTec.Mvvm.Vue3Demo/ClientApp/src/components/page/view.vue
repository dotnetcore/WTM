/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-03-12 17:20:19
 * @modify date 2021-03-12 17:20:19
 * @desc 详情视图 modal 或者 drawer 方式 根据全局配置而定
 */
<template>
  <a-modal
    v-if="isModal"
    class="w-view"
    footer=""
    :visible="visible"
    :title="_title"
    destroyOnClose
    @cancel="onCancel"
  >
    <slot />
  </a-modal>
  <a-drawer
    v-else
    class="w-view"
    :width="_width"
    :visible="visible"
    :title="_title"
    placement="right"
    destroyOnClose
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
  @Prop() readonly queryKey: string;
  /** 标题  */
  @Prop() readonly title: string;
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
  get _title() {
    if (this.title) {
      return this.title;
    }
    return this.lodash.get(this.query, this.visibleKey)
      ? this.$t("action.update")
      : this.$t("action.insert");
  }
  get _width() {
    const width = window.innerWidth * 0.5;
    return this.lodash.max([500, width > 800 ? 800 : width]);
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
