
import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import { mergeLocales } from 'locale';
import React from 'react';
import Store from '../store';
import Action from './action';
mergeLocales({
    "zh-CN": {
        'frameworkuser.ITCode': '账号',
        'frameworkuser.Name': '姓名',
        'frameworkuser.Sex': '性别',
        'frameworkuser.PhotoId': '照片',
        'frameworkuser.IsValid': '是否有效',
        'frameworkuser.RoleName_view': '角色',
        'frameworkuser.GroupName_view': '用户组',
    },
    "en-US": {
        'frameworkuser.ITCode': 'Account',
        'frameworkuser.Name': 'Name',
        'frameworkuser.Sex': 'Gender',
        'frameworkuser.PhotoId': 'Photo',
        'frameworkuser.IsValid': 'IsValid',
        'frameworkuser.RoleName_view': 'RoleName',
        'frameworkuser.GroupName_view': 'GroupName',
    }
});
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [
    {
        headerName: "frameworkuser.ITCode", field: "ITCode",
    },
    {
        headerName: "frameworkuser.Name", field: "Name",
    },
    {
        headerName: "frameworkuser.Sex", field: "Sex",
    },
    {
        headerName: "frameworkuser.PhotoId", field: "PhotoId", cellRenderer: "columnsRenderImg", minWidth: 130
    },
    {
        headerName: "frameworkuser.IsValid", field: "IsValid", cellRenderer: "columnsRenderBoolean"
    },
    {
        headerName: "frameworkuser.RoleName_view", field: "RoleName_view",
    },
    {
        headerName: "frameworkuser.GroupName_view", field: "GroupName_view",
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
