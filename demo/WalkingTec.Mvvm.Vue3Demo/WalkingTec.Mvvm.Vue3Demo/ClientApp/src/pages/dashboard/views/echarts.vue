<template>
  <div class="charts-wrap">
    <div class="charts-draw" ref="chart"></div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
const echarts = require("echarts/lib/echarts");
// 引入柱状图
require("echarts/lib/chart/pie");
// 引入提示框和标题组件
require("echarts/lib/component/tooltip");
require("echarts/lib/component/legend");

@Component({
    name: "echarts"
})
export default class extends Vue {
    mounted() {
        // 基于准备好的dom，初始化echarts实例
        const myChart = echarts.init(this.$refs.chart);
        const option = {
            title: {
                text: "南丁格尔玫瑰图",
                subtext: "纯属虚构",
                left: "center"
            },
            tooltip: {
                trigger: "item",
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            legend: {
                left: "center",
                top: "bottom",
                data: [
                    "rose1",
                    "rose2",
                    "rose3",
                    "rose4",
                    "rose5",
                    "rose6",
                    "rose7",
                    "rose8"
                ]
            },
            toolbox: {
                show: true,
                feature: {
                    mark: { show: true },
                    dataView: { show: true, readOnly: false },
                    magicType: {
                        show: true,
                        type: ["pie", "funnel"]
                    },
                    restore: { show: true },
                    saveAsImage: { show: true }
                }
            },
            series: [
                {
                    name: "半径模式",
                    type: "pie",
                    radius: [20, 110],
                    center: ["25%", "50%"],
                    roseType: "radius",
                    label: {
                        show: false
                    },
                    emphasis: {
                        label: {
                            show: true
                        }
                    },
                    data: [
                        { value: 10, name: "rose1" },
                        { value: 5, name: "rose2" },
                        { value: 15, name: "rose3" },
                        { value: 25, name: "rose4" },
                        { value: 20, name: "rose5" },
                        { value: 35, name: "rose6" },
                        { value: 30, name: "rose7" },
                        { value: 40, name: "rose8" }
                    ]
                },
                {
                    name: "面积模式",
                    type: "pie",
                    radius: [30, 110],
                    center: ["75%", "50%"],
                    roseType: "area",
                    data: [
                        { value: 10, name: "rose1" },
                        { value: 5, name: "rose2" },
                        { value: 15, name: "rose3" },
                        { value: 25, name: "rose4" },
                        { value: 20, name: "rose5" },
                        { value: 35, name: "rose6" },
                        { value: 30, name: "rose7" },
                        { value: 40, name: "rose8" }
                    ]
                }
            ]
        };
        // 绘制图表
        myChart.setOption(option);
    }
}
</script>

<style lang="less" scoped>
.charts-wrap {
    .charts-draw {
        width: 100%;
        min-height: 400px;
    }
}
</style>
