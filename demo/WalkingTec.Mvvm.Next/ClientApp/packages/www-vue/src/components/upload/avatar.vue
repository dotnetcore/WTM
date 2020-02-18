<template>
  <a-upload
    accept="image/*"
    listType="picture-card"
    class="avatar-uploader"
    :disabled="disabled"
    :showUploadList="false"
    :action="fileService.fileUpload.src"
    :beforeUpload="beforeUpload"
    @change="handleChange"
  >
    <img v-if="imageUrl" width="120" height="120" :src="imageUrl" alt="avatar" />
    <div v-else-if="!disabled">
      <a-icon :type="loading ? 'loading' : 'plus'" />
      <div class="ant-upload-text">Upload</div>
    </div>
  </a-upload>
</template>
<script lang="ts">
function getBase64(img, callback) {
  const reader = new FileReader();
  reader.addEventListener("load", () => callback(reader.result));
  reader.readAsDataURL(img);
}

import { Component, Prop, Vue } from "vue-property-decorator";
import lodash from "lodash";
@Component({ components: {} })
export default class avatar extends Vue {
  @Prop() private value: any;
  @Prop() private disabled: any;
  loading = false;
  base64Str = null;
  get imageUrl() {
    if (this.value) {
      return `${this.fileService.fileGet.src}/${this.value}`;
    }
    return this.base64Str;
  }
  fileService = this.$GlobalConfig.onCreateFileService();

  handleChange(info) {
    if (info.file.status === "uploading") {
      this.loading = true;
      return;
    }
    if (info.file.status === "done") {
      // Get this url from response in real world.
      getBase64(info.file.originFileObj, imageUrl => {
        this.base64Str = imageUrl;
        this.loading = false;
      });
      const Id = lodash.get(info, "file.response.Id");
      this.triggerChange(Id);
    }
  }
  beforeUpload(file) {
    //   const isJPG = file.type === "image/jpeg";
    //   if (!isJPG) {
    //     this.$message.error("You can only upload JPG file!");
    //   }
    const isLt2M = file.size / 1024 / 1024 < 2;
    if (!isLt2M) {
      this.$message.error("Image must smaller than 2MB!");
    }
    return isLt2M;
  }
  triggerChange(changedValue) {
    // Should provide an event to pass value to Form.
    this.$emit("change", changedValue);
  }
  mounted() {
    // console.log(this);
  }
}
</script>
<style>
.avatar-uploader > .ant-upload {
  width: 128px;
  height: 128px;
}
.ant-upload-select-picture-card i {
  font-size: 32px;
  color: #999;
}

.ant-upload-select-picture-card .ant-upload-text {
  margin-top: 8px;
  color: #666;
}
</style>