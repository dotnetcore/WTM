<template>
  <a-modal
    v-if="$GlobalConfig.settings.infoType==='Modal'"
    wrapClassName="page-action-modal"
    :class="slotName"
    key="page-action-modal"
    :destroyOnClose="true"
    :visible="visible"
    :width="width"
    @cancel="onVisible"
    @ok="onOk"
  >
    <!-- :title="$t(title)" -->
    <template #title>
      <h4 v-w-swipe="'page-action-modal'" v-text="title" class="page-action-modal-title" />
    </template>
    <slot></slot>
  </a-modal>
  <a-drawer
    v-else
    :title="title"
    :visible="visible"
    :destroyOnClose="true"
    :width="width"
    placement="right"
    @close="onVisible"
  >
    <slot></slot>
  </a-drawer>
</template> 
<script lang="ts">
import { WrappedFormUtils } from "ant-design-vue/types/form/form";
import { Component, Prop, Vue } from "vue-property-decorator";
import { EntitiesPageStore } from "@leng/public/src";
import { ICellRendererParams } from "ag-grid-community";
import { Modal } from "ant-design-vue";
import lodash from "lodash";
import { Subscriber, Subject } from "rxjs";
import { toJS } from "mobx";
import ImportUpload from "../upload/import.vue";
import { messages } from "./messages";
// import { i18n } from "../../locale";
@Component({ components: {} })
export default class ViewAction extends Vue {
  @Prop({ default: () => "70vw" }) width: string | number;
  /**
   * 表单域
   */
  @Prop() form: WrappedFormUtils;
  /**
   * 加载状态
   */
  @Prop() spinning;
  /**
   * 显示 隐藏 状态
   */
  @Prop() visible;
  /**
   * 标题
   */
  @Prop() title = "";
  /**
   * slot key
   */
  @Prop() slotName = "";
  /**
   *  更改 显示 弹框状态
   */
  onVisible(visible = !this.visible) {
    // this.visible = visible;
    this.$emit("cancel", false);
  }
  /**
   * 表单 提交处理
   */
  onOk(e?) {
    e && e.preventDefault();
    this.$emit("submit", false);
  }
}
</script>
<style  lang="less">
.page-action-modal {
  .ant-modal {
    // min-width: 70vw;
    // max-width: 99vw;
    // overflow: hidden;
    top: 50px;
    .ant-modal-body {
      max-height: calc(100vh - 200px);
      overflow: auto; // padding-bottom: 65px;
    }
    .page-action-modal-title {
      margin: 0;
    }
    &.Details {
      .ant-modal-footer button.ant-btn-primary {
        display: none;
      }
    }
  }
}
.page-import {
  .ant-modal-confirm-body .ant-modal-confirm-content {
    margin: 0;
  }
  .ant-modal-confirm-body > .anticon {
    display: none;
  }
}
</style>