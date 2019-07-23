import React, { PureComponent } from 'react';
import ReactEcharts from 'echarts-for-react'
import echarts from 'echarts';
var dataAxis = ['点', '击', '柱', '子', '或', '者', '两', '指', '在', '触', '屏', '上', '滑', '动', '能', '够', '自', '动', '缩', '放'];
var data = [220, 182, 191, 234, 290, 330, 310, 123, 442, 321, 90, 149, 210, 122, 133, 334, 198, 123, 125, 220];
var yMax = 500;
var dataShadow = [];
var zoomSize = 6;
/**
 * https://echarts.baidu.com/examples/editor.html?c=bar-gradient
 */
export default class Lunar extends PureComponent {
    getOption = () => {
        for (var i = 0; i < data.length; i++) {
            dataShadow.push(yMax);
        }
        return {
            title: {
                text: '特性示例：渐变色 阴影 点击缩放',
                subtext: 'Feature Sample: Gradient Color, Shadow, Click Zoom'
            },
            xAxis: {
                data: dataAxis,
                axisLabel: {
                    inside: true,
                    textStyle: {
                        color: '#fff'
                    }
                },
                axisTick: {
                    show: false
                },
                axisLine: {
                    show: false
                },
                z: 10
            },
            yAxis: {
                axisLine: {
                    show: false
                },
                axisTick: {
                    show: false
                },
                axisLabel: {
                    textStyle: {
                        color: '#999'
                    }
                }
            },
            dataZoom: [
                {
                    type: 'inside'
                }
            ],
            series: [
                { // For shadow
                    type: 'bar',
                    itemStyle: {
                        normal: { color: 'rgba(0,0,0,0.05)' }
                    },
                    barGap: '-100%',
                    barCategoryGap: '40%',
                    data: dataShadow,
                    animation: false
                },
                {
                    type: 'bar',
                    itemStyle: {
                        normal: {
                            color: new echarts.graphic.LinearGradient(
                                0, 0, 0, 1,
                                [
                                    { offset: 0, color: '#83bff6' },
                                    { offset: 0.5, color: '#188df0' },
                                    { offset: 1, color: '#188df0' }
                                ]
                            )
                        },
                        emphasis: {
                            color: new echarts.graphic.LinearGradient(
                                0, 0, 0, 1,
                                [
                                    { offset: 0, color: '#2378f7' },
                                    { offset: 0.7, color: '#2378f7' },
                                    { offset: 1, color: '#83bff6' }
                                ]
                            )
                        }
                    },
                    data: data
                }
            ]
        };
    };

    render() {
        return (
            <div className='examples'>
                <div className='parent'>
                    <label> render a lunar calendar chart. </label>
                    <ReactEcharts
                        option={this.getOption()}
                        style={{ height: '500px', width: '100%' }}
                        onChartReady={event => {
                            console.log("TCL: Lunar -> render -> event", event)
                            event.on('click', function (params) {
                                console.log(dataAxis[Math.max(params.dataIndex - zoomSize / 2, 0)]);
                                event.dispatchAction({
                                    type: 'dataZoom',
                                    startValue: dataAxis[Math.max(params.dataIndex - zoomSize / 2, 0)],
                                    endValue: dataAxis[Math.min(params.dataIndex + zoomSize / 2, data.length - 1)]
                                });
                            });
                        }}
                    />
                </div>
            </div>
        );
    }
}
