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
import { Form, Input } from 'antd';
import * as React from 'react';
import { DecoratorsTableEdit } from 'components/table/tableEdit';
import Store from '../store';
var FormItem = Form.Item;
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
var EditComponent = /** @class */ (function (_super) {
    __extends(EditComponent, _super);
    function EditComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    EditComponent.prototype.render = function () {
        var _a = this.props, form = _a.form, initialValue = _a.initialValue;
        var getFieldDecorator = form.getFieldDecorator;
        /**
         *  label 显示描述
         *
         *  rules 验证条件 空着就行。
         *
         *              属性key   类型 '' 就行
         *  initialValue('id', 'int64'),
         *
         *
         */
        return React.createElement(React.Fragment, null,
            React.createElement(FormItem, __assign({ label: "id" }, formItemLayout), getFieldDecorator('id', {
                rules: [{ "required": true, "message": "Please input your undefined!" }],
                initialValue: initialValue('id', 'int64'),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u540D\u79F0" }, formItemLayout), getFieldDecorator('name', {
                rules: [{ "required": true, "message": "Please input your undefined!" }, { "min": 0, "message": "min length 0!" }, { "max": 50, "message": "max length 50!" }],
                initialValue: initialValue('name', ''),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u5730\u5740" }, formItemLayout), getFieldDecorator('address', {
                rules: [{ "required": true, "message": "Please input your undefined!" }, { "min": 0, "message": "min length 0!" }, { "max": 500, "message": "max length 500!" }],
                initialValue: initialValue('address', ''),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u5907\u6CE8" }, formItemLayout), getFieldDecorator('remark', {
                rules: [{ "min": 0, "message": "min length 0!" }, { "max": 500, "message": "max length 500!" }],
                initialValue: initialValue('remark', ''),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u521B\u5EFA\u65E5\u671F" }, formItemLayout), getFieldDecorator('createdate', {
                rules: [{ "required": true, "message": "Please input your undefined!" }],
                initialValue: initialValue('createdate', 'date-time'),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u521B\u5EFA\u4EBA" }, formItemLayout), getFieldDecorator('createby', {
                rules: [{ "required": true, "message": "Please input your undefined!" }, { "min": 0, "message": "min length 0!" }, { "max": 50, "message": "max length 50!" }],
                initialValue: initialValue('createby', ''),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u4FEE\u6539\u65E5\u671F" }, formItemLayout), getFieldDecorator('updatedate', {
                rules: [],
                initialValue: initialValue('updatedate', 'date-time'),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))),
            React.createElement(FormItem, __assign({ label: "\u4FEE\u6539\u4EBA" }, formItemLayout), getFieldDecorator('updateby', {
                rules: [{ "min": 0, "message": "min length 0!" }, { "max": 50, "message": "max length 50!" }],
                initialValue: initialValue('updateby', ''),
            })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))));
    };
    EditComponent = __decorate([
        DecoratorsTableEdit(Store)
    ], EditComponent);
    return EditComponent;
}(React.Component));
export default EditComponent;
//# sourceMappingURL=Edit.js.map