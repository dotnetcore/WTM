/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-06-26 16:55:28
 * @modify date 2019-06-26 16:55:28
 * @desc [description]
 */
// import { IActionProps, Action } from '../action';
import { GridApi, GridReadyEvent, SelectionChangedEvent } from 'ag-grid-community';
import 'ag-grid-community/dist/styles/ag-grid.css';
import 'ag-grid-community/dist/styles/ag-theme-balham.css';
import "ag-grid-enterprise";
import globalConfig from 'global.config';
// import "./style.scss";
import { AgGridReact, AgGridReactProps } from 'ag-grid-react';
import { Pagination } from 'antd';
import { PaginationProps } from 'antd/lib/pagination';
import lodash from 'lodash';
import { Debounce, BindAll } from 'lodash-decorators';
import * as React from 'react';
import { fromEvent, Subscription } from 'rxjs';
import Store from 'store/dataSource';
import { observer } from 'mobx-react';
interface ITableProps extends AgGridReactProps {
    /** 状态 */
    Store: Store,
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
}
export class AgGrid extends React.Component<ITableProps, any> {
    /**
     * 全屏 容器
     */
    refFullscreen = React.createRef<HTMLDivElement>();
    render() {
        let {
            // actions,
            // dropdown,
            ...props
        } = this.props;
        return (
            // <div className='lenovo-collapse-refFullscreen' ref={this.refFullscreen}>
            //     {/* <Action actions={actions} dropdown={dropdown} fullscreenBody={() => this.refFullscreen} /> */}
            //     <Table {...props} />
            // </div>
            <Table {...props} />
        );
    }
}
@observer
@BindAll()
class Table extends React.Component<ITableProps, any> {
    gridApi: GridApi;
    // 表格容器
    refTableBody = React.createRef<HTMLDivElement>();
    // 事件对象
    resizeEvent: Subscription;
    minHeight = 400;
    state = {
        height: this.minHeight
    }
    /**
     * 修改 高度 
     * @param refFullscreen 
     */
    @Debounce(200)
    onUpdateHeight(refFullscreen = false) {
        try {
            // props 中传递了 height
            if (this.props.style && this.props.style.height) {
                return
            }
            const refTable = this.refTableBody.current;//ReactDOM.findDOMNode(this.ref.current) as HTMLDivElement;
            // 60 是头部 标题栏 高度
            let height = window.innerHeight - refTable.offsetTop - 60 - 100;
            // // 全屏 加回 60 高度
            // if (refFullscreen) {
            //     height += 60;
            // }
            height = height < this.minHeight ? this.minHeight : height;
            if (this.state.height !== height) {
                this.gridApi.sizeColumnsToFit();
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
    /**
     * 选择的 行 数据 回调
     * @param event 
     */
    onSelectionChanged(event: SelectionChangedEvent) {
        console.log("TCL: App -> onSelectionChanged -> event", event.api.getSelectedRows())
        this.props.Store.DataSource.selectedRowKeys = lodash.map(event.api.getSelectedRows(), 'key');
    }

    componentDidMount() {
        this.props.Store.onSearch();
        this.onUpdateHeight();
        this.resizeEvent = fromEvent(window, "resize").subscribe(e => {
            this.onUpdateHeight(lodash.get(e, 'detail') === 'refFullscreen');
            this.gridApi.sizeColumnsToFit();
        });
    }
    componentWillUnmount() {
        this.resizeEvent && this.resizeEvent.unsubscribe()
    }
    onGridReady(event: GridReadyEvent) {
        this.gridApi = event.api;
        // 更新 列 大小
        event.api.sizeColumnsToFit();
    }
    public render() {
        let {
            Store,
            paginationProps,
            style,
            theme = 'ag-theme-balham',
            className = '',
            children,
            onGridReady,
            loading,
            defaultColDef,
            // rowData,
            ...props
        } = this.props;
        const { DataSource } = Store;
        const dataSource = DataSource.tableList;
        const rowData = [...dataSource.Data];
        if (loading) {
            props.rowData = undefined
        }
        return (
            <>
                <div ref={this.refTableBody} style={{ height: this.state.height, width: '100%', transition: 'all .1s', ...style }} className={`lenovo-ag-grid ${className} ${theme}`}>
                    <AgGridReact
                        rowSelection="multiple"
                        {...props}
                        defaultColDef={{
                            sortable: true,
                            filter: true,
                            resizable: true,
                            // autoHeight: true,
                            ...defaultColDef,
                        }}
                        rowData={rowData}
                        onSelectionChanged={this.onSelectionChanged}
                        onGridReady={event => {
                            onGridReady && onGridReady(event);
                            this.onGridReady(event);
                        }}
                    />
                    {/* <Spin spinning={true} /> */}
                </div>
                <Pagination
                    className='ant-table-pagination'
                    {...{
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
            </>
        );
    }
}

export default AgGrid