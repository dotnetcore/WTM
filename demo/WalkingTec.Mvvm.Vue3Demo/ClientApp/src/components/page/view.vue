/** * @author 冷 (https://github.com/LengYXin) * @email
lengyingxin8966@gmail.com * @create date 2021-03-12 17:20:19 * @modify date
2021-03-12 17:20:19 * @desc 详情视图 modal 或者 drawer 方式 根据全局配置而定 */
<template>
  <a-modal
    v-if="isModal"
    class="w-view"
    footer
    :visible="visible"
    :title="_title"
    destroyOnClose
    @cancel="onCancel"
  >
    <slot v-if="visible" />
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
    <slot v-if="visible" />
  </a-drawer>
</template>

<script lang="ts">
import { Options, Prop, Vue } from "vue-property-decorator";
@Options({
  components: {}
})
export default class extends Vue {
  /** query参数的Key 默认取 detailsVisible */
  @Prop() readonly queryKey: string;
  /** 固定页面 只在第一次打开的页面有效 */
  @Prop({ default: true }) readonly fixedPage: boolean;
  /** 标题  */
  @Prop() readonly title: string;
  /** 弹框类型 */
  @Prop() readonly modalType: "modal" | "drawer";
  /** 记录创建 时的 page */
  PageKey = null;
  get visibleKey() {
    return this.queryKey || this.$WtmConfig.detailsVisible;
  }
  get isModal() {
    const width = window.innerWidth;
    const modalType = this.modalType || this.$WtmConfig.modalType;
    return this.lodash.eq(modalType, "modal") && width > 701;
  }
  get query() {
    return this.lodash.clone(this.$route.query);
  }
  get visible() {
    const visible = this.lodash.has(this.query, this.visibleKey);
    if (this.fixedPage) {
      return this.lodash.eq(this.PageKey, this.$route.name) && visible;
    }
    return visible;
  }
  // 只读
  get readonly() {
    return this.lodash.has(this.$route.query, "_readonly");
  }
  get batch() {
    return this.lodash.has(this.$route.query, "_batch");
  }
  get _title() {
    if (this.title) {
      return this.title;
    }
    if (this.lodash.get(this.query, this.visibleKey) || this.batch) {
      let title = this.$locales.action_update;
      if (this.readonly) {
        title = this.$locales.action_info;
      }
      if (this.batch) {
        title = this.$locales.action_update_batch;
      }
      return this.$t(title);
    }
    return this.$t(this.$locales.action_insert);
  }
  get _width() {
    const width = window.innerWidth * 0.5;
    return this.lodash.max([500, width > 800 ? 800 : width]);
  }
  onCancel() {
    this.__wtmBackDetails(this.visibleKey);
  }
  created() {}
  mounted() {
    this.PageKey = this.$route.name;
  }
}
</script>
<style lang="less">
.w-view {
  &.ant-modal {
    min-width: 650px;
    .ant-modal-body {
      max-height: 75vh;
      // position: relative;
      overflow: auto;
    }
  }
  &.ant-drawer {
    .ant-drawer-content-wrapper {
      max-width: 100vw;
    }
  }
}
</style>
