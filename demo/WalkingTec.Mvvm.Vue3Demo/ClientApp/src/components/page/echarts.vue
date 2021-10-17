<template>
  <div class="app-chart-spin">
    <a-spin :spinning="spinning">
      <v-chart class="chart" :option="_option" />
    </a-spin>
  </div>
</template>
<script lang="ts">
import { PieChart, LineChart, BarChart, ScatterChart } from "echarts/charts";
import {
  LegendComponent,
  TitleComponent,
  TooltipComponent,
  ToolboxComponent
} from "echarts/components";
import { use } from "echarts/core";
import { CanvasRenderer } from "echarts/renderers";
import VChart, { THEME_KEY } from "vue-echarts";
import { Options, Prop, Provide, Vue } from "vue-property-decorator";
// 文档地址 https://echarts.apache.org/handbook/zh/basics/import
use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  ToolboxComponent,
  LegendComponent,
  LineChart,
  BarChart,
  ScatterChart
]);
@Options({ components: { VChart } })
export default class extends Vue {
  // 图表参数
  @Prop({ type: Object }) option;
  // 需要替换的 key  {charttype:'pie'}
  @Prop({ type: Object }) replace;
  //   api 请求参数 string | AjaxRequest
  @Prop({}) request;
  @Provide() [THEME_KEY]: "dark";
  get _option() {
    return this.lodash.merge({}, this.requestOption, this.option);
  }
  spinning = true;
  requestOption = {};
  async onRequest() {
    if (this.request) {
      try {
        const res = await this.$Ajax.request<any>(this.request).toPromise();
        const option = this.lodash.mapValues<any, any>(res, val => {
          if (this.lodash.isString(val)) {
            // 替换所有配置的数据
            this.lodash.mapValues(this.replace, (regStr, key) => {
              const reg = new RegExp(key, "g");
              val = val.replace(reg, regStr);
            });
            return JSON.parse(val);
          }
        });
        this.requestOption = option;
      } catch (error) {
        console.log(
          "🚀 ~ file: echarts.vue ~ line 60 ~ extends ~ onRequest ~ error",
          error
        );
      }
    }
    this.spinning = false;
  }
  created() {
    this.onRequest();
  }
  mounted() {}
}
</script>
<style lang="less">
.app-chart-spin {
  width: 100%;
  height: 100%;
  .ant-spin-nested-loading,
  .ant-spin-container {
    width: 100%;
    height: 100%;
  }
}
</style>
