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
import { Button, Drawer, Form, Input, Divider } from 'antd';
import decoForm from 'components/decorators/form';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import { toJS } from 'mobx';
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
var default_1 = /** @class */ (function (_super) {
    __extends(default_1, _super);
    function default_1() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    default_1.prototype.render = function () {
        return React.createElement(Drawer, { title: "\u7F16\u8F91", className: "app-drawer", width: 500, placement: "right", closable: false, onClose: function () { Store.onPageState("visibleEdit", false); }, visible: Store.pageState.visibleEdit, destroyOnClose: true }, Store.pageState.isUpdate ? React.createElement(UpdateForm, null) : React.createElement(InsertForm, null));
    };
    default_1 = __decorate([
        observer
    ], default_1);
    return default_1;
}(React.Component));
export default default_1;
/**
 * 添加表单
 */
var InsertForm = /** @class */ (function (_super) {
    __extends(InsertForm, _super);
    function InsertForm() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    InsertForm.prototype.onSubmit = function (e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields(function (err, values) {
            if (!err) {
                // values = mapValues(values, "YYYY-MM-DD")
                Store.onEdit(values);
            }
        });
    };
    InsertForm.prototype.render = function () {
        var _a = this.props, form = _a.form, initialValue = _a.initialValue;
        var getFieldDecorator = form.getFieldDecorator;
        return React.createElement(Form, { onSubmit: this.onSubmit.bind(this) },
            React.createElement("div", { className: "app-drawer-formItem" },
                React.createElement(FormItem, __assign({ label: "\u8D26\u53F7" }, formItemLayout), getFieldDecorator('ITCode', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 ITCode" }))),
                React.createElement(FormItem, __assign({ label: "\u5BC6\u7801" }, formItemLayout), getFieldDecorator('Password', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 Password" }))),
                React.createElement(FormItem, __assign({ label: "\u90AE\u7BB1" }, formItemLayout), getFieldDecorator('Email', {})(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 Email" }))),
                React.createElement(FormItem, __assign({ label: "\u59D3\u540D" }, formItemLayout), getFieldDecorator('Name', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 Name" })))),
            React.createElement("div", { className: "app-drawer-btns" },
                React.createElement(Button, { onClick: function () { return Store.onPageState("visibleEdit", false); } }, "\u53D6\u6D88 "),
                React.createElement(Divider, { type: "vertical" }),
                React.createElement(Button, { loading: Store.pageState.loadingEdit, type: "primary", htmlType: "submit" }, "\u63D0\u4EA4 ")));
    };
    InsertForm = __decorate([
        decoForm,
        observer
    ], InsertForm);
    return InsertForm;
}(React.Component));
/**
 * 修改表单
 */
var UpdateForm = /** @class */ (function (_super) {
    __extends(UpdateForm, _super);
    function UpdateForm() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    UpdateForm.prototype.onSubmit = function (e) {
        e.stopPropagation();
        e.preventDefault();
        this.props.form.validateFields(function (err, values) {
            if (!err) {
                // values = mapValues(values, "YYYY-MM-DD")
                Store.onEdit(values);
            }
        });
    };
    UpdateForm.prototype.render = function () {
        var _a = this.props, form = _a.form, initialValue = _a.initialValue;
        var getFieldDecorator = form.getFieldDecorator;
        var details = toJS(Store.details);
        return React.createElement(Form, { onSubmit: this.onSubmit.bind(this) },
            React.createElement("div", { className: "app-drawer-formItem" },
                React.createElement(FormItem, __assign({ label: "\u8D26\u53F7" }, formItemLayout), getFieldDecorator('itCode', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                    initialValue: details['itCode']
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 ITCode" }))),
                React.createElement(FormItem, __assign({ label: "\u5BC6\u7801" }, formItemLayout), getFieldDecorator('password', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                    initialValue: details['password']
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 Password" }))),
                React.createElement(FormItem, __assign({ label: "\u90AE\u7BB1" }, formItemLayout), getFieldDecorator('email', {
                    initialValue: details['email']
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 Email" }))),
                React.createElement(FormItem, __assign({ label: "\u59D3\u540D" }, formItemLayout), getFieldDecorator('name', {
                    rules: [{ "required": true, "message": "Please input your undefined!" }],
                    initialValue: details['name']
                })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165 Name" })))),
            React.createElement("div", { className: "app-drawer-btns" },
                React.createElement(Button, { onClick: function () { return Store.onPageState("visibleEdit", false); } }, "\u53D6\u6D88 "),
                React.createElement(Divider, { type: "vertical" }),
                React.createElement(Button, { loading: Store.pageState.loadingEdit, type: "primary", htmlType: "submit" }, "\u63D0\u4EA4 ")));
    };
    UpdateForm = __decorate([
        decoForm,
        observer
    ], UpdateForm);
    return UpdateForm;
}(React.Component));
//# sourceMappingURL=edit.js.map