import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';

// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "用户组编码", field: "GroupCode"
    },
    {
        headerName: "用户组名称", field: "GroupName",
    },
    {
        headerName: "备注", field: "GroupRemark",
    },
]
/**
 * 表格
 */
export default class extends React.Component<any, any> {
    render() {
        return <AgGrid
            // 页面状态 
            Store={Store}
            // 列配置
            columnDefs={columnDefs}
            // 行操作 
            rowAction={Action.rowAction}
            // 行操作 col props 同 columnDefs配置相同
            // rowActionCol={{ headerName: "操作" }}
            // frameworkComponents={{
            // }}
        />
    }
}
