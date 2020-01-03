import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import { mergeLocales } from 'locale';
import React from 'react';
import Store from '../store';
import Action from './action';
mergeLocales({
    "zh-CN": {
        'frameworkrole.RoleCode': '角色编号',
        'frameworkrole.RoleName': '角色名称',
        'frameworkrole.RoleRemark': '备注',
    },
    "en-US": {
        'frameworkrole.RoleCode': 'RoleCode',
        'frameworkrole.RoleName': 'RoleName',
        'frameworkrole.RoleRemark': 'Remark',
    }
});
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "frameworkrole.RoleCode", field: "RoleCode"
    },
    {
        headerName: "frameworkrole.RoleName", field: "RoleName",
    },
    {
        headerName: "frameworkrole.RoleRemark", field: "RoleRemark",
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
            rowActionCol={{ width: 300 }}
        // frameworkComponents={{
        // }}
        />
    }
}
