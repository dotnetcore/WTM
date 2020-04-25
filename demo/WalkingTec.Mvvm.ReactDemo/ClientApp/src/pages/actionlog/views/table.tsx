import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import { mergeLocales } from 'locale';
import React from 'react';
import Store from '../store';
import Action from './action';
mergeLocales({
    "zh-CN": {
        'actionlog.LogType': '类型',
        'actionlog.ModuleName': '模块',
        'actionlog.ActionName': '动作',
        'actionlog.ITCode': 'ITCode',
        'actionlog.ActionUrl': 'Url',
        'actionlog.ActionTime': '操作时间',
        'actionlog.Duration': '时长',
        'actionlog.IP': 'IP',
        'actionlog.Remark': '备注',
    },
    "en-US": {
        'actionlog.LogType': 'LogType',
        'actionlog.ModuleName': 'Module',
        'actionlog.ActionName': 'Action',
        'actionlog.ITCode': 'ITCode',
        'actionlog.ActionUrl': 'Url',
        'actionlog.ActionTime': 'ActionTime',
        'actionlog.Duration': 'Duration',
        'actionlog.IP': 'IP',
        'actionlog.Remark': 'Remark',
    }
});
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "actionlog.LogType", field: "LogType",
    },
    {
        headerName: "actionlog.ModuleName", field: "ModuleName"
    },
    {
        headerName: "actionlog.ActionName", field: "ActionName"
    },
    {
        headerName: "actionlog.ITCode", field: "ITCode"
    },
    {
        headerName: "actionlog.ActionUrl", field: "ActionUrl",
    },
    {
        headerName: "actionlog.ActionTime", field: "ActionTime",
    },
    {
        headerName: "actionlog.Duration", field: "Duration"
    },
    {
        headerName: "actionlog.IP", field: "IP",
    },
    {
        headerName: "actionlog.Remark", field: "Remark", enableRowGroup: false
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
