<template>
  <a-popconfirm overlayClassName="x-action-import" @confirm="onConfirm">
    <template #icon>
      <div></div>
    </template>
    <template #title>
      <div>
        <i18n-t keypath="tips.text.importExplain" />
        <a-divider type="vertical" />
        <a-button @click="onGetTemplate">
          <template #icon>
            <CloudUploadOutlined />
          </template>
          <i18n-t keypath="action.downloadTemplate" />
        </a-button>
      </div>
      <a-divider style="margin:8px" />
      <a-upload-dragger
        name="file"
        :multiple="true"
        action="https://www.mocky.io/v2/5cc8019d300000980a055e76"
        @change="onChange"
      >
        <p class="ant-upload-drag-icon">
          <inbox-outlined />
        </p>
        <p class="ant-upload-text">
          <i18n-t keypath="tips.text.upload" />
        </p>
      </a-upload-dragger>
    </template>
    <a-button v-if="isImport" v-bind="ButtonProps">
      <template #icon v-if="isPageAction">
        <CloudUploadOutlined />
      </template>
      <i18n-t keypath="action.import" />
    </a-button>
  </a-popconfirm>
</template>
<script lang="ts">
import { Vue, Options, mixins, Prop } from "vue-property-decorator";
import { ControllerBasics } from "@/client";
import { ActionBasics } from "./script";
@Options({ components: {} })
export default class extends mixins(ActionBasics) {
  @Prop() readonly PageController: ControllerBasics;
  onConfirm() {

  }
  onChange() {

  }
  onGetTemplate() {
    this.PageController.onGetTemplate()
  }
  created() { }
  mounted() { }
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
