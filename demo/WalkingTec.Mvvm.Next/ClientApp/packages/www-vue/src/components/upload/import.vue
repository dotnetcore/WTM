<template>
  <div>
    <a-upload-dragger
      name="file"
      :showUploadList="false"
      :action="fileService.fileUpload.src"
      accept=".xlsx,.xls"
      @change="handleChange"
    >
      <p class="ant-upload-drag-icon">
        <a-icon type="inbox" />
      </p>
      <p class="ant-upload-text" v-t="'tips.text.upload'"></p>
      <!-- <p class="ant-upload-hint">
        Support for a single or bulk upload. Strictly prohibit from uploading company data or other
        band files
      </p>-->
    </a-upload-dragger>
    <a-button type="link" icon="cloud-download" @click="onTemplate">
      <span v-t="'action.downloadTemplate'" />
    </a-button>
  </div>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { EntitiesPageStore } from "@leng/public/src";
import { ICellRendererParams } from "ag-grid-community";
import globalConfig from "../../global.config";
import lodash from "lodash";
import { toJS } from "mobx";
// import { i18n } from "../../locale";
/**
 *  上传 组件
 */
@Component({})
export default class ImportContent extends Vue {
  @Prop() private PageStore: EntitiesPageStore;
  @Prop() private i18n: any;
  get _i18n() {
    return this.i18n;
  }
  fileService = globalConfig.onCreateFileService();
  success = false;
  created() {}
  mounted() {}
  destroyed() {
    if (this.success) {
      this.PageStore.EventSubject.next({
        EventType: "onSearch",
        AjaxRequest: {
          body: lodash.merge({}, toJS(this.PageStore.SearchParams), {
            Page: 1
          })
        }
      });
    }
  }
  handleChange(info) {
    if (info.file.status === "uploading") {
      return;
    }
    if (info.file.status === "done") {
      const Id = lodash.get(info, "file.response.Id");
      this.triggerChange(Id);
    }
  }
  async triggerChange(UploadFileId) {
    const res = await this.PageStore.onImport({ body: { UploadFileId } });
    if (res) {
      this.success = true;
      this.$notification.success({
        message: this.$t("tips.success.operation") as any,
        description: null
      });
    } else {
      this.$notification.error({
        message: this.$t("tips.error.operation") as any,
        description: null
      });
    }
  }
  onTemplate() {
    this.PageStore.onExport("Template");
  }
}
</script>
<style scoped lang="less">
</style>
