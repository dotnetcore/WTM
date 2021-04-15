<template>
  <template v-if="_readonly">
    <a-image
      :width="100"
      src="https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png"
    />
  </template>
  <template v-else>
    <a-upload
      v-model:file-list="value"
      name="avatar"
      list-type="picture-card"
      class="avatar-uploader"
      :show-upload-list="false"
      action="https://www.mocky.io/v2/5cc8019d300000980a055e76"
      :before-upload="beforeUpload"
      @change="handleChange"
    >
      <!-- <img v-if="imageUrl" :src="imageUrl" alt="avatar" /> -->
      <div>
        <!-- <loading-outlined v-if="loading"></loading-outlined> -->
        <plus-outlined ></plus-outlined>
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
  async mounted() {
    // this.onRequest();
    if (this.debug) {
      console.log("");
      console.group(`Field ~ ${this.entityKey} ${this._name} `);
      console.log(this);
      console.groupEnd();
    }
  }
  beforeUpload(){}
  handleChange(){}
}
</script>
<style lang="less">
</style>
