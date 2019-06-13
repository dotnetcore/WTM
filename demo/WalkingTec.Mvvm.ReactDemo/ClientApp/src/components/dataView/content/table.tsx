/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:53:22
 * @modify date 2018-09-12 18:53:22
 * @desc [description]
*/
import { Alert, Button, Divider, Icon, Switch, Table } from 'antd';
import { ColumnProps, TableProps, TableRowSelection } from 'antd/lib/table';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { Debounce } from 'lodash-decorators';
import { action, observable, runInAction, toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import ReactDOM from 'react-dom';
import { Resizable } from 'react-resizable';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import Store from 'store/dataSource';
import Regular from 'utils/Regular';
import { ToImg } from '../help/toImg';
import './style.less';
import RequestFiles from 'utils/RequestFiles';
import { Help } from 'utils/Help';


interface ITablePorps extends TableProps<any> {
    /** 状态 */
    Store: Store,
}

const TableUtils = {
    // 页面宽度
    // clientWidth: 0,
    // // 左边选择框 宽度
    // selectionColumnWidth: 0,
    /**
     * 设置列宽度
     * @param tableBody 
     * @param columns 
     */
    onSetColumnsWidth(tableBody: HTMLDivElement, columns: ColumnProps<any>[]) {
        // 获取页面宽度
        if (tableBody) {
            const notCol = lodash.filter(columns, data => lodash.isString(data.fixed) || data.width !== 201);
            // 表头
            const notColLenght = notCol.length// columns.length //columns.filter(x => typeof x.fixed !== "string").length;
            //计算表格设置的总宽度
            const columnWidth = this.onGetcolumnsWidth(columns, 0) + 50;
            const { clientWidth } = tableBody;
            if (columnWidth > clientWidth) {
            } else {
                const width = Math.ceil((clientWidth - columnWidth) / (columns.length - notColLenght));
                lodash.mapValues(columns, (data: any) => {
                    if (typeof data.width === "number" && !(lodash.isString(data.fixed) || data.width !== 201)) {
                        data.width += width;
                    }
                    return data;
                })
            }
            return columns
        }
    },
    /**
     * 获取列总宽度
     * @param columns 
     */
    onGetcolumnsWidth(columns, width = 201) {
        //计算表格设置的总宽度
        return columns.reduce((accumulator, currentValue) => {
            return Math.ceil(accumulator + (currentValue.width || width))
        }, 0);
    },
    /**
    * 动态设置列宽
    */
    onGetScroll(columns) {
        let scrollX = this.onGetcolumnsWidth(columns) //+ TableUtils.selectionColumnWidth;
        // console.log("TCL: onGetScroll -> scrollX", scrollX)
        // scrollX = scrollX > this.clientWidth ? scrollX : this.clientWidth - 10;
        return {
            x: scrollX,
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
    // 原始初始化列配置
    private OriginalColumns = this.props.columns.map(x => {
        x.width = lodash.get(x, 'width', 201)
        return x;
    });
    @observable columns = lodash.cloneDeep(this.OriginalColumns);
    @observable Height = 500;
    @observable TableKey = Help.GUID();
    // 拖拽状态
    isResize = false;
    // 页面状态
    Store = this.props.Store;
    tableRef = React.createRef<any>();
    tableDom: HTMLDivElement;
    // clientWidth = 0;
    /**
     * 初始化列参数配置
     */
    @action.bound
    initColumns() {
        this.columns = this.onGetColumns(TableUtils.onSetColumnsWidth(this.tableDom, this.OriginalColumns));
    }
    /**
     * 分页、排序、筛选变化时触发
     * @param page 
     * @param filters 
     * @param sorter 
     */
    @Debounce(300)
    onChange(page, filters, sorter) {
        if (this.isResize) {
            return console.log("拖拽中")
        }
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
        this.Store.onSearch({
            SortInfo: sort,
            Page: page.current,
            Limit: page.pageSize
        })
    }
    @Debounce(1000)
    onUpdateResize() {
        this.isResize = false;
    }
    /**
     * 拖拽
     */
    handleResize(index) {
        const path = `[${index}].width`;
        return (e, { size }) => {
            // console.log("TCL: DataViewTable -> handleResize -> e", e)
            // let width = lodash.get(this.columns, path);
            // let scrollw = TableUtils.onGetScroll(this.columns).x - width + size.width;
            // // if (TableUtils.clientWidth - scrollw > 5) {
            // //     return
            // // }
            this.isResize = true;
            this.onUpdateResize();
            runInAction(() => {
                lodash.update(this.columns, path, () => {
                    return lodash.max([size.width, 50])
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
                sorter: true,//typeof column.fixed !== "string",
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
    onRowSelection(): TableRowSelection<any> {
        const { DataSource } = this.Store;
        return {
            columnWidth: 50,
            selectedRowKeys: DataSource.selectedRowKeys,
            onChange: (e: string[]) => DataSource.selectedRowKeys = e,
        };
    }
    getHeight() {
        if (globalConfig.lockingTableRoll) {
            return {
                y: this.Height
            }
        }
        return {}
    }
    @action
    onCalculationHeight() {
        if (globalConfig.lockingTableRoll && this.tableDom) {
            // console.log(this.tableDom.offsetTop)
            let height = window.innerHeight - this.tableDom.offsetTop - 95;//(globalConfig.tabsPage ? 210 : 120);
            if (globalConfig.tabsPage) {
                height -= 90;
                if (lodash.some(["right", "left"], data => lodash.eq(data, globalConfig.tabPosition))) {
                    height += 40;
                }
            }
            if (this.Height != height) {
                this.Height = height
            }
        }
    }
    resize: Subscription
    async componentDidMount() {
        try {
            this.tableDom = ReactDOM.findDOMNode(this.tableRef.current) as any;
            this.onCalculationHeight()
            this.initColumns();
            this.resize = fromEvent(window, "resize").pipe(debounceTime(300)).subscribe(e => {
                this.onCalculationHeight();
                this.initColumns();
            });
            await this.Store.onSearch();
            runInAction(() => this.TableKey = Help.GUID())
        } catch (error) {
            console.error(error)
        }
    }
    componentWillUnmount() {
        this.resize && this.resize.unsubscribe()
    }
    render() {
        const { DataSource, PageState } = this.Store;
        const dataSource = DataSource.tableList;
        const tableDataSource = [...dataSource.Data];
        // console.log("TCL: render -> tableDataSource", tableDataSource)
        if (tableDataSource) {
            const columns = this.columns
                .map(x => {
                    return {
                        ...x,
                        render: (text, record, index) => {
                            if (x.render) {
                                return x.render(text, { ...record, __style: { maxWidth: lodash.toNumber(x.width) - 20 } }, index)
                            }
                            return text
                            // return <div className="columns-render" style={{ maxWidth: x.width }}>
                            //     {x.render ? x.render(text, record, index) : text}
                            // </div>
                        }
                    }
                });
            const scroll = { ...TableUtils.onGetScroll(columns), ...this.getHeight() }
            // 分页参数
            const props: TableProps<any> = {
                // bordered: true,
                size: "small",
                className: "data-view-table",
                rowKey: "ID",
                pagination: {
                    // hideOnSinglePage: true,//只有一页时是否隐藏分页器
                    position: "bottom",
                    showSizeChanger: true,//是否可以改变 pageSize
                    showQuickJumper: true,
                    pageSize: dataSource.Limit,
                    pageSizeOptions: lodash.get(globalConfig, 'pageSizeOptions', ['10', '20', '30', '40', '50', '100', '200']),
                    size: "small",
                    current: dataSource.Page,
                    // defaultPageSize: dataSource.Limit,
                    // defaultCurrent: dataSource.Page,
                    total: dataSource.Count
                },
                ...this.props,
                components: TableUtils.components,
                dataSource: tableDataSource,
                onChange: this.onChange.bind(this),
                columns: columns,
                scroll: scroll,
                rowSelection: this.onRowSelection(),
                // loading: PageState.tableLoading,
            }
            return (
                <div ref={this.tableRef}>
                    <Table

                        key={this.TableKey}
                        {...props}
                    />
                </div>
            );
        } else {
            return <div >
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
/**
 * 重写 列渲染 函数 
 * @param text 
 * @param record 
 */
export function columnsRender(text, record) {
    if (lodash.isBoolean(text) || text === "true" || text === "false") {
        text = (text === true || text === "true") ? <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled defaultChecked /> : <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled />;
    } else if (Regular.isHtml.test(text)) {
        // text = <Popover content={
        //     <div dangerouslySetInnerHTML={{ __html: text }}></div>
        // } trigger="hover">
        //     <a>查看详情</a>
        // </Popover>
        text = <div className="data-view-columns-render" style={record.__style} dangerouslySetInnerHTML={{ __html: text }}></div>
    }
    // if (lodash.isString(text) && text.length <= 12) {
    //     return text
    // }
    return <div className="data-view-columns-render" title={text} style={record.__style}>
        <span>{text}</span>
    </div>
}
/**
 * 重写 图片 函数 
 * @param text 
 * @param record 
 */
export function columnsRenderImg(text, record) {
    return <div>
        <ToImg fileID={text} />
    </div>
}
/**
 * 重写 下载 函数 
 * @param text 
 * @param record 
 */
export function columnsRenderDownload(text, record) {
    if (text) {
        return <div style={{ textAlign: "center" }} >
            <Button shape="circle" icon="download" onClick={e => {
                window.open(RequestFiles.onFileDownload(text))
            }} />
        </div>
    }
    return null
}
