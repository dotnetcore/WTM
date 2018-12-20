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
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Col, Form, Input } from 'antd';
import * as React from 'react';
import { DecoratorsTableHeader } from 'components/table/tableHeader';
import Store from '../store';
var FormItem = Form.Item;
var colLayout = {
    xl: 6,
    lg: 8,
    md: 12
};
var formItemLayout = {
    labelCol: {
        xs: { span: 24 },
        sm: { span: 6 },
    },
    wrapperCol: {
        xs: { span: 24 },
        sm: { span: 16 },
    },
};
/**
 * 组件继承 支持重写,
 */
var HeaderComponent = /** @class */ (function (_super) {
    __extends(HeaderComponent, _super);
    function HeaderComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    HeaderComponent.prototype.render = function () {
        var _a = this.props, form = _a.form, initialValue = _a.initialValue;
        var getFieldDecorator = form.getFieldDecorator;
        return React.createElement(React.Fragment, null,
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u672A\u914D\u7F6E\u8BF4\u660E" }, formItemLayout), getFieldDecorator('createdateFrom', {
                    initialValue: initialValue('createdateFrom', 'date-time'),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u672A\u914D\u7F6E\u8BF4\u660E" }, formItemLayout), getFieldDecorator('createdateTo', {
                    initialValue: initialValue('createdateTo', 'date-time'),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u672A\u914D\u7F6E\u8BF4\u660E" }, formItemLayout), getFieldDecorator('updatedateFrom', {
                    initialValue: initialValue('updatedateFrom', 'date-time'),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u672A\u914D\u7F6E\u8BF4\u660E" }, formItemLayout), getFieldDecorator('updatedateTo', {
                    initialValue: initialValue('updatedateTo', 'date-time'),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u540D\u79F0" }, formItemLayout), getFieldDecorator('name', {
                    initialValue: initialValue('name', ''),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u5730\u5740" }, formItemLayout), getFieldDecorator('address', {
                    initialValue: initialValue('address', ''),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u5907\u6CE8" }, formItemLayout), getFieldDecorator('remark', {
                    initialValue: initialValue('remark', ''),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u521B\u5EFA\u65E5\u671F" }, formItemLayout), getFieldDecorator('createdate', {
                    initialValue: initialValue('createdate', 'date-time'),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u521B\u5EFA\u4EBA" }, formItemLayout), getFieldDecorator('createby', {
                    initialValue: initialValue('createby', ''),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u4FEE\u6539\u65E5\u671F" }, formItemLayout), getFieldDecorator('updatedate', {
                    initialValue: initialValue('updatedate', 'date-time'),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
            React.createElement(Col, __assign({}, colLayout),
                React.createElement(FormItem, __assign({ label: "\u4FEE\u6539\u4EBA" }, formItemLayout), getFieldDecorator('updateby', {
                    initialValue: initialValue('updateby', ''),
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))));
    };
    HeaderComponent = __decorate([
        DecoratorsTableHeader(Store)
    ], HeaderComponent);
    return HeaderComponent;
}(React.Component));
export default HeaderComponent;
//# sourceMappingURL=Header.js.map