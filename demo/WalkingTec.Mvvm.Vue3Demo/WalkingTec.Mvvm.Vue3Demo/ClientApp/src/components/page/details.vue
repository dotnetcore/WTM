/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-03-12 17:19:19
 * @modify date 2021-03-12 17:19:19
 * @desc 详情表单 
 */
<template>
  <a-form
    class="w-form"
    ref="formRef"
    layout="vertical"
    :rules="rules"
    :model="formState"
    :label-col="labelCol"
    :wrapper-col="wrapperCol"
    @finish="onSubmit"
  >
    <a-spin :spinning="spinning">
      <slot />
    </a-spin>
    <a-space class="w-form-space" align="center">
      <a-spin :spinning="spinning">
        <template v-slot:indicator>
          <div></div>
        </template>
        <a-button type="primary" html-type="submit">
          <template v-slot:icon>
            <SaveOutlined />
          </template>
          <i18n-t keypath="action.submit" />
        </a-button>
        <a-button @click.stop.prevent="onReset">
          <template v-slot:icon>
            <RedoOutlined />
          </template>
          <i18n-t keypath="action.reset" />
        </a-button>
      </a-spin>
    </a-space>
  </a-form>
</template>

<script lang="ts">
import { Options, Prop, Vue, Inject, Ref, Emit } from "vue-property-decorator";
@Options({
  components: {},
})
export default class extends Vue {
  /** 表单状态 */
  @Inject() formState = {};
  /** 表单 ref */
  @Ref("formRef") formRef;
  /** 表单 rules */
  @Prop({ default: () => [] }) rules;
  @Prop({ type: Function, required: true }) onFinish;
  spinning = false;
  labelCol = { span: 24 };
  wrapperCol = { span: 24 };
  // @Emit("finish")
  // onFinish(values) {
  //   this.spinning = true;
  //   this.onSubmit(values)
  //   return {
  //     // 表单值
  //     values,
  //     // 成功回调
  //     onComplete: this.onComplete,
  //     // 失败回调
  //     onFail: this.onFail,
  //   };
  // }
  async onSubmit(values) {
    try {
      this.spinning = true;
      await this.onFinish(values);
      this.onComplete();
    } catch (error) {
      this.onFail(error);
    }
  }
  async onReset() {
    await this.lodash.result(this.formRef, "resetFields");
    // const values = await this.lodash.result(this.formRef, "validateFields");
  }
  // 成功
  onComplete() {
    this.spinning = false;
    // this.__wtmBackDetails();
  }
  // 失败
  onFail(error) {
    console.log("LENG ~ extends ~ onFail ~ error", error);
    this.spinning = false;
  }
  // 加载数据
  onLoading() {
    this.spinning = true;
    this.lodash.delay(() => {
      this.spinning = false;
    }, 2000);
  }
  created() {}
  mounted() {
    this.onLoading();
    console.log("LENG ~ extends ~ mounted ~ this", this);
  }
}
</script>
<style lang="less" scoped>
.w-form-space {
  width: 100%;
  justify-content: center;
}
</style>
<style lang="less">
.ant-drawer-body {
  .w-form {
    padding-bottom: 55px;
  }
  .w-form-space {
    position: absolute;
    right: 0;
    bottom: 0;
    padding: 10px 16px;
    border-top: 1px solid #e9e9e9;
    background: white;
    z-index: 1;
  }
}
</style>