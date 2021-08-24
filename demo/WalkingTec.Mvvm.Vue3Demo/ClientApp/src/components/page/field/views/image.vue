<template>
  <!-- <template v-if="_readonly">
    <a-image :width="100" :src="imageUrl" :fallback="imagefallback" />
  </template>
  <template v-else> -->
  <div class="w-avatar-uploader">
    <!-- <a-spin :spinning="spinning"> -->
    <!-- v-model:fileList="fileList" -->
    <a-upload
      :disabled="disabled || _readonly"
      :fileList="fileList"
      name="file"
      list-type="picture-card"
      accept="image/*"
      :action="action"
      :before-upload="beforeUpload"
      @change="onChange"
      v-bind="_fieldProps"
    >
      <!-- <img v-if="imageUrl" :src="imageUrl" class="w-upload-img" /> -->
      <!-- <a-image
            v-if="imageUrl"
            :width="104"
            class="w-upload-img"
            :src="imageUrl"
            :fallback="imagefallback"
          /> -->
      <div v-if="isPlusBottun">
        <!-- <loading-outlined v-if="loading"></loading-outlined> -->
        <plus-outlined></plus-outlined>
        <div class="ant-upload-text">Upload</div>
      </div>
    </a-upload>
    <!-- </a-spin> -->
  </div>
  <!-- </template> -->
</template>
<script lang="ts">
import { Vue, Options, Watch, mixins, Inject } from "vue-property-decorator";
import { $System } from "@/client";
import { FieldBasics } from "../script";
@Options({ components: {} })
export default class extends mixins(FieldBasics) {
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  // 表单类型
  @Inject({ default: "" }) readonly formType;
  imageUrl = "";
  get action() {
    return $System.FilesController.getUploadUrl();
  }
  get max() {
    return this.lodash.get(this._fieldProps, "max", 1);
  }
  get value() {
    return this.lodash.get(this.formState, this._name);
  }
  set value(value) {
    this.lodash.set(this.formState, this._name, value);
  }
  get isPlusBottun() {
    return (
      !(this.disabled || this._readonly) && this.fileList.length < this.max
    );
  }
  fileList = [];
  async mounted() {
    // this.onRequest();
    // if (this.value) {
    //   this.imageUrl = $System.FilesController.getDownloadUrl(this.value)
    // }
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
  beforeUpload() {}
  onChange(event) {
    this.fileList = event.fileList;
    if (event.file.status === "uploading") {
      this.spinning = true;
    } else {
      this.spinning = false;
      if (event.file.status === "done") {
        const Id = this.lodash.get(event, "file.response.Id");
        this.value = Id;
      }
    }
  }
  @Watch("value")
  onValueChange(val, old) {
    if (val) {
      this.imageUrl = $System.FilesController.getDownloadUrl(val);
    }
  }
}
function getBase64(img: Blob, callback: (base64Url: string) => void) {
  const reader = new FileReader();
  reader.addEventListener("load", () => callback(reader.result as string));
  reader.readAsDataURL(img);
}
</script>
<style lang="less">
.w-avatar-uploader {
  min-height: 120px;
  // .ant-spin-nested-loading,
  // .ant-spin-container,
  // .ant-upload-picture-card-wrapper {
  //   display: inline-block;
  //   width: auto;
  // }
  .ant-upload.ant-upload-select-picture-card {
    margin: 0;
  }
}
.w-upload-img {
  width: 104px;
  height: 104px;
}
</style>
