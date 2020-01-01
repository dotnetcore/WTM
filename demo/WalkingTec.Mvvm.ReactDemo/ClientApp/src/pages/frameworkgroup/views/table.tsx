import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import { mergeLocales } from 'locale';
import React from 'react';
import Store from '../store';
import Action from './action';
mergeLocales({
    "zh-CN": {
        'frameworkgroup.GroupCode': '用户组编码',
        'frameworkgroup.GroupName': '用户组名称',
        'frameworkgroup.GroupRemark': '备注',
    },
    "en-US": {
        'frameworkgroup.GroupCode': 'GroupCode',
        'frameworkgroup.GroupName': 'GroupName',
        'frameworkgroup.GroupRemark': 'Remark',
    }
});
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "frameworkgroup.GroupCode", field: "GroupCode"
    },
    {
        headerName: "frameworkgroup.GroupName", field: "GroupName",
    },
    {
        headerName: "frameworkgroup.GroupRemark", field: "GroupRemark",
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
