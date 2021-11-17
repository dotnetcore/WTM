<template>
    <div class="app-chart-spin">
        <a-spin :spinning="spinning">
            <v-chart class="chart" :option="_option" />
        </a-spin>
    </div>
</template>
<script lang="ts">
import "echarts";
import VChart, { THEME_KEY } from "vue-echarts";
import { Options, Prop, Provide, Vue } from "vue-property-decorator";
// 文档地址 https://echarts.apache.org/handbook/zh/basics/import
    @Options({ components: { VChart } })
    export default class extends Vue {
        // 图表参数
        @Prop({ type: Object }) option;
        // 需要替换的 key  {charttype:'pie'}
        @Prop({ type: Object }) replace;
        @Prop({ type: String }) type;
        @Prop({ type: String }) title;
        @Prop({ type: String, default: 'X' }) namex;
        @Prop({ type: String, default: 'Y' }) namey;

        @Prop({ type: Boolean, default: false }) opensmooth;
        @Prop({ type: Boolean, default: true }) showlegent;
        @Prop({ type: Boolean, default: true }) showtooltip;
        @Prop({ type: Boolean, default: false }) ishorizontal;
        //   api 请求参数 string | AjaxRequest
        @Prop({}) request;
        @Provide() [THEME_KEY]: "dark";
        get _option() {
            return this.lodash.merge(
                {},
                this.requestOption,
                this.option,
                this.baseOption
            );
        }
        spinning = true;
        requestOption = {};
        baseOption = {};
        async onRequest() {
            //this.replace = "{ charttype: '" + this.type + "' }";
            if (this.request) {
                try {
                    const res = await this.$Ajax.post<any>(this.request);
                    const option = this.lodash.mapValues<any, any>(res, val => {
                        if (this.lodash.isString(val)) {
                            // 替换所有配置的数据
                            this.lodash.mapValues(this.replace, (regStr, key) => {
                                const reg = new RegExp(key, '"type":"charttype"');
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

        }
        refresh(){
            console.log("xxxxxxxxxxxxxxxxxxxxx"); 
        }
        setBase() {
            var tooltip = "";
            if (this.showtooltip == true) {
                tooltip = "tooltip: {},";
                if (this.type == "scatter") {
                    tooltip = `tooltip:{
                    formatter: function (params) {
                            var xl = '${this.namex}';
                            var yl = '${this.namey}';
                            var al = '附加';
                            var cl = '';
                            return params.seriesName + ' <br/>'
                                + xl + params.value[0] + ' <br/>'
                                + yl + params.value[1] + ' <br/>'
                                + al + params.value[2] + ' <br/>'
                                + cl + params.value[3] + ' <br/>';
                        },}, `;
                }
                if (this.type == "line") {
                    tooltip = "tooltip: {trigger: 'axis'},";
                }
            }
            var legend = "";
            if (this.showlegent == false) {
                legend = `legend: {show: false},`;
            }
            //if (this.type == 'piehollow')
            //    this.replace = `"type":"pie","radius": ["40%", "70%"]`;
            //else
            //    this.replace = `"type":"${this.type}"`;
            //if (this.type == 'line')
            //    this.replace += `,"smooth": ${this.opensmooth}`;

            var xAxis = "";
            var yAxis = "";
            if (this.type != "pie" && this.type != "piehollow") {
                if (this.ishorizontal == false) {
                    xAxis = `xAxis: {name:'${this.namex}',type: 'category'},`;
                    yAxis = `yAxis: {name:'${this.namey}'},`;
                }
                else {
                    xAxis = `xAxis: {name:'${this.namey}'},`;
                    yAxis = `yAxis: {name:'${this.namex}',type: 'category'},`;
                }

                if (this.type == "scatter") {
                    xAxis = `xAxis: {{ name:'${this.namex}',type: 'value',splitLine: {{ lineStyle: {{ type: 'dashed'}} }} }},`;
                    yAxis = `yAxis:{{name:'${this.namey}',splitLine:{{lineStyle:{{type: 'dashed'}} }},scale: true}},`;
                }
            }

            var opTitle = '';
            if (this.title != undefined) {
                opTitle = `title:{text: '${this.title}'},`
            }
            var str = `{
    ${opTitle}
    ${legend}
    ${tooltip}
    ${xAxis}
    ${yAxis}
                }`;
            this.baseOption = eval("(" + str + ")");
        }

        created() {
            this.setBase();
            this.onRequest();

            this.spinning = false;
        }
        mounted() { }
    }
</script>
<style lang="less">
    .app-chart-spin { width: 100%; height: 100%; .ant-spin-nested-loading, .ant-spin-container
    { width: 100%; height: 100%; }
    }
</style>
