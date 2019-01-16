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
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spread = (this && this.__spread) || function () {
    for (var ar = [], i = 0; i < arguments.length; i++) ar = ar.concat(__read(arguments[i]));
    return ar;
};
import { Button, Col, Divider, Form, Row, Icon } from 'antd';
import * as React from 'react';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import { observable, action } from 'mobx';
import decoForm from 'components/decorators/form';
import "./style.less";
/**
 * 装饰器
 * @param params
 */
export function DecoratorsSearch(params) {
    return function (Component) {
        return /** @class */ (function (_super) {
            __extends(class_1, _super);
            function class_1() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            class_1.prototype.render = function () {
                return (React.createElement(SearchComponent, __assign({}, params, this.props),
                    React.createElement(Component, __assign({ Store: params.Store }, this.props))));
            };
            return class_1;
        }(React.Component));
    };
}
/**
 * 搜索标题组件
 *
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
var SearchComponent = /** @class */ (function (_super) {
    __extends(SearchComponent, _super);
    function SearchComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        _this.toggle = false;
        _this.FormItems = _this.props.FormItems(_this.props.form);
        return _this;
    }
    SearchComponent.prototype.onSubmit = function (e) {
        var _this = this;
        e.preventDefault();
        this.props.form.validateFields(function (err, values) {
            if (!err) {
                // 转换时间对象  moment 对象 valueOf 为时间戳，其他类型数据 为原始数据。
                // values = mapValues(values, this.Store.Format.date)
                console.log(values);
                _this.Store.onSearch(values);
            }
        });
    };
    SearchComponent.prototype.onReset = function () {
        var _this = this;
        var resetFields = this.props.form.resetFields;
        resetFields();
        this.props.form.validateFields(function (err, values) {
            if (!err) {
                _this.Store.onSearch(lodash.mapValues(values, function (x) { return undefined; }));
            }
        });
    };
    SearchComponent.prototype.onToggle = function () {
        this.toggle = !this.toggle;
    };
    SearchComponent.prototype.render = function () {
        var items = null;
        var FormItems = this.props.FormItems(this.props.form);
        if (Array.isArray(FormItems)) {
            if (this.toggle) {
                items = FormItems;
            }
            else {
                items = __spread(FormItems).splice(0, 4);
            }
        }
        var toggleShow = FormItems.length >= 4;
        return (React.createElement(Form, { className: "data-view-search", onSubmit: this.onSubmit.bind(this) },
            React.createElement(Row, { type: "flex", gutter: 16 }, items),
            React.createElement(Row, { type: "flex", gutter: 16, justify: "end" },
                React.createElement(Col, { span: 16, className: "data-view-search-left" }, this.props.children),
                React.createElement(Col, { span: 8, className: "data-view-search-right" },
                    React.createElement(Button, { icon: "retweet", onClick: this.onReset.bind(this), loading: this.Store.pageState.loading }, "\u91CD\u7F6E"),
                    React.createElement(Divider, { type: "vertical" }),
                    React.createElement(Button, { icon: "search", htmlType: "submit", loading: this.Store.pageState.loading }, "\u641C\u7D22"),
                    React.createElement(Divider, { type: "vertical" }),
                    toggleShow ? React.createElement("a", { onClick: this.onToggle }, this.toggle ? React.createElement(React.Fragment, null,
                        "\u6536\u8D77 ",
                        React.createElement(Icon, { type: 'down' })) : React.createElement(React.Fragment, null,
                        "\u5C55\u5F00 ",
                        React.createElement(Icon, { type: 'up' }))) : null))));
    };
    __decorate([
        observable
    ], SearchComponent.prototype, "toggle", void 0);
    __decorate([
        action.bound
    ], SearchComponent.prototype, "onToggle", null);
    SearchComponent = __decorate([
        decoForm,
        observer
    ], SearchComponent);
    return SearchComponent;
}(React.Component));
export default SearchComponent;
//# sourceMappingURL=search.js.map