/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-06-26 16:55:28
 * @modify date 2019-06-26 16:55:28
 * @desc [description]
 */
import { ColDef, ColumnRowGroupChangedEvent, GridApi, GridReadyEvent, SelectionChangedEvent, SortChangedEvent, ColGroupDef } from 'ag-grid-community';
import 'ag-grid-community/dist/styles/ag-grid.css';
// import 'ag-grid-community/dist/styles/ag-theme-fresh.css';
// import 'ag-grid-community/dist/styles/ag-theme-blue.css';
import 'ag-grid-community/dist/styles/ag-theme-balham.css';
// import 'ag-grid-community/dist/styles/ag-theme-bootstrap.css';
import 'ag-grid-community/dist/styles/ag-theme-material.css';
import 'ag-grid-community/dist/styles/ag-theme-alpine.css';
import { LicenseManager } from 'ag-grid-enterprise';
import { AgGridReact, AgGridReactProps } from 'ag-grid-react';
import { Button, Icon, Pagination, Spin, Switch } from 'antd';
import { PaginationProps } from 'antd/lib/pagination';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { BindAll, Debounce } from 'lodash-decorators';
import { toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import Store from 'store/dataSource';
import Regular from 'utils/Regular';
import RequestFiles from 'utils/RequestFiles';
import { ToImg } from '../help/toImg';
import localeText from './localeText ';
import { IntlProvider } from 'react-intl';
import { getLocalesValue, getLocales } from 'locale';
LicenseManager.setLicenseKey('ag-Grid_Evaluation_License_Not_for_Production_100Devs30_August_2037__MjU4ODczMzg3NzkyMg==9e93ed5f03b0620b142770f2594a23a2');
declare namespace AgGridReactProps {
    interface ColDef {
        headerName?: Object;
    }
}

interface ITableProps extends AgGridReactProps {
    /** 状态 */
    Store: Store;
    /**
     * 行 操作
     */
    rowAction?: any;
    /**
     * 行 操作列配置
     */
    rowActionCol?: ColDef;
    /**
     * 容器样式
     */
    style?: React.CSSProperties;
    /**
     * 主题
     */
    theme?: string;
    /**
     * 样式
     */
    className?: string;
    /**
     * 分页
     */
    paginationProps?: PaginationProps | boolean;
    /**
     * 加载中
     */
    loading?: boolean;
    /**
     * 选择
     */
    checkboxSelection?: boolean;
}
const frameworkRender = {
    // 图片
    columnsRenderImg: (props) => {
        return columnsRenderImg(props.value, props.data)
    },
    // 布尔
    columnsRenderBoolean: (props) => {
        return (props.value === true || props.value === 'true') ? <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled defaultChecked /> : <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled />
    },
    // 下载
    columnsRenderDownload: (props) => {
        if (props.value) {
            return <div style={{ textAlign: "center" }} >
                <Button style={{ height: 25, width: 25, objectFit: "cover" }} shape="circle" icon="download" onClick={e => {
                    window.open(RequestFiles.onFileDownload(props.value))
                }} />
            </div>
        }
        return null
    },
    // 默认
    columnsRenderDefault: (props) => {
        if (props.value) {
            let render = props.value
            // colDef.field
            if (Regular.isHtml.test(props.value)) {
                render = <span dangerouslySetInnerHTML={{ __html: props.value }}></span>
            }
            // // 前景色
            // const forecolor = lodash.get(props.data, props.colDef.field + '__forecolor');
            // // 背景色
            // const backcolor = lodash.get(props.data, props.colDef.field + '__backcolor');
            // if (forecolor || backcolor) {
            //     render = <div style={{ color: forecolor, backgroundColor: backcolor, display: "inline-block" }}>{render}</div>
            // }
            return render
        } else {
            return ''
        }
    }
}
// export class AgGrid extends React.Component<ITableProps, any> {
//     /**
//      * 全屏 容器
//      */
//     refFullscreen = React.createRef<HTMLDivElement>();
//     render() {
//         let {
//             ...props
//         } = this.props;
//         return (
//             <Table {...props} />
//         );
//     }
// }
@observer
@BindAll()
export class GridCommunity extends React.Component<ITableProps, any> {
    gridApi: GridReadyEvent;
    // 表格容器
    refTableBody = React.createRef<HTMLDivElement>();
    // 事件对象
    resizeEvent: Subscription;
    minHeight = 400;
    state = {
        sortable: true,
        height: this.minHeight
    }
    /**
     * 修改 高度
     * @param refFullscreen
     */
    @Debounce(100)
    onUpdateHeight(refFullscreen = false) {
        try {
            // props 中传递了 height
            if (this.props.style && this.props.style.height) {
                return
            }
            const refTable = this.refTableBody.current;//ReactDOM.findDOMNode(this.ref.current) as HTMLDivElement;
            // 60 是头部 标题栏 高度
            let height = window.innerHeight - refTable.offsetTop - 168;
            if (!globalConfig.settings.tabsPage) {
                height += 90;
            }
            height = height < this.minHeight ? this.minHeight : height;
            if (this.state.height !== height) {
                this.gridApi.api.sizeColumnsToFit();
                this.setState({ height });
            }
        } catch (error) {
            console.log(error)
        }
    }
    // onGetColumnDefs() {
    //     let columnDefs = [...this.props.columnDefs];
    //     console.log(columnDefs.reduce((accumulator, currentValue) => {
    //         return accumulator.width + currentValue.w
    //     }), 0)
    //     return columnDefs;
    // }
    /**
     * 分页 参数 改变回调
     * @param current
     * @param pageSize
     */
    onChangePagination(current: number, pageSize: number) {
        console.log("TCL: App -> onChangePagination -> ", current, pageSize)
        this.props.Store.onSearch({
            Page: current,
            Limit: pageSize
        });
    }
    onSortChanged(event: SortChangedEvent) {
        const SortModel = lodash.head(event.api.getSortModel());
        this.props.Store.onSearch({
            SortInfo: SortModel && SortModel.sort && { Direction: lodash.capitalize(SortModel.sort), Property: SortModel.colId },
            Page: 1,
            Limit: this.props.Store.DataSource.searchParams.Limit
        })
    }
    /**
     * 选择的 行 数据 回调
     * @param event
     */
    onSelectionChanged(event: SelectionChangedEvent) {
        // console.log("TCL: App -> onSelectionChanged -> event", event.api.getSelectedRows())
        // event.api.getSelectedNodesById()
        this.props.Store.DataSource.selectedRowKeys = lodash.map(event.api.getSelectedRows(), 'key');
    }
    onColumnRowGroupChanged(event: ColumnRowGroupChangedEvent) {
        // this.setState({ sortable: event.columns.length > 0 })
    }
    async componentDidMount() {
        this.onUpdateHeight();
        this.resizeEvent = fromEvent(window, "resize").pipe(debounceTime(300)).subscribe(e => {
            // 获取当前高度 ，高度 为 0 说明页面属于隐藏状态
            if (lodash.get(this.refTableBody.current, 'clientHeight', 0) > 0) {
                // if (!globalConfig.settings.tabsPage) {
                this.onUpdateHeight(lodash.get(e, 'detail') === 'refFullscreen');
                // }
                // this.sizeColumnsToFit();
                this.autoSizeAll();
            }
        });
        // await this.props.Store.onSearch();
        lodash.defer(() => {
            this.sizeColumnsToFit();
            this.autoSizeAll();
            this.setSelected();
        });
    }
    @Debounce(100)
    autoSizeAll() {
        // var allColumnIds = [];
        // this.gridApi.columnApi.getAllColumns().forEach((column: any) => {
        //     allColumnIds.push(column.colId);
        // });
        this.gridApi.columnApi.autoSizeColumns(['Action']);
    }
    setSelected() {
        this.gridApi &&
            this.gridApi.api.forEachNode((rowNode) => {
                rowNode.setSelected(lodash.includes(this.props.Store.DataSource.selectedRowKeys, lodash.get(rowNode, 'data.key')));
            });
    }
    componentWillUnmount() {
        this.resizeEvent && this.resizeEvent.unsubscribe()
    }
    sizeColumnsToFit() {
        if (lodash.get(this.refTableBody.current, 'clientHeight', 0) && this.gridApi) {
            this.gridApi.api.sizeColumnsToFit();
        }
    }
    onGridReady(event: GridReadyEvent) {
        this.gridApi = event;
        lodash.defer(() => {
            this.sizeColumnsToFit();
            this.autoSizeAll();
            this.setSelected();
        });
    }
    public render() {
        let {
            Store,
            rowAction: RowAction,
            rowActionCol,
            paginationProps,
            style,
            theme = globalConfig.settings.agGridTheme,//'ag-theme-balham',
            className = '',
            children,
            onGridReady,
            loading,
            defaultColDef,
            columnDefs,
            checkboxSelection = true,
            frameworkComponents,
            // rowData,
            ...props
        } = this.props;
        const { DataSource, PageState } = Store;
        const { language } = globalConfig;
        const dataSource = DataSource.tableList;
        const checkboxSelectionWidth = {
            "ag-theme-balham": 40,
            "ag-theme-material": 70,
            "ag-theme-alpine": 50,
        }[theme];
        if (loading) {
            props.rowData = undefined
        } else {
            props.rowData = toJS(dataSource.Data);
        }
        // 分组工具栏
        if (!!!props.treeData && !props.rowGroupPanelShow) {
            props.rowGroupPanelShow = "always"
        }
        columnDefs = lodash.cloneDeep(columnDefs);
        const colDefMap = (col: any) => {
            // 根据 数据 定制 样式
            col.cellStyle = col.cellStyle ||
                ((props) => {
                    if (props.data) {
                        // 前景色
                        const forecolor = lodash.get(props.data, props.colDef.field + '__forecolor');
                        // 背景色
                        const backcolor = lodash.get(props.data, props.colDef.field + '__bgcolor');
                        return { color: forecolor, backgroundColor: backcolor }
                    }
                });
            // 渲染器
            col.cellRenderer = col.cellRenderer || 'columnsRenderDefault';
            // if (language === "en-US") {
            //     col.headerName = col.field;
            // }
            if (col.field != "RowAction") {
                col.headerName = getLocalesValue(col.headerName, col.headerName);
            }
            if (col.children) {
                col.children = col.children.map(colDefMap);
            }
            return col
        }
        if (checkboxSelection) {
            columnDefs.unshift({
                // pivotIndex: 0,
                rowDrag: false,
                dndSource: false,
                lockPosition: true,
                // dndSourceOnRowDrag: false,
                suppressMenu: true,
                suppressSizeToFit: true,
                suppressMovable: true,
                suppressNavigable: true,
                suppressCellFlash: true,
                // rowGroup: false,
                enableRowGroup: false,
                enablePivot: false,
                enableValue: false,
                suppressResize: false,
                editable: false,
                suppressColumnsToolPanel: true,
                suppressAutoSize: true,
                filter: false,
                resizable: false,
                checkboxSelection: true,
                headerCheckboxSelection: true,
                width: checkboxSelectionWidth,
                maxWidth: checkboxSelectionWidth,
                minWidth: checkboxSelectionWidth,
                pinned: 'left',
            });
        }
        if (RowAction) {
            columnDefs.push({
                headerName: language === "zh-CN" ? "操作" : 'Action',
                field: "RowAction",
                cellRenderer: 'RowAction',
                pinned: 'right',
                sortable: false,
                menuTabs: [],
                // minWidth: 120,
                ...rowActionCol,
            })
        }
        // 替换默认的渲染器
        columnDefs = columnDefs.map(colDefMap);

        // console.log("TCL: GridCommunity -> render -> PageState.tableLoading", PageState.tableLoading)
        const key = `${theme}_${language}`;
        const PVmessages = getLocales(language);

        return (
            <>
                <div ref={this.refTableBody} style={{ height: this.state.height, ...style }} className={`app-ag-grid  ${className} ${theme}`}>
                    <Spin spinning={PageState.tableLoading} size="large" indicator={<Icon type="loading" spin />} />
                    <AgGridReact
                        key={key}
                        // 内置 翻译 替换
                        localeText={language === "zh-CN" && localeText}
                        // suppressMenuHide
                        // 禁用“加载” 叠加层。
                        suppressNoRowsOverlay
                        // 禁用“无行” 覆盖。
                        suppressLoadingOverlay
                        // 设置为true以启用范围选择。
                        enableRangeSelection
                        animateRows
                        // suppressMakeColumnVisibleAfterUnGroup
                        // suppressDragLeaveHidesColumns
                        rowSelection="multiple"
                        // 分组工具栏 
                        // rowGroupPanelShow="always"
                        sideBar={{
                            toolPanels: [
                                {
                                    id: 'columns',
                                    labelDefault: 'Columns',
                                    labelKey: 'columns',
                                    iconKey: 'columns',
                                    toolPanel: 'agColumnsToolPanel',
                                    toolPanelParams: {
                                        // suppressRowGroups: true,
                                        suppressValues: true,
                                        suppressPivots: true,
                                        suppressPivotMode: true
                                        // suppressSideButtons: true,
                                        // suppressColumnFilter: true,
                                        // suppressColumnSelectAll: true,
                                        // suppressColumnExpandAll: true
                                    }
                                },
                                {
                                    id: 'filters',
                                    labelDefault: 'Filters',
                                    labelKey: 'filters',
                                    iconKey: 'filter',
                                    toolPanel: 'agFiltersToolPanel',
                                }
                            ]
                        }}
                        {...props}
                        frameworkComponents={
                            {
                                RowAction: (rowProps) => {
                                    if (rowProps.data) {
                                        return <IntlProvider locale={language} messages={PVmessages}>
                                            <RowAction {...rowProps} />
                                        </IntlProvider>;
                                    }
                                    return null;
                                },
                                ...frameworkRender,
                                ...frameworkComponents,
                            }
                        }
                        defaultColDef={{
                            // editable: true,
                            resizable: true,
                            sortable: true,
                            minWidth: 100,
                            filter: true,
                            enableRowGroup: true,
                            ...defaultColDef
                        }}
                        autoGroupColumnDef={{
                            sortable: false
                        }}
                        columnDefs={columnDefs}
                        // 分组改变
                        onColumnRowGroupChanged={this.onColumnRowGroupChanged}
                        // 选择框
                        onSelectionChanged={this.onSelectionChanged}
                        // 排序
                        onSortChanged={this.onSortChanged}
                        onGridReady={event => {
                            onGridReady && onGridReady(event);
                            this.onGridReady(event);
                        }}
                    />
                    {/* </Spin> */}
                </div>
                <div className='app-table-pagination'>
                    <Pagination

                        {...{
                            disabled: PageState.tableLoading,
                            position: "bottom",
                            showSizeChanger: true,//是否可以改变 pageSize
                            showQuickJumper: true,
                            pageSize: dataSource.Limit,
                            pageSizeOptions: lodash.get(globalConfig, 'pageSizeOptions', ['10', '20', '30', '40', '50', '100', '200']),
                            size: "small",
                            current: dataSource.Page,
                            // defaultPageSize: dataSource.Limit,
                            // defaultCurrent: dataSource.Page,
                            total: dataSource.Count,
                            onChange: this.onChangePagination,
                            onShowSizeChange: this.onChangePagination
                        }}
                    />
                </div>
            </>
        );
    }
}

export default GridCommunity

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