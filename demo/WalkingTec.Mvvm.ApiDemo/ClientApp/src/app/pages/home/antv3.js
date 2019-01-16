var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
import { Chart, Tooltip, Axis, Legend, Coord, Pie } from 'viser-react';
import * as React from 'react';
var DataSet = require('@antv/data-set');
var sourceData = [
    { item: '事例一', count: 40 },
    { item: '事例二', count: 21 },
    { item: '事例三', count: 17 },
    { item: '事例四', count: 13 },
    { item: '事例五', count: 9 }
];
var scale = [{
        dataKey: 'percent',
        min: 0,
        formatter: '.0%',
    }];
var dv = new DataSet.View().source(sourceData);
dv.transform({
    type: 'percent',
    field: 'count',
    dimension: 'item',
    as: 'percent'
});
var data = dv.rows;
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    App.prototype.render = function () {
        return (React.createElement(Chart, { forceFit: true, height: 600, data: data, scale: scale },
            React.createElement(Tooltip, { showTitle: false }),
            React.createElement(Axis, null),
            React.createElement(Legend, { dataKey: "item" }),
            React.createElement(Coord, { type: "theta", radius: 0.75, innerRadius: 0.6 }),
            React.createElement(Pie, { position: "percent", color: "item", style: { stroke: '#fff', lineWidth: 1 }, label: ['percent', {
                        formatter: function (val, item) {
                            return item.point.item + ': ' + val;
                        }
                    }] })));
    };
    return App;
}(React.Component));
export default App;
//# sourceMappingURL=antv3.js.map