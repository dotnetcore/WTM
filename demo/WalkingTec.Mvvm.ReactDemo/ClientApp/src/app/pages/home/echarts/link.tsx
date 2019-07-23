import React, { PureComponent } from 'react';
import ReactEcharts from 'echarts-for-react'
/**
 * https://echarts.baidu.com/examples/editor.html?c=pie-nest&theme=light
 */
export default class Lunar extends PureComponent {
    getOption = () => {
        return {
            legend: {},
            tooltip: {
                trigger: 'axis',
                showContent: false
            },
            dataset: {
                source: [
                    ['product', '2012', '2013', '2014', '2015', '2016', '2017'],
                    ['Matcha Latte', 41.1, 30.4, 65.1, 53.3, 83.8, 98.7],
                    ['Milk Tea', 86.5, 92.1, 85.7, 83.1, 73.4, 55.1],
                    ['Cheese Cocoa', 24.1, 67.2, 79.5, 86.4, 65.2, 82.5],
                    ['Walnut Brownie', 55.2, 67.1, 69.2, 72.4, 53.9, 39.1]
                ]
            },
            xAxis: { type: 'category' },
            yAxis: { gridIndex: 0 },
            grid: { top: '55%' },
            series: [
                { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                { type: 'line', smooth: true, seriesLayoutBy: 'row' },
                {
                    type: 'pie',
                    id: 'pie',
                    radius: '30%',
                    center: ['50%', '25%'],
                    label: {
                        formatter: '{b}: {@2012} ({d}%)'
                    },
                    encode: {
                        itemName: 'product',
                        value: '2012',
                        tooltip: '2012'
                    }
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
                        theme="light"
                        option={this.getOption()}
                        style={{ height: '800px', width: '100%' }}
                        onChartReady={event => {
                            console.log("TCL: Lunar -> render -> event", event)
                            event.on('updateAxisPointer', function (event) {
                                var xAxisInfo = event.axesInfo[0];
                                if (xAxisInfo) {
                                    var dimension = xAxisInfo.value + 1;
                                    // event.setOption({
                                    //     series: {
                                    //         id: 'pie',
                                    //         label: {
                                    //             formatter: '{b}: {@[' + dimension + ']} ({d}%)'
                                    //         },
                                    //         encode: {
                                    //             value: dimension,
                                    //             tooltip: dimension
                                    //         }
                                    //     }
                                    // });
                                }
                            });
                        }}
                    />
                </div>
            </div>
        );
    }
}
