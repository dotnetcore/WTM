import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import { mergeLocales } from 'locale';
import React from 'react';
import Store from '../store';
import Action from './action';
mergeLocales({
    "zh-CN": {
        'dataprivilege.Name': '授权对象',
        'dataprivilege.TableName': '权限名称',
        'dataprivilege.RelateIDs': '权限',
    },
    "en-US": {
        'dataprivilege.Name': 'Name',
        'dataprivilege.TableName': 'TableName',
        'dataprivilege.RelateIDs': 'Privileges',
    }
});
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "dataprivilege.Name", field: "Name"
    },
    {
        headerName: "dataprivilege.TableName", field: "TableName",
    },
    {
        headerName: "dataprivilege.RelateIDs", field: "RelateIDs",
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
