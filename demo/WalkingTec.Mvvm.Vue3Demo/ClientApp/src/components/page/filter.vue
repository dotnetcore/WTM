/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-03-12 17:20:02
 * @modify date 2021-03-12 17:20:02
 * @desc 搜索条件 表单
 */
<template>
  <!-- <a-collapse class="w-filter-collapse" v-model:activeKey="activeKey" @change="onCollapseChange"> -->
  <!-- <a-collapse-panel key="search" :header="$t($locales.action_search)"> -->
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
      <slot name="more" v-if="moreOpen" />
      <a-col :offset="offset" :span="colProps.colSpan" style="text-align: right">
        <a-form-item :wrapper-col="{ span: 22 }">
          <a-space align="center">
            <!-- 搜索按钮 -->
            <a-button type="primary" html-type="submit">
              <template v-slot:icon>
                <SearchOutlined />
              </template>
              <i18n-t :keypath="$locales.action_submit" />
            </a-button>
            <!-- 重置按钮 -->
            <a-button @click.stop.prevent="onReset">
              <template v-slot:icon>
                <RedoOutlined />
              </template>
              <i18n-t :keypath="$locales.action_reset" />
            </a-button>
            <!-- 展开按钮 -->
            <a-button @click.stop.prevent="onMoreOpen" type="link" v-show="moreShow">
              <template v-slot:icon>
                <UpOutlined v-if="moreOpen" />
                <DownOutlined v-else />
              </template>
              <i18n-t :keypath="moreOpen ? $locales.action_retract : $locales.action_open" />
            </a-button>
          </a-space>
        </a-form-item>
      </a-col>
    </a-row>
  </a-form>
  <!-- </a-collapse-panel> -->
  <!-- </a-collapse> -->
</template>

<script lang="ts">
import { ControllerBasics } from "@/client";
import lodash from "lodash";
import { fromEvent, Subscription } from "rxjs";
import { debounceTime } from "rxjs/operators";
import {
  Emit, Inject, Options,
  Prop,


  Provide, Ref, Vue
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
    [1057, 2, "horizontal"],
    [1352, 3, "horizontal"],
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
  @Prop({ required: true }) readonly PageController: ControllerBasics;
  get Pagination() {
    return this.PageController.Pagination;
  }
  /** 表单状态 */
  @Inject() readonly formState = {};
  /** 表单 ref */
  @Ref("formRef") readonly formRef;

  /** 表单 rules */
  @Prop({ default: () => [] }) readonly rules;

  /** 告诉 field 使用 a-col */
  @Provide({ reactive: true }) colProps = {
    colItem: true,
    colSpan: 6,
  };
  labelCol = { xs: 24, sm: 24, md: 7, lg: 7, xl: 7, xxl: 7 };
  wrapperCol = { xs: 24, sm: 24, md: 15, lg: 15, xl: 15, xxl: 15 };
  layout: "vertical" | "horizontal" = "vertical";
  offset = 0;
  // activeKey = 'search'
  // 更多条件
  moreOpen = false
  ResizeEvent: Subscription;
  // 显示 展开更多按钮
  get moreShow() {
    if (lodash.isFunction(this.$slots.more)) {
      return this.$slots.more().filter((x) => typeof x.type !== "symbol").length
    }
    return false
  }
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
  onMoreOpen() {
    this.moreOpen = !this.moreOpen
    this.breakPoint()
    this.onCollapseChange()
  }
  onCollapseChange = lodash.debounce(() => {
    dispatchEvent(new CustomEvent('resize'));
  }, 500)
  /** 回填 url 数据 */
  backfillQuery() {
    this.lodash.assign(
      this.formState,
      this.lodash.pick(this.$route.query, this.lodash.keys(this.formState))
    );
  }
  breakPoint() {
    let length = 0;
    // 默认搜索条件 x.type !== "symbol" 排除空格
    if (this.lodash.isFunction(this.$slots.default)) {
      length = this.$slots.default().filter((x) => typeof x.type !== "symbol").length
    }
    // 隐藏的搜索条件
    if (this.moreOpen && this.lodash.isFunction(this.$slots.more)) {
      length += this.$slots.more().filter((x) => typeof x.type !== "symbol").length
    }

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
    switch (true) {
      // case offset === 0:
      //   this.offset = this.colProps.colSpan * (breakPoint[1] - 1);
      //   break;
      case this.colProps.colSpan + offset === 24:
        this.offset = 0;
        break;
      case this.colProps.colSpan + offset < 24:
        this.offset = 24 - (this.colProps.colSpan + offset);
        break;
      default:
        break;
    }
  }
  created() { }
  mounted() {
    this.breakPoint();
    this.backfillQuery();
    this.onFinish(this.lodash.cloneDeep(this.formState), false);
    this.ResizeEvent = fromEvent(window, "resize")
      .pipe(debounceTime(200))
      .subscribe(this.breakPoint);
  }
  unmounted() {
    this.ResizeEvent && this.ResizeEvent.unsubscribe();
    this.ResizeEvent = undefined;
  }
}
</script>
<style  lang="less">
.w-filter-collapse {
  .ant-collapse-content-box {
    @media screen and (min-width: 785px) {
      padding: 16px 8px 0 8px !important;
    }
  }
  margin-bottom: 8px !important;
}
</style>
