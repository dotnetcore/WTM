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
import { Button, Divider, Dropdown, Menu, message, Popconfirm, Row, Icon, Modal } from 'antd';
import Visible from 'components/dataView/help/visible';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from '../store';
import Dragger from 'antd/lib/upload/Dragger';
var IApp = /** @class */ (function (_super) {
    __extends(IApp, _super);
    function IApp() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    IApp.prototype.onAdd = function () {
        Store.onModalShow({}, "Insert");
    };
    IApp.prototype.onImport = function () {
        Store.onPageState("visiblePort", true);
    };
    IApp.prototype.onExport = function () {
        Store.onExport();
    };
    IApp.prototype.onExportIds = function () {
        Store.onExportIds();
    };
    /**
     * 多选删除
     */
    IApp.prototype.onDelete = function () {
        return __awaiter(this, void 0, void 0, function () {
            var params;
            return __generator(this, function (_a) {
                params = Store.dataSource.Data.filter(function (x) { return Store.selectedRowKeys.some(function (y) { return y == x.key; }); });
                return [2 /*return*/];
            });
        });
    };
    /**
     * 多选修改
     */
    IApp.prototype.onUpdate = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                if (Store.selectedRowKeys.length == 1) {
                    Store.onModalShow(lodash.find(Store.dataSource.Data, ['key', lodash.head(Store.selectedRowKeys)]), "Update");
                }
                else {
                    message.warn("请选择一条数据");
                }
                return [2 /*return*/];
            });
        });
    };
    IApp.prototype.render = function () {
        var selectedRowKeys = Store.selectedRowKeys, Actions = Store.Actions;
        var deletelength = selectedRowKeys.length;
        var disabled = deletelength < 1;
        return (React.createElement(Row, null,
            React.createElement(Visible, { visible: Actions.insert },
                React.createElement(Button, { icon: "plus", onClick: this.onAdd.bind(this) }, "\u65B0\u5EFA")),
            React.createElement(Visible, { visible: Actions.update },
                React.createElement(Divider, { type: "vertical" }),
                React.createElement(Button, { icon: "edit", onClick: this.onUpdate.bind(this), disabled: disabled }, "\u4FEE\u6539")),
            React.createElement(Visible, { visible: Actions.delete },
                React.createElement(Divider, { type: "vertical" }),
                disabled ?
                    React.createElement(Button, { icon: "delete", disabled: disabled }, " \u5220\u9664  ") :
                    React.createElement(Popconfirm, { placement: "right", title: "\u786E\u5B9A\u5220\u9664 " + deletelength + "\u6761 \u6570\u636E\uFF1F", onConfirm: this.onDelete.bind(this), okText: "\u786E\u5B9A", cancelText: "\u53D6\u6D88" },
                        React.createElement(Button, { icon: "delete" }, " \u5220\u9664  "))),
            React.createElement(Visible, { visible: Actions.import },
                React.createElement(Divider, { type: "vertical" }),
                React.createElement(Button, { icon: "folder-add", onClick: this.onImport.bind(this) }, "\u5BFC\u5165")),
            React.createElement(Divider, { type: "vertical" }),
            React.createElement(Dropdown, { overlay: React.createElement(Menu, null,
                    React.createElement(Menu.Item, null,
                        React.createElement("a", { onClick: this.onExport.bind(this) }, "\u5BFC\u51FA\u5168\u90E8")),
                    React.createElement(Menu.Item, { disabled: disabled },
                        React.createElement("a", { onClick: this.onExportIds.bind(this) }, "\u5BFC\u51FA\u52FE\u9009"))) },
                React.createElement(Button, { icon: "download" }, "\u5BFC\u51FA")),
            React.createElement(PortComponent, null)));
    };
    IApp = __decorate([
        observer
    ], IApp);
    return IApp;
}(React.Component));
export default IApp;
/**
 * 导入导出
 */
var PortComponent = /** @class */ (function (_super) {
    __extends(PortComponent, _super);
    function PortComponent() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PortComponent.prototype.render = function () {
        return (React.createElement(Modal, { title: "\u5BFC\u5165", centered: true, visible: Store.pageState.visiblePort, destroyOnClose: true, width: 600, cancelText: "\u53D6\u6D88", footer: null, onCancel: function () { Store.onPageState("visiblePort", false); } },
            React.createElement("div", null,
                React.createElement("div", null,
                    "\u5BFC\u5165\u8BF4\u660E\uFF1A\u8BF7\u4E0B\u8F7D\u6A21\u7248\uFF0C\u7136\u540E\u5728\u628A\u4FE1\u606F\u8F93\u5165\u5230\u6A21\u7248\u4E2D   ",
                    React.createElement(Divider, { type: "vertical" }),
                    " ",
                    React.createElement(Button, { icon: "download", onClick: function () { Store.onTemplate(); } }, "\u4E0B\u8F7D\u6A21\u677F")),
                React.createElement(Divider, { style: { margin: "5px 0" } }),
                React.createElement(Dragger, __assign({}, Store.importConfig),
                    React.createElement("p", { className: "ant-upload-drag-icon" },
                        React.createElement(Icon, { type: "inbox" })),
                    React.createElement("p", { className: "ant-upload-text" }, "\u5355\u51FB\u6216\u62D6\u52A8\u6587\u4EF6\u5230\u8BE5\u533A\u57DF\u4E0A\u8F7D")))));
    };
    PortComponent = __decorate([
        observer
    ], PortComponent);
    return PortComponent;
}(React.Component));
//# sourceMappingURL=action.js.map