<template>
    <div v-loading="loading" style="height: 100%;width:100%" ref="chartRef"></div>
</template>
<script setup lang="ts">
import { defineAsyncComponent, reactive,markRaw , getCurrentInstance, onMounted, watch, nextTick, onActivated, ref, computed } from 'vue';
import * as echarts from 'echarts';
import request from '/@/utils/request';

const ci = getCurrentInstance() as any;
const chartRef = ref();
const loading = ref(false);
const props = defineProps({
    type: String,
    title: {
        type:String,
        default:''
    },
    namex: {
        type:String,
        default:''
    },
    namey: {
        type:String,
        default:''
    },
    namecategory: {
        type:String,
        default:''
    },
    request: {
        type: Function,
        default: ()=>{},
    },
    para: {
        type: Object,
        default: { },
    },
    nameaddition: {
        type: String,
        default: '',
    },
    opensmooth: {
        type: Boolean,
        default: false,
    },
    showlegent: {
        type: Boolean,
        default: true,
    },
    showtooltip: {
        type: Boolean,
        default: true,
    },
    ishorizontal: {
        type: Boolean,
        default: false,
    }
});
   
    let chart = null as any;
    let repStr = '';
    let replace = {} as any;
    let baseOption = {} as any;
    let requestOption = {} as any;


const doSearch = (api: any = null, para: any = null) => {
    loading.value = true;
    if (!api) {
        api = props.request();
    }
    if (!para) {
        para = props.para;
    }
    request({
        url: api,
        method: 'post',
        data: para,
    }).then((res: any) => {
        let resString = JSON.stringify(res);        
        for (let key in replace) {
            const reg = new RegExp(key, "g");
            resString = resString.replace(reg, replace[key]);
        }
        requestOption = JSON.parse(resString);
        requestOption.dataset = JSON.parse(requestOption.dataset);
        requestOption.series = JSON.parse(requestOption.series);
        requestOption.legend = JSON.parse(requestOption.legend);
        if(requestOption.series){
            for(let i=0;i<requestOption.series.length;i++){
                requestOption.series[i].symbolSize = new Function('return '+requestOption.series[i].symbolSize);
            }
        }
        const op = Object.assign({}, requestOption, baseOption);        
        chart.setOption(op);
        loading.value = false;
        return true;
    })
}
const setBase = () => {
    var tooltip = "";
    if (props.showtooltip == true) {
        tooltip = "tooltip: {},";
        if (props.type == "scatter") {
            tooltip = `tooltip:{
                    formatter: function (params) {
                            var xl = '${props.namex}';
                            var yl = '${props.namey}';
                            var al = '${props.nameaddition}';
                            var cl = '${props.namecategory}';
                            return params.seriesName + ' <br/>'
                                + xl + params.value[0] + ' <br/>'
                                + yl + params.value[1] + ' <br/>'
                                + al + params.value[2] + ' <br/>'
                                + cl + params.value[3] + ' <br/>';
                        },}, `;
        }
        if (props.type == "line") {
            tooltip = "tooltip: {trigger: 'axis'},";
        }
    }
    var legend = "";
    if (props.showlegent == false) {
        legend = `legend: {show: false},`;
    }
    repStr = '';
    if (props.type == 'piehollow')
        repStr = `\\\"type\\\":\\\"pie\\\",\\\"radius\\\": ["40%", "70%"]`;
    else
        repStr = `\\\"type\\\":\\\"${props.type}\\\"`;
    if (props.type == 'line')
        repStr += `,\\\"smooth\\\": ${props.opensmooth}`;
    replace = { '\\\\\\\"type\\\\\\\":\\\\\\\"charttype\\\\\\\"': repStr }

    var xAxis = "";
    var yAxis = "";
    if (props.type != "pie" && props.type != "piehollow") {
        if (props.ishorizontal == false) {
            xAxis = `xAxis: {name:'${props.namex}',type: 'category'},`;
            yAxis = `yAxis: {name:'${props.namey}'},`;
        }
        else {
            xAxis = `xAxis: {name:'${props.namey}'},`;
            yAxis = `yAxis: {name:'${props.namex}',type: 'category'},`;
        }

        if (props.type == "scatter") {
            xAxis = `xAxis: { name:'${props.namex}',type: 'value',splitLine: { lineStyle: { type: 'dashed'} } },`;
            yAxis = `yAxis:{name:'${props.namey}',splitLine:{lineStyle:{type: 'dashed'} },scale: true},`;
        }
    }

    var opTitle = '';
    if (props.title != undefined) {
        opTitle = `title:{text: '${props.title}'},`
    }
    var str = `{
    ${opTitle}
    ${legend}
    ${tooltip}
    ${xAxis}
    ${yAxis}
                }`;
    baseOption = eval("(" + str + ")");
}
onMounted(() => {
    const c = markRaw(echarts.init(chartRef.value));
    chart = c;
    nextTick(() => {
        window.addEventListener('resize', () => { c.resize() });
    });
    setBase();
    doSearch();
})
defineExpose({
	doSearch
});

</script>