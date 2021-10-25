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
  @Prop({ type: Object }) type;
  @Prop({ type: Object }) title;
  @Prop({ type: Boolean }) showtooltip;
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
  setBase() {
    var tooltip = "";
    if (this.showtooltip == true) {
      tooltip = "tooltip: {},";
      if (this.type == "scatter") {
        tooltip = `tooltip:{
                    formatter: function (params) {
                            var xl = 'x';
                            var yl = 'y';
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
        {
          tooltip = "tooltip: {trigger: 'axis'},";
        }
      }
      var xAxis = "",
        yAxis = "";
      if (this.type != "pie" && this.type != "piehollow") {
        xAxis = `xAxis: {name:'Y'},`;
        yAxis = `yAxis: {name:'X',type: 'category'},`;
        if (this.type == "scatter") {
          xAxis = `xAxis: {{ name:'X',type: 'value',splitLine: {{ lineStyle: {{ type: 'dashed'}} }} }},`;
          yAxis = `yAxis:{{name:'Y',splitLine:{{lineStyle:{{type: 'dashed'}} }},scale: true}},`;
        }
      }

      var str = `{
    ${tooltip}
    ${xAxis}
    ${yAxis}
                }`;
      this.baseOption = eval("(" + str + ")");
    }
  }
  created() {
    this.onRequest();
    this.setBase();
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
