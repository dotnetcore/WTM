<template>
  <template v-if="_readonly">
    <a-image
      :width="100"
      src="https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png"
    />
  </template>
  <template v-else>
    <a-upload
      :disabled="disabled"
      v-model:file-list="value"
      name="avatar"
      list-type="picture-card"
      class="avatar-uploader"
      :show-upload-list="false"
      action="https://www.mocky.io/v2/5cc8019d300000980a055e76"
      :before-upload="beforeUpload"
      @change="onChange"
    >
      <img v-if="imageUrl" :src="imageUrl" alt="avatar" style="" />
      <div v-else>
        <!-- <loading-outlined v-if="loading"></loading-outlined> -->
        <plus-outlined></plus-outlined>
        <div class="ant-upload-text">Upload</div>
      </div>
    </a-upload>
  </template>
</template>
<script lang="ts">
import { Vue, Options, Prop, mixins, Inject } from "vue-property-decorator";
import { FieldBasics } from "../script";
@Options({ components: {} })
export default class extends mixins(FieldBasics) {
  // 表单状态值
  @Inject() readonly formState;
  // 自定义校验状态
  @Inject() readonly formValidate;
  // 实体
  @Inject() readonly PageEntity;
  imageUrl = "";
  get value() {
    return this.lodash.get(this.formState, this._name, []);
  }
  set value(value) {
    this.lodash.set(this.formState, this._name, value);
  }
  async mounted() {
    // this.onRequest();
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
  beforeUpload() {}
  onChange(event) {
    this.value = event.fileList;
    if (event.file.status === "done") {
      // Get this url from response in real world.
      getBase64(event.file.originFileObj, (base64Url: string) => {
        this.imageUrl = base64Url;
        console.log(
          "🚀 ~ file: image.vue ~ line 62 ~ extends ~ getBase64 ~ this.imageUrl",
          this.imageUrl
        );
      });
    }
    console.log(
      "🚀 ~ file: image.vue ~ line 57 ~ extends ~ onChange ~ this.value",
      this.value
    );
  }
}
function getBase64(img: Blob, callback: (base64Url: string) => void) {
  const reader = new FileReader();
  reader.addEventListener("load", () => callback(reader.result as string));
  reader.readAsDataURL(img);
}
</script>
<style lang="less"></style>
