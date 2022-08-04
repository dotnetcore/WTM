<template>
  <a-button
    v-if="isPageAction && isRef"
    v-bind="ButtonProps"
    @click="onToReful"
  >
    <template #icon v-if="isPageAction">
      <slot name="icon">
        <RedoOutlined />
      </slot>
    </template>
    <slot>
      <i18n-t :keypath="$locales.action_ref" />
    </slot>
  </a-button>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { $locales, ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController: ControllerBasics;
  /**
   * 行 操作需要 aggrid 传入
   * @type {ICellRendererParams}
   * @memberof Action
   */
  @Prop() readonly params;
  /** 请求参数 */
  @Prop({}) toQuery;
  get successMsg() {
    return this.$t(this.$locales.tips_success_operation);
  }
  get errorMsg() {
    return this.$t(this.$locales.tips_error_operation);
  }
  async onToReful() {
  const { successMsg, errorMsg } = this;
  try {
     await this.PageController.onRef();
     this.$message.success(successMsg);
    } catch (error) {
      this.$message.error(errorMsg);
    }
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
