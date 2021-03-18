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
    @finish="onFinish"
  >
    <slot />
    <a-space class="w-form-space" align="center">
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
  labelCol = { span: 24 };
  wrapperCol = { span: 24 };
  @Emit("finish")
  onFinish(values) {
    return values;
  }
  async onReset() {
    await this.lodash.result(this.formRef, "resetFields");
    // const values = await this.lodash.result(this.formRef, "validateFields");
  }
  created() {}
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