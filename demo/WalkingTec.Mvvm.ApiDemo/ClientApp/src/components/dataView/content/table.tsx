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

const TableUtils = {
    clientWidth: 0,
    /**
     * 设置列宽度
     * @param tableBody 
     * @param columns 
     */
    onSetColumnsWidth(tableBody, columns) {
        // 获取页面宽度
        if (tableBody) {
            // 表头
            const { clientWidth } = tableBody.querySelector(".ant-table-thead");
            this.clientWidth = clientWidth;
            // 选择框
            // const { clientWidth: selectionWidth } = tableBody.querySelector(".ant-table-thead .ant-table-selection-column");
            const columnsLenght = columns.length;
            //计算表格设置的总宽度
            const columnWidth = this.onGetcolumnsWidth(columns);
            // 总宽度差值
            const width = clientWidth - columnWidth - 100;
            if (width > 0) {
                const average = width / columnsLenght
                // 平均分配
                columns = columns.map(x => {
                    return {
                        ...x,
                        width: Math.ceil((x.width || 0) + average)
                    }
                })
            } else {
                const average = clientWidth / columnsLenght
                columns = columns.map(x => {
                    return {
                        ...x,
                        width: Math.ceil(average)
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
        let scrollX = this.onGetcolumnsWidth(columns);
        // scrollX = scrollX > this.clientWidth ? scrollX : this.clientWidth;
        return {
            x: scrollX
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
export default class TableComponent extends React.Component<ITablePorps, any> {
    @observable columns = this.props.columns;
    Store = this.props.Store;
    /**
     * 初始化列参数配置
     */
    @action.bound
    initColumns() {
        this.columns = TableUtils.onSetColumnsWidth(this.rowDom, toJS(this.columns))
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
    /**
     * 列配置
     */
    onGetColumns() {
        return this.columns.map((column, index) => {
            if (column.dataIndex === "Action") {
                // let width = 150;
                // try {
                //     const fixed = this.rowDom.querySelector('.ant-table-fixed-columns-in-body');
                //     width = fixed.clientWidth
                //     console.log(width);
                // } catch (error) {

                // }
                return { ...column };
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

    private rowDom: HTMLDivElement;
    componentDidMount() {
        this.Store.onSearch({}, "", this.Store.dataSource.Page, this.Store.dataSource.Limit);
        this.initColumns();
    }
    componentWillUnmount() {
    }

    render() {
        const dataSource = this.Store.dataSource;
        if (dataSource.Data) {
            const columns = this.onGetColumns();
            return (
                <Row ref={e => this.rowDom = ReactDOM.findDOMNode(e) as any}>
                    <Table
                        bordered
                        size="middle" 
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
