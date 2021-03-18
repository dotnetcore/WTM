/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-03-12 17:20:02
 * @modify date 2021-03-12 17:20:02
 * @desc 搜索条件 表单
 */
<template>
  <a-form
    ref="formRef"
    :rules="rules"
    :model="formState"
    :label-col="labelCol"
    :wrapper-col="wrapperCol"
    @finish="onFinish"
  >
    <a-row type="flex">
      <slot v-bind="bind" />
      <a-col :span="span">
        <a-form-item :wrapper-col="{ span: 24 }">
          <a-space align="center">
            <a-button type="primary" html-type="submit">
              <template v-slot:icon>
                <SearchOutlined />
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
        </a-form-item>
      </a-col>
    </a-row>
  </a-form>
</template>

<script lang="ts">
import { ControllerBasics } from "@/client";
import {
  Options,
  Prop,
  Vue,
  Ref,
  Provide,
  Inject,
  Emit,
} from "vue-property-decorator";

const CONFIG_SPAN_BREAKPOINTS = {
  xs: 513,
  sm: 513,
  md: 785,
  lg: 992,
  xl: 1057,
  xxl: Infinity,
};
/** 配置表单列变化的容器宽度断点 */
const BREAKPOINTS = {
  vertical: [
    // [breakpoint, cols, layout]
    [513, 1, "vertical"],
    [785, 2, "vertical"],
    [1057, 3, "vertical"],
    [Infinity, 4, "vertical"],
  ],
  default: [
    [513, 1, "vertical"],
    [701, 2, "vertical"],
    [1062, 3, "horizontal"],
    [1352, 3, "horizontal"],
    [Infinity, 4, "horizontal"],
  ],
};
@Options({
  components: {},
})
export default class extends Vue {
  @Prop({ required: true }) PageController: ControllerBasics;
  get Pagination() {
    return this.PageController.Pagination;
  }
  /** 告诉 field 使用 a-col */
  @Provide() colItem = true;
  /** 表单状态 */
  @Inject() formState = {};
  /** 原始的表单 数据 formState 初始化状态 */
  // originalFormState = {};
  /** 表单 ref */
  @Ref("formRef") formRef;
  /** 表单 rules */
  @Prop({ default: () => [] }) rules;
  labelCol = { span: 6 };
  wrapperCol = { span: 14 };
  span = 6;
  bind = { span: 6 };
  @Emit("finish")
  onFinish(values, replace = true) {
    this.__wtmToQuery(values);
    return values;
  }
  async onReset() {
    await this.lodash.result(this.formRef, "resetFields");
    let values = await this.lodash.result(this.formRef, "validateFields");
    this.Pagination.onReset();
    // 清空 current & pageSize
    values = this.lodash.assign({}, values, {
      [this.Pagination.options.currentKey]: "",
      [this.Pagination.options.pageSizeKey]: "",
    });

    this.onFinish(values);
  }
  /** 回填 url 数据 */
  backfillQuery() {
    this.lodash.assign(
      this.formState,
      this.lodash.pick(this.$route.query, this.lodash.keys(this.formState))
    );
  }
  created() {}
  mounted() {
    this.backfillQuery();
    this.onFinish(this.lodash.cloneDeep(this.formState), false);
  }
}
</script>
<style  lang="less">
</style>
