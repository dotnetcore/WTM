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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:26
 * @modify date 2018-09-12 18:53:26
 * @desc [description]
*/
import { Alert, Button, Divider, Drawer, Form, Icon, Modal, Popconfirm, Row, Select, Spin, Tabs, Upload } from 'antd';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import moment from 'moment';
import * as React from 'react';
var FormItem = Form.Item;
var Option = Select.Option;
var TabPane = Tabs.TabPane;
var Dragger = Upload.Dragger;
/**
 * 编辑渲染组件
 *
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
var TableEditComponent = /** @class */ (function (_super) {
    __extends(TableEditComponent, _super);
    function TableEditComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        return _this;
    }
    /**
     * 表单 item
     * @param param0
     */
    TableEditComponent.prototype.renderItem = function (params) {
        if (this.props.renderItem) {
            return this.props.renderItem(params);
        }
    };
    /**
     * 渲染按钮组
     */
    TableEditComponent.prototype.renderButtons = function () {
        var _this = this;
        var button = [];
        var _a = this.Store, Actions = _a.Actions, selectedRowKeys = _a.selectedRowKeys;
        var deletelength = selectedRowKeys.length;
        if (Actions.insert) {
            button.push(React.createElement(Button, { icon: "folder-add", onClick: this.Store.onModalShow.bind(this.Store, {}) }, "\u6DFB\u52A0"));
        }
        if (Actions.import) {
            button.push(React.createElement(Button, { icon: "cloud-download", onClick: function () { _this.Store.onPageState("visiblePort", true); } }, "  \u5BFC\u5165 / \u5BFC\u51FA "));
        }
        if (Actions.delete) {
            button.push(React.createElement(Popconfirm, { placement: "right", title: "Sure to delete ? length : (" + deletelength + ") ", onConfirm: this.onDelete.bind(this), okText: "Yes", cancelText: "No" },
                React.createElement(Button, { icon: "delete", disabled: deletelength < 1 }, "\u6279\u91CF\u5220\u9664")));
        }
        return button.map(function (x, i) {
            return React.createElement(React.Fragment, { key: i },
                x,
                React.createElement(Divider, { type: "vertical" }));
        });
    };
    /**
     * 多选删除事件
     */
    TableEditComponent.prototype.onDelete = function () {
        return __awaiter(this, void 0, void 0, function () {
            var params;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        params = this.Store.dataSource.list.filter(function (x) { return _this.Store.selectedRowKeys.some(function (y) { return y == x.key; }); });
                        return [4 /*yield*/, this.Store.onDelete(params)];
                    case 1:
                        _a.sent();
                        return [2 /*return*/];
                }
            });
        });
    };
    TableEditComponent.prototype.render = function () {
        return (React.createElement(Row, null,
            this.renderButtons(),
            React.createElement(EditComponent, __assign({}, this.props, { renderItem: this.renderItem.bind(this) })),
            React.createElement(PortComponent, __assign({}, this.props))));
    };
    TableEditComponent = __decorate([
        observer
    ], TableEditComponent);
    return TableEditComponent;
}(React.Component));
export default TableEditComponent;
/**
 * 编辑
 */
var EditComponent = /** @class */ (function (_super) {
    __extends(EditComponent, _super);
    function EditComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        _this.WrappedFormComponent = Form.create()(FormComponent);
        return _this;
    }
    EditComponent.prototype.render = function () {
        var _this = this;
        return (React.createElement(Drawer, { title: this.Store.pageState.isUpdate ? '修改' : '添加', width: 500, placement: "right", closable: false, onClose: function () { return _this.Store.onPageState("visibleEdit", false); }, visible: this.Store.pageState.visibleEdit, className: "app-table-edit-drawer", destroyOnClose: true },
            React.createElement(this.WrappedFormComponent, __assign({}, this.props, { renderItem: this.props.renderItem }))));
    };
    EditComponent = __decorate([
        observer
    ], EditComponent);
    return EditComponent;
}(React.Component));
/**
 * 表单
 */
var FormComponent = /** @class */ (function (_super) {
    __extends(FormComponent, _super);
    function FormComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        /**
          * 提交数据
          */
        _this.handleSubmit = function (e) {
            e.preventDefault();
            _this.props.form.validateFields(function (err, values) {
                if (!err) {
                    values = mapValues(values, "YYYY-MM-DD");
                    _this.Store.onEdit(values);
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
        var value = this.Store.details[key];
        switch (type) {
            case 'int32':
                return value == null ? 0 : value;
                break;
            case 'date-time':
                return this.moment(value);
                break;
            default: //默认字符串
                return value || '';
                break;
        }
    };
    /**
     * 时间转化
     * @param date
     */
    FormComponent.prototype.moment = function (date) {
        if (date == '' || date == null || date == undefined) {
            date = moment(new Date(), this.Store.Format.date);
        }
        if (typeof date == 'string') {
            date = moment(date, this.Store.Format.date);
        }
        else {
            date = moment(date);
        }
        return date;
    };
    /**
     * 表单 项
     */
    FormComponent.prototype.renderItem = function () {
        var getFieldDecorator = this.props.form.getFieldDecorator;
        return this.props.renderItem({ form: this.props.form, initialValue: this.initialValue.bind(this) });
    };
    FormComponent.prototype.componentWillUnmount = function () {
        this.Store.onPageState("loadingEdit", false);
    };
    FormComponent.prototype.render = function () {
        var _this = this;
        return (React.createElement(Form, { onSubmit: this.handleSubmit, className: "app-table-edit-form" },
            React.createElement(Spin, { spinning: this.Store.pageState.loadingEdit }, this.renderItem()),
            React.createElement("div", { className: "app-table-edit-btns" },
                React.createElement(Button, { onClick: function () { return _this.Store.onPageState("visibleEdit", false); } }, "\u53D6\u6D88 "),
                React.createElement(Divider, { type: "vertical" }),
                React.createElement(Button, { loading: this.Store.pageState.loadingEdit, type: "primary", htmlType: "submit" }, "\u63D0\u4EA4 "))));
    };
    FormComponent = __decorate([
        observer
    ], FormComponent);
    return FormComponent;
}(React.Component));
/**
 * 导入导出
 */
var PortComponent = /** @class */ (function (_super) {
    __extends(PortComponent, _super);
    function PortComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        return _this;
    }
    PortComponent.prototype.render = function () {
        var _this = this;
        return (React.createElement(Modal, { title: "Import&Export", centered: true, visible: this.Store.pageState.visiblePort, 
            // destroyOnClose={true}
            closable: false, onOk: function () { _this.Store.onPageState("visiblePort", false); }, onCancel: function () { _this.Store.onPageState("visiblePort", false); }, className: "app-table-port-modal" },
            React.createElement(Tabs, { defaultActiveKey: "Import" },
                React.createElement(TabPane, { tab: React.createElement("span", null,
                        React.createElement(Icon, { type: "cloud-upload" }),
                        "Import"), key: "Import" },
                    React.createElement("div", { className: "app-table-port-tab-pane" },
                        React.createElement(Button, { icon: "download", block: true, size: "large", onClick: function () { _this.Store.onTemplate(); } }, "\u6A21\u677F"),
                        React.createElement(Divider, null),
                        React.createElement(Dragger, __assign({}, this.Store.importConfig),
                            React.createElement("p", { className: "ant-upload-drag-icon" },
                                React.createElement(Icon, { type: "inbox" })),
                            React.createElement("p", { className: "ant-upload-text" }, "\u5355\u51FB\u6216\u62D6\u52A8\u6587\u4EF6\u5230\u8BE5\u533A\u57DF\u4E0A\u8F7D")))),
                React.createElement(TabPane, { tab: React.createElement("span", null,
                        React.createElement(Icon, { type: "cloud-download" }),
                        "Export"), key: "Export" },
                    React.createElement("div", { className: "app-table-port-tab-pane" },
                        React.createElement(Alert, { message: "\u5BFC\u51FA\u5F53\u524D\u7B5B\u9009\u6761\u4EF6\u4E0B\u7684\u6570\u636E", type: "info", showIcon: true }),
                        React.createElement(Divider, null),
                        React.createElement(Button, { icon: "download", block: true, size: "large", onClick: function () { _this.Store.onExport(); } }, "download"))))));
    };
    PortComponent = __decorate([
        observer
    ], PortComponent);
    return PortComponent;
}(React.Component));
/**
 * 处理数据类型
 * @param values
 */
export function mapValues(values, dateFormat) {
    return lodash.mapValues(
    // 去除空值
    lodash.pickBy(values, function (data) { return !lodash.isNil(data); }), function (data) {
        // if (data instanceof moment) {
        //   console.log(data);
        //   data = moment(data.format(dateFormat))
        // }
        if (Array.isArray(data) && data.some(function (x) { return x instanceof moment; })) {
            // data = data.map(x => moment(x.format(dateFormat)).valueOf()).join(',')
            data = data.map(function (x) { return x.valueOf(); }).join(',');
        }
        return data.valueOf();
    });
}
/**
 * 编辑 装饰器
 * @param Store 状态
 */
export function DecoratorsTableEdit(Store) {
    return function (Component) {
        return /** @class */ (function (_super) {
            __extends(class_1, _super);
            function class_1() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            class_1.prototype.render = function () {
                return React.createElement(TableEditComponent, { Store: Store, renderItem: function (params) {
                        return React.createElement(Component, __assign({}, params, { Store: Store }));
                    } });
            };
            return class_1;
        }(React.Component));
    };
}
//# sourceMappingURL=tableEdit.js.map