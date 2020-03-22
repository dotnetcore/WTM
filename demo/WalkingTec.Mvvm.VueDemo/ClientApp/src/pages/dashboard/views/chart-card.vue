<template>
  <el-row class="chart-card">
    <el-col :span="24" class="card-header" :class="[backgroundColor]">
      <div :id="chartId" class="ct-chart"></div>
    </el-col>
    <el-col :span="24" class="card-content">
      <slot name="content"></slot>
    </el-col>
    <el-col :span="24" class="card-actions">
      <slot name="footer"></slot>
    </el-col>
  </el-row>
</template>

<script lang="ts">
import { Component, Vue, Prop, Watch } from "vue-property-decorator";

@Component({
  name: "ChartCard"
})
export default class extends Vue {
  @Prop({ default: "Line" })
  chartType: String;

  @Prop({ default: () => {} })
  chartOptions: Object;

  @Prop({
    default: () => ({
      labels: [],
      series: []
    })
  })
  chartData: Object;

  @Prop({ default: "" })
  backgroundColor: string;

  chartId: string = "no-id";

  initChart(Chartist: any) {
    var chartIdQuery = `#${this.chartId}`;
    Chartist[this.chartType](chartIdQuery, this.chartData, this.chartOptions);
  }

  updateChartId() {
    var currentTime = new Date().getTime().toString();
    var randomInt = this.getRandomInt(0, currentTime);
    this.chartId = `chart_${randomInt}`;
  }

  getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }

  mounted() {
    this.updateChartId();
    import("chartist").then(Chartist => {
      let ChartistLib = Chartist.default || Chartist;
      this.$nextTick(() => {
        this.initChart(ChartistLib);
      });
    });
  }
}
</script>
<style lang="less">
.ct-grid {
  stroke: rgba(255, 255, 255, 0.2);
}
.ct-series-a .ct-point,
.ct-series-a .ct-line,
.ct-series-a .ct-bar,
.ct-series-a .ct-slice-donut {
  stroke: rgba(255, 255, 255, 0.8);
}
.ct-series-a .ct-slice-pie,
.ct-series-a .ct-area {
  fill: rgba(255, 255, 255, 0.4);
}

.ct-label {
  color: rgba(255, 255, 255, 0.7);
}
</style>

<style lang="less" scoped>
.chart-card {
  background-color: #fff;
  display: inline-block;
  position: relative;
  width: 100%;
  margin: 25px 0;
  overflow: unset;
  box-shadow: 0 1px 4px 0 rgba(0, 0, 0, 0.14);
  border-radius: 3px;
  color: rgba(0, 0, 0, 0.87);
  .card-header {
    padding: 0;
    min-height: 160px;
    margin: -20px 15px 0;
    width: calc(100% - 30px);
    border-radius: 3px;
    box-shadow: 0 1px 4px 0 rgba(0, 0, 0, 0.14);
    background-color: #999999;
    .ct-chart {
    }
  }
  .card-content {
    padding: 15px 20px;
  }
  .card-actions {
    margin: 0 20px 10px;
    width: calc(100% - 40px);
    padding: 10px 0 0 0;
    border-top: 1px solid #eee;
  }

  .purple {
    background: linear-gradient(60deg, #ab47bc, #8e24aa);
  }
  .blue {
    background: linear-gradient(60deg, #26c6da, #00acc1);
  }
  .green {
    background: linear-gradient(60deg, #66bb6a, #43a047);
  }
  .orange {
    background: linear-gradient(60deg, #ffa726, #fb8c00);
  }
  .red {
    background: linear-gradient(60deg, #ef5350, #e53935);
  }
}
</style>
