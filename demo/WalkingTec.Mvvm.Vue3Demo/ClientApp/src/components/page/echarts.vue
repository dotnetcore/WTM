<template>
  <v-chart class="chart" :option="option" />
</template>
<script lang="ts">
import { PieChart } from "echarts/charts";
import { LegendComponent, TitleComponent, TooltipComponent } from "echarts/components";
import { use } from "echarts/core";
import { CanvasRenderer } from "echarts/renderers";
import VChart, { THEME_KEY } from "vue-echarts";
import { Options, Prop, Provide, Vue } from "vue-property-decorator";
use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent
]);
@Options({ components: { VChart } })
export default class extends Vue {
  @Prop({ required: true }) option;
  @Provide() [THEME_KEY]: "dark";
  created() {}
  mounted() {}
}
</script>
<style lang="less"></style>
