<template>
  <div class="w-uploader" :class="{ 'w-uploader-readonly': _readonly }">
    <a-upload
      v-model:fileList="value"
      :disabled="disabled || _readonly"
      :multiple="true"
      :action="action"
      :headers="headers"
      :before-upload="beforeUpload"
      :remove="onRemove"
      @change="onChange"
      v-bind="_fieldProps"
    >
      <a-button>
        <upload-outlined />Upload
      </a-button>
    </a-upload>
  </div>
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
  @Inject({ default: '' }) readonly formType;
  get action() {
    return $System.FilesController.getUploadUrl()
  }
  get headers() {
    return { Authorization: $System.UserController.Authorization }
  }
  get value() {
    return this.lodash.get(this.formState, this._name);
  }
  set value(value) {
    this.lodash.set(this.formState, this._name, value);
  }
  async mounted() {
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
  beforeUpload() { }
  onChange(event) {
    // if (event.file.status === 'uploading') {
    //   this.spinning = true
    // } else {
    //   this.spinning = false
    // }
  }
  onRemove(file) {
    const response = this.lodash.get(file, 'response')
    return response ? $System.FilesController.deleteFiles(response) : true
  }
}

</script>
<style lang="less">
.w-uploader {
  &-readonly {
    .ant-upload {
      display: none;
    }
  }
  .ant-upload-list-item-info {
    padding-right: 60px;
  }
}
</style>
