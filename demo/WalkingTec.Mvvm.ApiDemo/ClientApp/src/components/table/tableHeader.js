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
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:30
 * @modify date 2018-09-12 18:53:30
 * @desc [description]
*/
import { Button, Col, Divider, Form, Row, Select } from 'antd';
import * as React from 'react';
import lodash from 'lodash';
import moment from 'moment';
import { observer } from 'mobx-react';
import { mapValues } from './tableEdit';
var FormItem = Form.Item;
var Option = Select.Option;
/**
 * 搜索标题组件
 *
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
var TableHeaderComponent = /** @class */ (function (_super) {
    __extends(TableHeaderComponent, _super);
    function TableHeaderComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        _this.WrappedFormComponent = Form.create()(FormComponent);
        return _this;
    }
    /**
     * 表单 item
     * @param param0
     */
    TableHeaderComponent.prototype.renderItem = function (params) {
        if (this.props.renderItem) {
            return this.props.renderItem(params);
        }
    };
    TableHeaderComponent.prototype.render = function () {
        return (
        // <Spin spinning={this.Store.pageConfig.loading}>
        React.createElement(Row, null,
            React.createElement(this.WrappedFormComponent, __assign({}, this.props, { renderItem: this.renderItem.bind(this) })))
        // </Spin>
        );
    };
    return TableHeaderComponent;
}(React.Component));
export default TableHeaderComponent;
var FormComponent = /** @class */ (function (_super) {
    __extends(FormComponent, _super);
    function FormComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        _this.state = {
            key: new Date().getTime()
        };
        _this.handleSubmit = function (e) {
            e.preventDefault();
            _this.props.form.validateFields(function (err, values) {
                if (!err) {
                    // 转换时间对象  moment 对象 valueOf 为时间戳，其他类型数据 为原始数据。
                    values = mapValues(values, _this.Store.Format.date);
                    _this.Store.onSearch(values);
                }
            });
        };
        return _this;
    }
    /**
     * 获取 数据类型默认值
     * @param key 属性名称
     * @param type 属性值类型
     */
    FormComponent.prototype.initialValue = function (key, type) {
        var value = this.Store.searchParams[key];
        // console.log(key, value, this.Store.searchParams);
        switch (type) {
            // case 'int32':
            //   return value == null ? 0 : value;
            //   break;
            case 'date-time':
                return this.moment(value);
                break;
            default: //默认字符串
                return value;
                break;
        }
    };
    /**
     * 时间转化
     * @param date
     */
    FormComponent.prototype.moment = function (date) {
        if (date == '' || date == null || date == undefined) {
            return date;
        }
        if (typeof date == 'string') {
            date = moment(date, this.Store.Format.date);
        }
        else {
            date = moment(date);
        }
        return date;
    };
    FormComponent.prototype.renderItem = function () {
        return this.props.renderItem({ form: this.props.form, initialValue: this.initialValue.bind(this) });
    };
    FormComponent.prototype.onReset = function () {
        var _this = this;
        var resetFields = this.props.form.resetFields;
        resetFields();
        this.setState({ key: new Date().getTime() });
        // this.forceUpdate();
        this.props.form.validateFields(function (err, values) {
            if (!err) {
                _this.Store.onSearch(lodash.mapValues(values, function (x) { return undefined; }));
            }
        });
    };
    FormComponent.prototype.render = function () {
        return (React.createElement(Form, { className: "app-table-header-form", onSubmit: this.handleSubmit },
            React.createElement(Row, { type: "flex", gutter: 16, className: "table-header-search", key: this.state.key }, this.renderItem()),
            React.createElement(Row, { type: "flex", gutter: 16, justify: "end" },
                React.createElement(Col, { span: 24, className: "table-header-btn" },
                    React.createElement(Button, { icon: "retweet", onClick: this.onReset.bind(this), loading: this.Store.pageState.loading }, "\u91CD\u7F6E"),
                    React.createElement(Divider, { type: "vertical" }),
                    React.createElement(Button, { icon: "search", htmlType: "submit", loading: this.Store.pageState.loading }, "\u641C\u7D22")))));
    };
    FormComponent = __decorate([
        observer
    ], FormComponent);
    return FormComponent;
}(React.Component));
/**
 * 编辑 装饰器
 * @param Store 状态
 */
export function DecoratorsTableHeader(Store) {
    return function (Component) {
        return /** @class */ (function (_super) {
            __extends(class_1, _super);
            function class_1() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            class_1.prototype.render = function () {
                return React.createElement(TableHeaderComponent, { Store: Store, renderItem: function (params) {
                        return React.createElement(Component, __assign({}, params, { Store: Store }));
                    } });
            };
            return class_1;
        }(React.Component));
    };
}
//# sourceMappingURL=tableHeader.js.map