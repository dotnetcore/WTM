<template>
  <div>
    <a-upload-dragger name="file" action="https://www.mocky.io/v2/5cc8019d300000980a055e76">
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
import lodash from "lodash";
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
  created() {}
  mounted() {}
  onTemplate() {
    this.PageStore.onExport("Template");
  }
  async onImport() {
    const res = await this.PageStore.onImport();
  }
}
</script>
<style scoped lang="less">
</style>
