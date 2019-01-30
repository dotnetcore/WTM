/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:22
 * @modify date 2018-09-12 18:53:22
 * @desc [description]
*/
import { Alert, Divider, Row, Table, notification } from 'antd';
import { ColumnProps } from 'antd/lib/table';
import { action, observable, runInAction, toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import ReactDOM from 'react-dom';
import lodash from 'lodash';
import { Resizable } from 'react-resizable';
import Rx from 'rxjs';
import Store from 'store/dataSource';
import './style.less';
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

const TableUtils = {
    // 页面宽度
    clientWidth: 0,
    // 左边选择框 宽度
    selectionColumnWidth: 0,
    /**
     * 设置列宽度
     * @param tableBody 
     * @param columns 
     */
    onSetColumnsWidth(tableBody, columns: ColumnProps<any>[]) {
        // 获取页面宽度
        if (tableBody) {
            // 表头
            const { clientWidth } = tableBody.querySelector(".ant-table-thead ");
            // 选择框
            const { clientWidth: selectionWidth = 0 } = tableBody.querySelector(".ant-table-thead .ant-table-selection-column");
            TableUtils.selectionColumnWidth = selectionWidth;
            let exclude = 0;
            const notFixed = columns.filter(x => {
                if (typeof x.fixed === "string") {
                    if (typeof x.width === "number") {
                        exclude += x.width
                    }
                    return false
                }
                return true
            })
            const columnsLenght = notFixed.length;
            //计算表格设置的总宽度
            const columnWidth = this.onGetcolumnsWidth(notFixed);
            // 总宽度差值
            const width = clientWidth - columnWidth - exclude - selectionWidth;
            if (width > 0) {
                const average = Math.ceil(width / columnsLenght)
                // 平均分配
                columns = columns.map(x => {
                    if (typeof x.fixed === "string") {
                        return x;
                    }
                    return {
                        ...x,
                        width: ((x.width as any || 0) + average)
                    }
                })
            } else {
                const average = Math.ceil(TableUtils.clientWidth / columnsLenght);
                columns = columns.map(x => {
                    if (typeof x.fixed === "string") {
                        return x;
                    }
                    return {
                        ...x,
                        width: average
                    }
                })
            }
            return columns
        }
    },
    /**
     * 获取列总宽度
     * @param columns 
     */
    onGetcolumnsWidth(columns) {
        //计算表格设置的总宽度
        return columns.reduce((accumulator, currentValue) => {
            return Math.ceil(accumulator + (currentValue.width || 0))
        }, 0);
    },
    /**
    * 动态设置列宽
    */
    onGetScroll(columns) {
        let scrollX = this.onGetcolumnsWidth(columns) + TableUtils.selectionColumnWidth - 5;
        // scrollX = scrollX > this.clientWidth ? scrollX : this.clientWidth - 10;
        return {
            x: scrollX,
            // y: 550
        }
    },
    /**
     * 高度
     */
    onGetScrollY(tableBody) {
        return {
            y: 0
        }
    },
    /**
     * 覆盖默认的 table 元素
     */
    components: {
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
    }
}
/**
 * 表格渲染组件 
 * 
 * 不要直接修改 wtm 组件 使用继承重写的方式修改
 */
@observer
export class DataViewTable extends React.Component<ITablePorps, any> {
    @observable columns = this.props.columns.map(x => {
        if (typeof x.fixed === "string") {
            if (!x.width) {
                notification.warn({
                    message: "fixed 列 需要设置固定宽度",
                    description: `Title ${x.title}`
                })
                x.width = 150;
            }
            return x;
        }
        x.width = x.width || 100;
        return x;
    });
    Store = this.props.Store;
    tableRef = React.createRef<any>();
    tableDom: HTMLDivElement;
    // clientWidth = 0;
    /**
     * 初始化列参数配置
     */
    @action.bound
    initColumns() {
        this.columns = this.onGetColumns(TableUtils.onSetColumnsWidth(this.tableDom, toJS(this.columns)));
    }
    /**
     * 分页、排序、筛选变化时触发
     * @param page 
     * @param filters 
     * @param sorter 
     */
    onChange(page, filters, sorter) {
        let sort: any = "";
        if (sorter.columnKey) {
            if (sorter.order == 'descend') {
                sort = { Direction: "Desc", Property: sorter.columnKey }
                // sort = `${sorter.columnKey} desc`
            } else {
                sort = { Direction: "Asc", Property: sorter.columnKey }
                // sort = `${sorter.columnKey} asc`
            }
        }
        this.Store.onSearch({}, sort, page.current, page.pageSize)
    }
    /**
     * 拖拽
     */
    handleResize(index) {
        const path = `[${index}].width`;
        return (e, { size }) => {
            let width = lodash.get(this.columns, path);
            let scrollw = TableUtils.onGetScroll(this.columns).x - width + size.width;
            if (TableUtils.clientWidth - scrollw > 5) {
                return
            }
            runInAction(() => {
                lodash.update(this.columns, path, () => {
                    return lodash.max([size.width, 100])
                })
            })
        }
    }
    /**
     * 列配置
     */
    onGetColumns(columns) {
        return columns.map((column, index) => {
            if (typeof column.fixed === "string") {
                return column;
            }
            return {
                ...column,
                sorter: true,
                onHeaderCell: col => ({
                    width: col.width,
                    onResize: this.handleResize(index),
                }),
            }
        });
    }
    /**
     * 行选择
     */
    onRowSelection() {
        return {
            selectedRowKeys: this.Store.selectedRowKeys,
            onChange: e => this.Store.onSelectChange(e),
        };
    }
    /**
     * 
     */
    resize: Rx.Subscription
    componentDidMount() {
        try {
            this.tableDom = ReactDOM.findDOMNode(this.tableRef.current) as any;
            TableUtils.clientWidth = this.tableDom.clientWidth;
            this.Store.onSearch({}, "", this.Store.dataSource.Page, this.Store.dataSource.Limit);
            this.initColumns();
            this.resize = Rx.Observable.fromEvent(window, "resize").subscribe(e => {
                if (this.tableDom.clientWidth > TableUtils.clientWidth) {
                    TableUtils.clientWidth = this.tableDom.clientWidth;
                    this.initColumns();
                }
            });
        } catch (error) {

        }
    }
    componentWillUnmount() {
        this.resize && this.resize.unsubscribe()
    }
    render() {
        const dataSource = this.Store.dataSource;
        if (dataSource.Data) {
            const columns = [...this.columns];
            console.log(TableUtils.clientWidth, TableUtils.onGetScroll(columns).x);
            return (
                <Row >
                    <Table
                        ref={this.tableRef}
                        bordered
                        size="default"
                        className="data-view-table"
                        components={TableUtils.components}
                        dataSource={[...dataSource.Data]}
                        onChange={this.onChange.bind(this)}
                        columns={columns}
                        scroll={TableUtils.onGetScroll(columns)}
                        rowSelection={this.onRowSelection()}
                        loading={this.Store.pageState.loading}
                        pagination={
                            {
                                // hideOnSinglePage: true,//只有一页时是否隐藏分页器
                                position: "bottom",
                                showSizeChanger: true,//是否可以改变 pageSize
                                showQuickJumper: true,
                                pageSize: dataSource.Limit,
                                size: "default",
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
