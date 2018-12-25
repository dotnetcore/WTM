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
import * as React from 'react';
import { Chart, Tooltip, Axis, Bar } from 'viser-react';
var data = [
    { year: '1951 年', sales: 38 },
    { year: '1952 年', sales: 52 },
    { year: '1956 年', sales: 61 },
    { year: '1957 年', sales: 145 },
    { year: '1958 年', sales: 48 },
    { year: '1959 年', sales: 38 },
    { year: '1960 年', sales: 38 },
    { year: '1962 年', sales: 38 },
];
var scale = [{
        dataKey: 'sales',
        tickInterval: 20,
    }];
// https://viserjs.github.io/
// https://viserjs.github.io/demo.html#/bar/basic-column
var IApp = /** @class */ (function (_super) {
    __extends(IApp, _super);
    function IApp() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    IApp.prototype.render = function () {
        return (React.createElement(Chart, { forceFit: true, height: 600, data: data, scale: scale },
            React.createElement(Tooltip, null),
            React.createElement(Axis, null),
            React.createElement(Bar, { position: "year*sales" })));
    };
    return IApp;
}(React.Component));
export default IApp;
//# sourceMappingURL=antv2.js.map