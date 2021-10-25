<template>
  <a-popconfirm overlayClassName="x-action-import" @confirm="onConfirm">
    <template #icon>
      <div></div>
    </template>
    <template #title>
      <div v-if="__wtmAuthority(EnumActionType.Template, PageController)">
        <i18n-t :keypath="$locales.tips_text_importExplain" />
        <a-divider type="vertical" />
        <a-button @click="onGetTemplate">
          <template #icon>
            <CloudUploadOutlined />
          </template>
          <i18n-t :keypath="$locales.action_downloadTemplate" />
        </a-button>
        <a-divider style="margin:8px" />
      </div>
      <a-spin :spinning="loading">
        <a-upload-dragger
          name="file"
          :showUploadList="false"
          :action="action"
          :headers="headers"
          @change="onChange"
        >
          <p class="ant-upload-drag-icon">
            <inbox-outlined />
          </p>
          <p class="ant-upload-text">
            <i18n-t :keypath="$locales.tips_text_upload" />
          </p>
        </a-upload-dragger>
      </a-spin>
    </template>
    <a-button v-if="isImport" v-bind="ButtonProps">
      <template #icon v-if="isPageAction">
        <slot name="icon">
          <CloudUploadOutlined />
        </slot>
      </template>
      <slot>
        <i18n-t :keypath="$locales.action_import" />
      </slot>
    </a-button>
  </a-popconfirm>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics, $System, $locales } from "@/client";
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
  get dateKey() {
    if (this.isRowAction) {
      return this.rowKey;
    }
    return this.lodash.get(
      this.lodash.head(this.Pagination.selectionDataSource),
      this.PageController.key
    );
  }
  loading = false;
  get action() {
    return $System.FilesController.getUploadUrl();
  }
  get headers() {
    return { Authorization: $System.UserController.Authorization };
  }
  get successMsg() {
    return this.$t(this.$locales.tips_success_operation);
  }
  get errorMsg() {
    return this.$t(this.$locales.tips_error_operation);
  }
  onConfirm() {}
  async onChange(event) {
    this.loading = true;
    const { successMsg, errorMsg } = this;
    if (event.file.status === "done") {
      const Id = this.lodash.get(event, "file.response.Id");
      this.loading = false;
      try {
        await this.PageController.onImport(Id);
        this.$message.success(successMsg);
      } catch (error) {
        this.$message.error(errorMsg);
      }
    }
  }
  onGetTemplate() {
    this.PageController.onGetTemplate();
  }
  created() {}
  mounted() {}
}
</script>
<style lang="less">
.x-action-import {
  .ant-popover-message-title {
    padding: 0;
  }
  .ant-upload.ant-upload-drag .ant-upload {
    padding: 16px;
  }
}
</style>
