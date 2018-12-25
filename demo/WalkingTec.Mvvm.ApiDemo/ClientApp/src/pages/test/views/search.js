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
import { DecoratorsSearch } from 'components/dataView/header/search';
import * as React from 'react';
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
var default_1 = /** @class */ (function (_super) {
    __extends(default_1, _super);
    function default_1() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    default_1.prototype.shouldComponentUpdate = function () {
        return false;
    };
    default_1.prototype.render = function () {
        return this.props.children;
    };
    default_1 = __decorate([
        DecoratorsSearch({
            Store: Store,
            FormItems: function (_a) {
                var getFieldDecorator = _a.getFieldDecorator;
                return [
                    React.createElement(Col, __assign({}, colLayout, { key: "test" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test', {
                            initialValue: Store.searchParams['test'],
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test1" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test1', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test2" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test2', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test3" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test3', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test4" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test4', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test5" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test5', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test6" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test6', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test7" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test7', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" })))),
                    React.createElement(Col, __assign({}, colLayout, { key: "test8" }),
                        React.createElement(FormItem, __assign({ label: "\u6D4B\u8BD5" }, formItemLayout), getFieldDecorator('test8', {
                            initialValue: '',
                        })(React.createElement(Input, { placeholder: "\u8BF7\u8F93\u5165" }))))
                ];
            }
        })
    ], default_1);
    return default_1;
}(React.Component));
export default default_1;
//# sourceMappingURL=search.js.map