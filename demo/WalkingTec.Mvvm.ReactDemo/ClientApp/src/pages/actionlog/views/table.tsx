import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "类型", field: "LogType", enableRowGroup: true
    },
    {
        headerName: "模块", field: "ModuleName", enableRowGroup: true
    },
    {
        headerName: "动作", field: "ActionName", enableRowGroup: true
    },
    {
        headerName: "ITCode", field: "ITCode", enableRowGroup: true
    },
    {
        headerName: "Url", field: "ActionUrl",
    },
    {
        headerName: "操作时间", field: "ActionTime",
    },
    {
        headerName: "时长", field: "Duration", enableValue: true
    },
    {
        headerName: "IP", field: "IP",
    },
    {
        headerName: "备注", field: "Remark",
    },
]
/**
 * 表格
 */
export default class extends React.Component<any, any> {
    render() {
        return <AgGrid
            // 分组工具栏 
            rowGroupPanelShow="always"
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
