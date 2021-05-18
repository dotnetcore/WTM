<template>
  <!-- <template v-if="_readonly">
    <a-list item-layout="horizontal" :data-source="value">
      <template #renderItem="{ item }">
        <a-list-item>
          <a-list-item-meta :title="item.name"></a-list-item-meta>
        </a-list-item>
      </template>
    </a-list>
  </template>-->
  <!-- <template v-else> -->
  <div class="w-uploader" :class="{ 'w-uploader-readonly': _readonly }">
    <!-- <a-spin :spinning="spinning"> -->
      <!--  -->
      <a-upload
        v-model:fileList="value"
        :disabled="disabled || _readonly"
        :multiple="true"
        :action="action"
        :before-upload="beforeUpload"
        :remove="onRemove"
        @change="onChange"
        v-bind="fieldProps"
      >
        <a-button>
          <upload-outlined />Upload
        </a-button>
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
  @Inject({ default: '' }) readonly formType;
  get action() {
    return $System.FilesController.getUploadUrl()
  }
  get value() {
    return this.lodash.get(this.formState, this._name);
  }
  set value(value) {
    this.lodash.set(this.formState, this._name, value);
  }
  // fileList = [this.value]
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
