<template>
    <!--<a-row>
        <a-col :span="12">
            <div style="height: 400px;width: 600px;">
                <WtmEcharts :option="optionPie" />
            </div>
        </a-col>
        <a-col :span="12">
            <div style="height: 400px;width: 600px;">
                <WtmEcharts :option="optionPieB" />
            </div>
        </a-col>
    </a-row>
    <a-row>
        <a-col :span="12">
            <div style="height: 400px;width: 600px;">
                <WtmEcharts :option="optionLine" />
            </div>
        </a-col>
        <a-col :span="12">
            <div style="height: 400px;width: 600px;">
                <WtmEcharts :option="optionBar" />
            </div>
        </a-col>
    </a-row>-->
    <a-row>
        <a-col :span="12">
            <div style="height: 400px;width: 600px;">
                <WtmEcharts :option="optionScatter" />
            </div>
        </a-col>
        <a-col :span="12">
            <WtmEcharts request="/api/School/GetCharts" type="line"
                        :showtooltip="true"
                        :title="柱状图" />
        </a-col>
    </a-row>
</template>
<script lang="ts">
    import { ColDef, ColGroupDef, GridOptions } from "ag-grid-community";
    import { Inject, Options, Vue } from "vue-property-decorator";
    import { PageController } from "../controller";
    import { EnumLocaleLabel } from "../locales";
    import RowAction from "./action.vue";
    @Options({ components: {} })
    export default class extends Vue {
        optionPie = {
            title: {
                text: "Traffic Sources",
                left: "center"
            },
            tooltip: {
                trigger: "item",
                formatter: "{a} <br/>{b} : {c} ({d}%)"
            },
            legend: {
                orient: "vertical",
                left: "left",
                data: ["Direct", "Email", "Ad Networks", "Video Ads", "Search Engines"]
            },
            series: [
                {
                    name: "Traffic Sources",
                    type: "pie",
                    radius: "55%",
                    center: ["50%", "60%"],
                    data: [
                        { value: 335, name: "Direct" },
                        { value: 310, name: "Email" },
                        { value: 234, name: "Ad Networks" },
                        { value: 135, name: "Video Ads" },
                        { value: 1548, name: "Search Engines" }
                    ],
                    emphasis: {
                        itemStyle: {
                            shadowBlur: 10,
                            shadowOffsetX: 0,
                            shadowColor: "rgba(0, 0, 0, 0.5)"
                        }
                    }
                }
            ]
        };
        optionPieB = {
            tooltip: {
                trigger: "item"
            },
            legend: {
                top: "5%",
                left: "center"
            },
            series: [
                {
                    name: "Access From",
                    type: "pie",
                    radius: ["40%", "70%"],
                    avoidLabelOverlap: false,
                    itemStyle: {
                        borderRadius: 10,
                        borderColor: "#fff",
                        borderWidth: 2
                    },
                    label: {
                        show: false,
                        position: "center"
                    },
                    emphasis: {
                        label: {
                            show: true,
                            fontSize: "40",
                            fontWeight: "bold"
                        }
                    },
                    labelLine: {
                        show: false
                    },
                    data: [
                        { value: 1048, name: "Search Engine" },
                        { value: 735, name: "Direct" },
                        { value: 580, name: "Email" },
                        { value: 484, name: "Union Ads" },
                        { value: 300, name: "Video Ads" }
                    ]
                }
            ]
        };

        optionLine = {
            title: {
                text: "Stacked Line"
            },
            tooltip: {
                trigger: "axis"
            },
            legend: {
                data: ["Email", "Union Ads", "Video Ads", "Direct", "Search Engine"]
            },
            grid: {
                left: "3%",
                right: "4%",
                bottom: "3%",
                containLabel: true
            },
            toolbox: {
                feature: {
                    saveAsImage: {}
                }
            },
            xAxis: {
                type: "category",
                boundaryGap: false,
                data: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
            },
            yAxis: {
                type: "value"
            },
            series: [
                {
                    name: "Email",
                    type: "line",
                    stack: "Total",
                    data: [120, 132, 101, 134, 90, 230, 210]
                },
                {
                    name: "Union Ads",
                    type: "line",
                    stack: "Total",
                    data: [220, 182, 191, 234, 290, 330, 310]
                },
                {
                    name: "Video Ads",
                    type: "line",
                    stack: "Total",
                    data: [150, 232, 201, 154, 190, 330, 410]
                },
                {
                    name: "Direct",
                    type: "line",
                    stack: "Total",
                    data: [320, 332, 301, 334, 390, 330, 320]
                },
                {
                    name: "Search Engine",
                    type: "line",
                    stack: "Total",
                    data: [820, 932, 901, 934, 1290, 1330, 1320]
                }
            ]
        };
        optionBar = {
            xAxis: {
                type: "category",
                data: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
            },
            yAxis: {
                type: "value"
            },
            series: [
                {
                    data: [120, 200, 150, 80, 70, 110, 130],
                    type: "bar"
                }
            ]
        };
        optionScatter = {
            xAxis: {},
            yAxis: {},
            series: [
                {
                    symbolSize: 20,
                    data: [
                        [10.0, 8.04],
                        [8.07, 6.95],
                        [13.0, 7.58],
                        [9.05, 8.81],
                        [11.0, 8.33],
                        [14.0, 7.66],
                        [13.4, 6.81],
                        [10.0, 6.33],
                        [14.0, 8.96],
                        [12.5, 6.82],
                        [9.15, 7.2],
                        [11.5, 7.2],
                        [3.03, 4.23],
                        [12.2, 7.83],
                        [2.02, 4.47],
                        [1.05, 3.33],
                        [4.05, 4.96],
                        [6.03, 7.24],
                        [12.0, 6.26],
                        [12.0, 8.84],
                        [7.08, 5.82],
                        [5.02, 5.68]
                    ],
                    type: "scatter"
                }
            ]
        };
        created() { }
    }
</script>
<style lang="less"></style>
