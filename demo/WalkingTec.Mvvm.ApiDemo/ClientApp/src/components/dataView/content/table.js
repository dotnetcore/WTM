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
import { Divider, Row, Table, Alert, Popover } from 'antd';
import { observer } from 'mobx-react';
import * as React from 'react';
import { Resizable } from 'react-resizable';
import ReactDOM from 'react-dom';
import Rx from 'rxjs';
import { observable, action, runInAction } from 'mobx';
/**
 * 表格渲染组件
 *
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
var TableComponent = /** @class */ (function (_super) {
    __extends(TableComponent, _super);
    function TableComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.columns = _this.props.columns;
        _this.Store = _this.props.Store;
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
        return _this;
    }
    /**
     * 初始化列参数配置
     */
    TableComponent.prototype.initColumns = function () {
        var _this = this;
        if (this.rowDom && this.rowDom.clientWidth) {
            var width_1 = Math.floor(this.rowDom.clientWidth / (this.columns.length + 1));
            this.columns = this.columns.map(function (col, index) {
                return _this.columnsMap(col, index, width_1);
            });
        }
    };
    /**
    *  处理 表格类型输出
    * @param column
    * @param index
    */
    TableComponent.prototype.columnsMap = function (column, index, width) {
        // if (this.props.columnsMap) {
        //     return this.props.columnsMap(column, index, width);
        // }
        // switch (column.format) {
        //     // 转换时间类型 输出
        //     case 'date-time':
        //         column.render = (record) => {
        //             try {
        //                 if (record == null || record == undefined) {
        //                     return "";
        //                 }
        //                 return moment(record).format(this.Store.Format.dateTime)
        //             } catch (error) {
        //                 return error.toString()
        //             }
        //         }
        //         break;
        //     default:
        var _this = this;
        // }
        if (!column.render) {
            column.render = function (record) {
                try {
                    record = record && record.toString() || record;
                }
                catch (error) {
                    record = error.toString();
                }
                return (React.createElement(Popover, { placement: "topLeft", overlayClassName: "app-table-column-popover", content: record, trigger: "click" },
                    React.createElement("div", { className: "app-table-column-render" }, record)));
            };
        }
        return __assign({}, column, { sorter: true, width: width, 
            // 列拖拽
            onHeaderCell: function (col) { return ({
                width: col.width,
                onResize: _this.handleResize(index),
            }); } });
    };
    /**
     * 分页、排序、筛选变化时触发
     * @param page
     * @param filters
     * @param sorter
     */
    TableComponent.prototype.onChange = function (page, filters, sorter) {
        var sort = "";
        if (sorter.columnKey) {
            if (sorter.order == 'descend') {
                sort = sorter.columnKey + " desc";
            }
            else {
                sort = sorter.columnKey + " asc";
            }
        }
        this.Store.onSearch({}, sort, page.current, page.pageSize);
    };
    /**
     * 拖拽
     */
    TableComponent.prototype.handleResize = function (index) {
        var _this = this;
        return function (e, _a) {
            var size = _a.size;
            var column = __assign({}, _this.columns[index], { width: size.width });
            runInAction(function () {
                _this.columns = __spread(_this.columns.slice(0, index), [
                    column
                ], _this.columns.slice(index + 1, _this.columns.length));
            });
        };
    };
    TableComponent.prototype.componentDidMount = function () {
        var _this = this;
        this.Store.onSearch({}, "", this.Store.dataSource.Page, this.Store.dataSource.Limit);
        this.initColumns();
        // 窗口变化重新计算列宽度
        this.resize = Rx.Observable.fromEvent(window, "resize").debounceTime(800).subscribe(function (e) {
            _this.initColumns();
            // this.forceUpdate();
        });
    };
    TableComponent.prototype.componentWillUnmount = function () {
        this.resize.unsubscribe();
    };
    TableComponent.prototype.render = function () {
        var _this = this;
        var dataSource = this.Store.dataSource;
        /**
        * 行选择
        */
        var rowSelection = {
            selectedRowKeys: this.Store.selectedRowKeys,
            onChange: function (e) { return _this.Store.onSelectChange(e); },
        };
        var columns = this.columns.slice();
        if (dataSource.Data) {
            return (React.createElement(Row, { ref: function (e) { return _this.rowDom = ReactDOM.findDOMNode(e); } },
                React.createElement(Table, { bordered: true, components: this.components, dataSource: dataSource.Data.slice(), onChange: this.onChange.bind(this), columns: columns, rowSelection: rowSelection, loading: this.Store.pageState.loading, pagination: {
                        // hideOnSinglePage: true,//只有一页时是否隐藏分页器
                        position: "bottom",
                        showSizeChanger: true,
                        pageSize: dataSource.Limit,
                        current: dataSource.Page,
                        defaultPageSize: dataSource.Limit,
                        defaultCurrent: dataSource.Page,
                        total: dataSource.Count
                    } })));
        }
        else {
            return React.createElement("div", null,
                React.createElement(Divider, null),
                React.createElement(Alert, { showIcon: true, message: "\u6570\u636E\u683C\u5F0F\u5E76\u975Etable\u6807\u51C6\u683C\u5F0F\u8BF7\u4F7F\u7528\u5176\u4ED6\u6A21\u677F\u6216\u8005\u68C0\u67E5\u63A5\u53E3\u6570\u636E\u662F\u5426\u6709\u8BEF", type: "warning" }));
        }
    };
    __decorate([
        observable
    ], TableComponent.prototype, "columns", void 0);
    __decorate([
        action.bound
    ], TableComponent.prototype, "initColumns", null);
    TableComponent = __decorate([
        observer
    ], TableComponent);
    return TableComponent;
}(React.Component));
export default TableComponent;
//# sourceMappingURL=table.js.map