
import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';

// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "账号", field: "ITCode",
    },
    {
        headerName: "姓名", field: "Name",
    },
    {
        headerName: "性别", field: "Sex",
    },
    {
        headerName: "照片", field: "PhotoId", cellRenderer: "columnsRenderImg"
    },
    {
        headerName: "是否有效", field: "IsValid", cellRenderer: "columnsRenderBoolean"
    },
    {
        headerName: "角色", field: "RoleName_view",
    },
    {
        headerName: "用户组", field: "GroupName_view",
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
            rowHeight={110}
        />
    }
}
