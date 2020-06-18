<template>
  <wtm-dialog-box :is-show="isShow" title="导入" width="40%" @close="onClose">
    <div>
      <span>导入说明：请下载模版，然后在把信息输入到模版中</span>
      <el-divider direction="vertical" />
      <el-button type="primary" icon="el-icon-download" size="small" @click="onDownload">
        下载模版
      </el-button>
    </div>
    <el-divider />
    <el-upload class="upload-box" drag :action="uploadApi" :show-file-list="false" :on-success="handleAvatarSuccess" :on-error="onError" multiple>
      <i class="el-icon-upload" />
      <div class="el-upload__text">
        将文件拖到此处，或<em>点击上传</em>
      </div>
    </el-upload>
  </wtm-dialog-box>
</template>
<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import { upload } from "@/service/modules/upload";

@Component
export default class Upload extends Vue {
    @Prop({ type: Boolean, default: false })
    isShow;
    uploadApi:string = upload.url;
    onClose() {
        this.$emit("update:isShow", false);
    }
    handleAvatarSuccess(res) {
        if (res.Id) {
            this.$emit("onImport", res);
            this.onClose();
        } else {
            this["$message"].error("上传失败!");
        }
    }
    onDownload() {
        this.$emit("onDownload");
    }
    /**
     * 导出错误
     */
    onError(err) {
        this["$message"].error(`上传失败,${err}`);
    }
}
</script>
<style lang="less" >
.upload-box {
    width: 100%;
    .el-upload {
        width: 100%;
    }
    .el-upload-dragger {
        width: 100%;
    }
}
</style>
