/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:22
 * @modify date 2018-09-12 18:53:22
 * @desc [description]
*/
import { Divider, Popconfirm, Row, Table, Alert, Popover } from 'antd';
import Store from 'store/dataSource';
import { observer } from 'mobx-react';
import moment from 'moment';
import * as React from 'react';
import lodash from 'lodash';
import { Resizable } from 'react-resizable';
import ReactDOM from 'react-dom';
import Rx, { Observable, Subscription } from 'rxjs';
import { ColumnProps } from 'antd/lib/table';
import { observable, toJS, action, runInAction } from 'mobx';
interface ITablePorps {
    /** 状态 */
    Store: Store,
    /**
    *  处理 表格类型输出
    * @param column 
    * @param index 
    */
    columns: ColumnProps<any>[];
    // columnsMap?: (column: any, index: any, width: any) => any;
}
/**
 * 表格渲染组件 
 * 
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
@observer
export default class TableComponent extends React.Component<ITablePorps, any> {
    @observable columns = this.props.columns;
    Store = this.props.Store;
    /**
     * 初始化列参数配置
     */
    @action.bound
    initColumns() {
        if (this.rowDom && this.rowDom.clientWidth) {
            const width = Math.floor(this.rowDom.clientWidth / (this.columns.length + 1))
            this.columns = this.columns.map((col, index) => {
                return this.columnsMap(col, index, width)
            })
        }
    }
    /**
    *  处理 表格类型输出
    * @param column 
    * @param index 
    */
    columnsMap(column, index, width) {
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

        // }
        if (!column.render) {
            column.render = (record) => {
                try {
                    record = record && record.toString() || record
                } catch (error) {
                    record = error.toString()
                }
                return (
                    <Popover placement="topLeft" overlayClassName="app-table-column-popover" content={record} trigger="click">
                        <div className="app-table-column-render">
                            {record}
                        </div>
                    </Popover>
                )
            }
        }
        return {
            ...column,
            sorter: true,
            width: width,
            // 列拖拽
            onHeaderCell: col => ({
                width: col.width,
                onResize: this.handleResize(index),
            }),
        }
    }
    /**
     * 分页、排序、筛选变化时触发
     * @param page 
     * @param filters 
     * @param sorter 
     */
    onChange(page, filters, sorter) {
        let sort = "";
        if (sorter.columnKey) {
            if (sorter.order == 'descend') {
                sort = `${sorter.columnKey} desc`
            } else {
                sort = `${sorter.columnKey} asc`
            }
        }
        this.Store.onSearch({}, sort, page.current, page.pageSize)
    }

    /**
     * 覆盖默认的 table 元素
     */
    private components = {
        header: {
            cell: (props) => {
                const { onResize, width, ...restProps } = props;

                if (!width) {
                    return <th {...restProps} />;
                }

                return (
                    <Resizable width={width} height={0} onResize={onResize}>
                        <th {...restProps} />
                    </Resizable>
                );
            },
        },
    };
    /**
     * 拖拽
     */
    handleResize(index) {
        return (e, { size }) => {
            let column = {
                ...this.columns[index],
                width: size.width,
            }
            runInAction(() => {
                this.columns = [
                    ...this.columns.slice(0, index),
                    column,
                    ...this.columns.slice(index + 1, this.columns.length),
                ]
            })
        }
    }
    resize: Subscription;
    private rowDom: HTMLDivElement;
    componentDidMount() {
        this.Store.onSearch({}, "", this.Store.dataSource.Page, this.Store.dataSource.Limit);
        this.initColumns();
        // 窗口变化重新计算列宽度
        this.resize = Rx.Observable.fromEvent(window, "resize").debounceTime(800).subscribe(e => {
            this.initColumns();
            // this.forceUpdate();
        });
    }
    componentWillUnmount() {
        this.resize.unsubscribe();
    }
    render() {
        const dataSource = this.Store.dataSource;
        /**
        * 行选择
        */
        const rowSelection = {
            selectedRowKeys: this.Store.selectedRowKeys,
            onChange: e => this.Store.onSelectChange(e),
        };
        const columns = this.columns.slice();
        if (dataSource.Data) {
            return (
                <Row ref={e => this.rowDom = ReactDOM.findDOMNode(e) as any}>
                    <Table
                        bordered
                        components={this.components}
                        dataSource={dataSource.Data.slice()}
                        onChange={this.onChange.bind(this)}
                        columns={columns}
                        rowSelection={rowSelection}
                        loading={this.Store.pageState.loading}
                        pagination={
                            {
                                // hideOnSinglePage: true,//只有一页时是否隐藏分页器
                                position: "bottom",
                                showSizeChanger: true,//是否可以改变 pageSize
                                pageSize: dataSource.Limit,
                                current: dataSource.Page,
                                defaultPageSize: dataSource.Limit,
                                defaultCurrent: dataSource.Page,
                                total: dataSource.Count
                            }
                        }
                    />
                </Row>
            );
        } else {
            return <div>
                <Divider />
                <Alert
                    showIcon
                    message="数据格式并非table标准格式请使用其他模板或者检查接口数据是否有误"
                    type="warning"
                />
            </div>
        }

    }
}
