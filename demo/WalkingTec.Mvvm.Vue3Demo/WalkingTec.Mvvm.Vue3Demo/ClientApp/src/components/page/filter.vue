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
    :layout="layout"
    :rules="rules"
    :model="formState"
    :label-col="labelCol"
    :wrapper-col="wrapperCol"
    @finish="onFinish"
  >
    <a-row type="flex" :gutter="8">
      <slot />
      <a-col
        :offset="offset"
        :span="colProps.colSpan"
        style="text-align: right"
      >
        <a-form-item :wrapper-col="{ span: 22 }">
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
import { fromEvent, Subscription } from "rxjs";
import { debounceTime } from "rxjs/operators";
import { computed } from "vue";
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
    [1057, 3, "horizontal"],
    [Infinity, 4, "horizontal"],
  ],
  default: [
    [513, 1, "vertical"],
    [701, 2, "vertical"],
    [1062, 2, "horizontal"],
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
  @Provide() colProps = {
    colItem: true,
    colSpan: 6,
  };
  /** 表单状态 */
  @Inject() formState = {};
  /** 原始的表单 数据 formState 初始化状态 */
  // originalFormState = {};
  /** 表单 ref */
  @Ref("formRef") formRef;
  /** 表单 rules */
  @Prop({ default: () => [] }) rules;
  labelCol = { xs: 24, sm: 24, md: 6, lg: 6, xl: 6, xxl: 6 };
  wrapperCol = { xs: 24, sm: 24, md: 16, lg: 16, xl: 16, xxl: 16 };
  layout: "vertical" | "horizontal" = "vertical";
  offset = 0;
  ResizeEvent: Subscription;
  get formEl(): HTMLFormElement {
    return this.$el;
  }
  @Emit("finish")
  onFinish(values, query = true) {
    if (query) {
      this.__wtmToQuery(values);
    }
    return values;
  }
  @Emit("reset")
  async onReset() {
    await this.lodash.result(this.formRef, "resetFields");
    let values = await this.lodash.result(this.formRef, "validateFields");
    // 清空 current & pageSize
    this.__wtmToQuery(
      this.lodash.assign({}, values, {
        [this.Pagination.options.currentKey]: "",
        [this.Pagination.options.pageSizeKey]: "",
      })
    );
    // this.onFinish(values);
    return values;
  }
  /** 回填 url 数据 */
  backfillQuery() {
    this.lodash.assign(
      this.formState,
      this.lodash.pick(this.$route.query, this.lodash.keys(this.formState))
    );
  }
  breakPoint() {
    const length = this.$slots
      .default()
      .filter((x) => typeof x.type !== "symbol").length;
    const width = window.innerWidth;
    const breakPoint: [number, number, string] = this.lodash.find<any>(
      BREAKPOINTS.vertical,
      (item) => {
        return width < (item[0] as number) + 16;
      }
    );
    this.colProps.colSpan = 24 / breakPoint[1];
    this.layout = breakPoint[2] as any;
    const offset = (length * this.colProps.colSpan) % 24;
    this.offset = offset + this.colProps.colSpan;
    if (offset === 0) {
      this.offset = this.colProps.colSpan * (breakPoint[1] - 1);
    }
  }
  created() {}
  mounted() {
    this.breakPoint();
    this.backfillQuery();
    this.onFinish(this.lodash.cloneDeep(this.formState), false);
    this.ResizeEvent = fromEvent(window, "resize")
      .pipe(debounceTime(200))
      .subscribe(this.breakPoint);
    console.log(this.$slots.default());
  }
  unmounted() {
    this.ResizeEvent && this.ResizeEvent.unsubscribe();
    this.ResizeEvent = undefined;
  }
}
</script>
<style  lang="less">
</style>
