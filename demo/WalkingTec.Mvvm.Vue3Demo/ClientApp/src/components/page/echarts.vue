<template>
  <v-chart class="chart" :option="_option" />
</template>
<script lang="ts">
import { PieChart, LineChart, BarChart, ScatterChart } from "echarts/charts";
import {
  LegendComponent,
  TitleComponent,
  TooltipComponent
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
  LegendComponent,
  LineChart,
  BarChart,
  ScatterChart
]);
@Options({ components: { VChart } })
export default class extends Vue {
  @Prop({ type: Object }) option;
  //   api 请求参数 string | AjaxRequest
  @Prop({}) request;
  @Provide() [THEME_KEY]: "dark";
  get _option() {
    return this.lodash.merge({}, this.requestOption, this.option);
  }
  requestOption = {};
  async onRequest() {
    if (this.request) {
      const res = await this.$Ajax.request<any>(this.request).toPromise();
      const option = this.lodash.mapValues(res, JSON.parse);
      this.requestOption = option;
    }
  }
  created() {
    this.onRequest();
  }
  mounted() {}
}
</script>
<style lang="less"></style>
