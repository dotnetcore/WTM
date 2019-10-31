import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "类型", field: "LogType",
    },
    {
        headerName: "模块", field: "ModuleName"
    },
    {
        headerName: "动作", field: "ActionName"
    },
    {
        headerName: "ITCode", field: "ITCode"
    },
    {
        headerName: "Url", field: "ActionUrl",
    },
    {
        headerName: "操作时间", field: "ActionTime",
    },
    {
        headerName: "时长", field: "Duration"
    },
    {
        headerName: "IP", field: "IP",
    },
    {
        headerName: "备注", field: "Remark", enableRowGroup: false
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
