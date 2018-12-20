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
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Divider, Popconfirm } from 'antd';
import React from 'react';
import { DecoratorsTableBody } from 'components/table/tableBody';
import Store from '../store';
/**
 * 组件继承 支持重写,
 */
var BodyComponent = /** @class */ (function (_super) {
    __extends(BodyComponent, _super);
    function BodyComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    BodyComponent.prototype.render = function () {
        return null;
    };
    BodyComponent = __decorate([
        DecoratorsTableBody({
            Store: Store,
            // 列属性配置
            columns: [
                {
                    dataIndex: "dateFormatted",
                    title: "dateFormatted"
                },
                {
                    dataIndex: "summary",
                    title: "summary"
                }, {
                    dataIndex: "temperatureC",
                    title: "temperatureC"
                },
                {
                    dataIndex: "temperatureF",
                    title: "temperatureF"
                },
                {
                    title: 'Action',
                    dataIndex: 'Action',
                    render: function () {
                        return React.createElement("div", null,
                            React.createElement("a", null, "\u4FEE\u6539"),
                            React.createElement(Divider, { type: "vertical" }),
                            React.createElement(Popconfirm, { title: "Sure to delete?" },
                                React.createElement("a", null, "\u5220\u9664")));
                    },
                }
            ]
        })
    ], BodyComponent);
    return BodyComponent;
}(React.Component));
export default BodyComponent;
//# sourceMappingURL=Body.js.map