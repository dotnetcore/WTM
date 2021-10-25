/** * @author 冷 (https://github.com/LengYXin) * @email
lengyingxin8966@gmail.com * @create date 2021-03-12 17:19:19 * @modify date
2021-03-12 17:19:19 * @desc 详情表单 */
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
    @validate="onValidate"
  >
    <a-spin :spinning="spinning || loading">
      <div class="w-form-items">
        <slot />
      </div>
    </a-spin>
    <a-space class="w-form-space" align="center">
      <a-spin :spinning="spinning || loading">
        <template v-slot:indicator>
          <div></div>
        </template>
        <slot name="button">
          <!-- 非只读状态显示底部按钮 -->
          <template v-if="!readonly">
            <a-button type="primary" html-type="submit">
              <template v-slot:icon>
                <SaveOutlined />
              </template>
              <i18n-t :keypath="$locales.action_submit" />
            </a-button>
            <a-divider type="vertical" />
            <a-button @click.stop.prevent="onReset">
              <template v-slot:icon>
                <RedoOutlined />
              </template>
              <i18n-t :keypath="$locales.action_reset" />
            </a-button>
          </template>
        </slot>
      </a-spin>
    </a-space>
  </a-form>
</template>

<script lang="ts">
import {
  Inject,
  Options,
  Prop,
  Ref,
  Vue,
  Provide,
  Watch
} from "vue-property-decorator";
@Options({
  components: {}
})
export default class extends Vue {
  /** 表单状态 */
  @Inject() readonly formState = {};
  /** 自定义 校验状态 用于服务器返回 错误*/
  @Provide({ reactive: true }) formValidate = {};
  // 只读
  @Provide({ reactive: true }) formType = "details";
  /** 表单 ref */
  @Ref("formRef") readonly formRef;
  /** 表单 rules */
  @Prop({ default: () => [] }) readonly rules;
  /** 加载中 */
  @Prop({ default: false }) readonly loading;
  @Prop() readonly queryKey: string;
  /** 数据 提交 函数 */
  @Prop({ type: Function, required: true }) readonly onFinish;
  // 只读
  get readonly() {
    return this.lodash.has(this.$route.query, "_readonly");
  }
  get batch() {
    return this.lodash.has(this.$route.query, "_batch");
  }
  get successMsg() {
    return this.$t(this.$locales.tips_success_operation);
  }
  get errorMsg() {
    return this.$t(this.$locales.tips_error_operation);
  }
  spinning = false;
  labelCol = { span: 24 };
  wrapperCol = { span: 24 };
  async onSubmit(values) {
    try {
      this.spinning = true;
      await this.onFinish(this.lodash.cloneDeep(values));
      this.onComplete();
    } catch (error) {
      this.onFail(error);
    }
  }
  async onReset() {
    await this.lodash.result(this.formRef, "resetFields");
    this.formValidate = {};
    // const values = await this.lodash.result(this.formRef, "validateFields");
  }
  onValidate(name) {
    console.log("LENG ~ extends ~ onValidate ~ name", name);
  }
  // 成功
  onComplete() {
    this.spinning = false;
    this.__wtmBackDetails(this.queryKey);
    this.$message.success(this.successMsg);
  }
  // 失败
  onFail(error) {
    if (this.lodash.isString(error)) {
      this.$message.warn(error);
    } else {
      const formErrors = this.lodash.get(error, "response.Form");
      this.formValidate = this.lodash.mapValues(formErrors, (msg, key) => {
        return {
          help: msg,
          validateStatus: "error"
        };
      });
      console.error("LENG  ~ onFail ", this.formRef, formErrors, error);
    }

    this.spinning = false;
    // this.$message.error(this.errorMsg)
  }
  created() {}
  mounted() {
    // this.onLoading();
    // console.log("LENG ~ extends ~ mounted ~ this", this);
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
.ant-modal-body,
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
.w-form-items {
  .ant-space {
    width: 100%;
    @media screen and (max-width: 785px) {
      display: block;
    }
  }

  .ant-space-item {
    flex: 1;
  }
}
</style>
