<template>
  <div>
    <a-popconfirm :title="title" @confirm="onConfirm">
      <a-button class="w-grid-del" type="link">
        <i18n-t :keypath="$locales.action_delete" />
      </a-button>
    </a-popconfirm>
  </div>
</template>
<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  /**
   * 行 操作 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  readonly params = {};
  get title() {
    return this.$t(this.$locales.action_deleteConfirm, { text: 1 });
  }
  get successMsg() {
    return this.$t(this.$locales.tips_success_operation);
  }
  get errorMsg() {
    return this.$t(this.$locales.tips_error_operation);
  }
  onConfirm() {
    this.lodash.invoke(this.params, "context.onRemove", this.params);
  }
  mounted() {}
}
</script>
<style lang="less">
.w-grid-del {
  padding: 0 !important;
}
</style>
