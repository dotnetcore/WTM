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
var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) if (e.indexOf(p[i]) < 0)
            t[p[i]] = s[p[i]];
    return t;
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
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:22
 * @modify date 2018-09-12 18:53:22
 * @desc [description]
*/
import { Divider, Popconfirm, Row, Table } from 'antd';
import { observer } from 'mobx-react';
import moment from 'moment';
import * as React from 'react';
import { Resizable } from 'react-resizable';
import "./style.less";
import ReactDOM from 'react-dom';
import Rx from 'rxjs';
/**
 * 表格渲染组件
 *
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
var TableBodyComponent = /** @class */ (function (_super) {
    __extends(TableBodyComponent, _super);
    function TableBodyComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        _this.columns = _this.props.columns;
        /**
         * 覆盖默认的 table 元素
         */
        _this.components = {
            header: {
                cell: function (props) {
                    var onResize = props.onResize, width = props.width, restProps = __rest(props, ["onResize", "width"]);
                    if (!width) {
                        return React.createElement("th", __assign({}, restProps));
                    }
                    return (React.createElement(Resizable, { width: width, height: 0, onResize: onResize },
                        React.createElement("th", __assign({}, restProps))));
                },
            },
        };
        /**
         * 拖拽
         */
        _this.handleResize = function (index) { return function (e, _a) {
            var size = _a.size;
            var column = __assign({}, _this.columns[index], { width: size.width });
            _this.columns = __spread(_this.columns.slice(0, index), [
                column
            ], _this.columns.slice(index + 1, _this.columns.length));
            _this.forceUpdate();
        }; };
        return _this;
    }
    /**
     * 初始化列参数配置
     */
    TableBodyComponent.prototype.initColumns = function () {
        // if (this.rowDom && this.rowDom.clientWidth) {
        //     const width = Math.floor((this.rowDom.clientWidth) / (this.columns.length + 1))
        //     this.columns = this.columns.map((col, index) => {
        //         return this.columnsMap(col, index, width)
        //     })
        // }
    };
    /**
    *  处理 表格类型输出
    * @param column
    * @param index
    */
    TableBodyComponent.prototype.columnsMap = function (column, index, width) {
        var _this = this;
        if (this.props.columnsMap) {
            return this.props.columnsMap(column, index, width);
        }
        switch (column.format) {
            // 转换时间类型 输出
            case 'date-time':
                column.render = function (record) {
                    try {
                        if (record == null || record == undefined) {
                            return "";
                        }
                        return moment(record).format(_this.Store.Format.dateTime);
                    }
                    catch (error) {
                        return error.toString();
                    }
                };
                break;
        }
        return __assign({}, column, { sorter: true, width: width, 
            // 列拖拽
            onHeaderCell: function (col) { return ({
                width: col.width,
                onResize: _this.handleResize(index),
            }); } });
    };
    /**
     * 选项
     * @param text
     * @param record
     */
    TableBodyComponent.prototype.renderAction = function (text, record) {
        if (this.props.renderAction) {
            return this.props.renderAction(text, record);
        }
        return React.createElement(ActionComponent, __assign({}, this.props, { data: record }));
    };
    /**
     * 分页、排序、筛选变化时触发
     * @param page
     * @param filters
     * @param sorter
     */
    TableBodyComponent.prototype.onChange = function (page, filters, sorter) {
        this.Store.onSearch({}, {
            pageNo: page.current,
            pageSize: page.pageSize
        });
    };
    TableBodyComponent.prototype.componentDidMount = function () {
        var _this = this;
        this.Store.onSearch();
        this.initColumns();
        // 窗口变化重新计算列宽度
        this.resize = Rx.Observable.fromEvent(window, "resize").debounceTime(800).subscribe(function (e) {
            _this.initColumns();
            // this.forceUpdate();
        });
    };
    TableBodyComponent.prototype.componentWillUnmount = function () {
        this.resize.unsubscribe();
    };
    TableBodyComponent.prototype.render = function () {
        var _this = this;
        var dataSource = this.Store.dataSource;
        /**
        * 行选择
        */
        var rowSelection = {
            selectedRowKeys: this.Store.selectedRowKeys,
            onChange: function (e) { return _this.Store.onSelectChange(e); },
        };
        return (React.createElement(Row, { ref: function (e) { return _this.rowDom = ReactDOM.findDOMNode(e); } },
            React.createElement(Divider, null),
            React.createElement(Table, { bordered: true, components: this.components, dataSource: dataSource.list.slice(), onChange: this.onChange.bind(this), columns: this.columns, rowSelection: rowSelection, loading: this.Store.pageState.loading, pagination: {
                    // hideOnSinglePage: true,//只有一页时是否隐藏分页器
                    position: "top",
                    showSizeChanger: true,
                    pageSize: dataSource.pageSize,
                    defaultPageSize: dataSource.pageSize,
                    defaultCurrent: dataSource.pageNo,
                    total: dataSource.count
                } })));
    };
    TableBodyComponent = __decorate([
        observer
    ], TableBodyComponent);
    return TableBodyComponent;
}(React.Component));
export default TableBodyComponent;
/**
 * 数据操作按钮
 */
var ActionComponent = /** @class */ (function (_super) {
    __extends(ActionComponent, _super);
    function ActionComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.Store = _this.props.Store;
        return _this;
    }
    ActionComponent.prototype.onDelete = function () {
        return __awaiter(this, void 0, void 0, function () {
            var data;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, this.Store.onDelete([this.props.data])];
                    case 1:
                        data = _a.sent();
                        if (data) {
                            this.Store.onSearch();
                        }
                        return [2 /*return*/];
                }
            });
        });
    };
    ActionComponent.prototype.render = function () {
        return (React.createElement(React.Fragment, null,
            this.Store.Actions.update ? React.createElement("a", { onClick: this.Store.onModalShow.bind(this.Store, this.props.data) }, "\u4FEE\u6539") : null,
            React.createElement(Divider, { type: "vertical" }),
            this.Store.Actions.delete ?
                React.createElement(Popconfirm, { title: "Sure to delete?", onConfirm: this.onDelete.bind(this) },
                    React.createElement("a", null, "\u5220\u9664")) : null));
    };
    return ActionComponent;
}(React.Component));
/**
 * table 装饰器
 * @param params
 */
export function DecoratorsTableBody(params) {
    return function (Component) {
        return /** @class */ (function (_super) {
            __extends(class_1, _super);
            function class_1() {
                return _super !== null && _super.apply(this, arguments) || this;
            }
            class_1.prototype.render = function () {
                return React.createElement(React.Fragment, null,
                    React.createElement(TableBodyComponent, __assign({}, params)),
                    React.createElement(Component, __assign({}, params)));
            };
            return class_1;
        }(React.Component));
    };
}
//# sourceMappingURL=tableBody.js.map