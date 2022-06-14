import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [

    {
        field: "TCode",
        headerName: "租户编号"
    },

    {
        field: "TName",
        headerName: "租户名称"
    },

    {
        field: "TDb",
        headerName: "租户数据库"
    },

    {
        field: "TDbType",
        headerName: "数据库类型"
    },

    {
        field: "DbContext",
        headerName: "数据库架构"
    },

    {
        field: "TDomain",
        headerName: "租户域名"
    },

    {
        field: "TenantCode",
        headerName: "租户"
    },

    {
        field: "EnableSub",
        headerName: "允许子租户",
        cellRenderer: "columnsRenderBoolean" 
    },

    {
        field: "Enabled",
        headerName: "启用",
        cellRenderer: "columnsRenderBoolean" 
    }

]
/**
 * 表格
 */
export default class extends React.Component<any, any> {
    render() {
        return <AgGrid
            Store={Store} 
            columnDefs={columnDefs} 
            rowAction={Action.rowAction} 
        />
    }
}
