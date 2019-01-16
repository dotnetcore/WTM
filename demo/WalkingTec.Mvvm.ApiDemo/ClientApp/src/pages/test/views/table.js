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
import Table from 'components/dataView/content/table';
import Visible from 'components/dataView/help/visible';
import React from 'react';
import Store from '../store';
import { Divider, Popconfirm } from 'antd';
var default_1 = /** @class */ (function (_super) {
    __extends(default_1, _super);
    function default_1() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    default_1.prototype.onDelete = function (data) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                Store.onDelete(data.ID);
                return [2 /*return*/];
            });
        });
    };
    default_1.prototype.onUpdate = function (data) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                Store.onModalShow(data, "Update");
                return [2 /*return*/];
            });
        });
    };
    default_1.prototype.onInfo = function (data) {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                Store.onModalShow(data, "Info");
                return [2 /*return*/];
            });
        });
    };
    default_1.prototype.render = function () {
        var _this = this;
        return React.createElement(Table, { Store: Store, columns: [
                {
                    dataIndex: "ID",
                    title: "ID"
                },
                {
                    dataIndex: "ITCode",
                    title: "ITCode"
                }, {
                    dataIndex: "Name",
                    title: "Name"
                },
                {
                    dataIndex: "PhotoId",
                    title: "PhotoId"
                },
                {
                    dataIndex: "Roles",
                    title: "Roles"
                },
                {
                    title: 'Action',
                    dataIndex: 'Action',
                    render: function (text, record) {
                        return React.createElement("div", null,
                            React.createElement("a", { onClick: _this.onInfo.bind(_this, record) }, "\u8BE6\u60C5"),
                            React.createElement(Visible, { visible: Store.Actions.update },
                                React.createElement(Divider, { type: "vertical" }),
                                React.createElement("a", { onClick: _this.onUpdate.bind(_this, record) }, "\u4FEE\u6539")),
                            React.createElement(Visible, { visible: Store.Actions.delete },
                                React.createElement(Divider, { type: "vertical" }),
                                React.createElement(Popconfirm, { title: "\u786E\u5B9A\u5220\u9664?", onConfirm: _this.onDelete.bind(_this, record) },
                                    React.createElement("a", null, "\u5220\u9664"))));
                    },
                }
            ] });
    };
    return default_1;
}(React.Component));
export default default_1;
//# sourceMappingURL=table.js.map