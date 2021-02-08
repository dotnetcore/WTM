import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [

    {
        field: "ID",
        headerName: "账号"
    },

    {
        field: "Password",
        headerName: "密码"
    },

    {
        field: "Email",
        headerName: "邮箱"
    },

    {
        field: "Name",
        headerName: "姓名"
    },

    {
        field: "Sex",
        headerName: "性别"
    },

    {
        field: "CellPhone",
        headerName: "手机"
    },

    {
        field: "Address",
        headerName: "住址"
    },

    {
        field: "ZipCode",
        headerName: "邮编"
    },

    {
        field: "PhotoId",
        headerName: "照片",
        cellRenderer: "columnsRenderImg" 
    },

    {
        field: "FileId",
        headerName: "附件",
        cellRenderer: "columnsRenderDownload" 
    },

    {
        field: "IsValid",
        headerName: "是否有效",
        cellRenderer: "columnsRenderBoolean" 
    },

    {
        field: "EnRollDate",
        headerName: "日期"
    },

    {
        field: "MajorName_view",
        headerName: "专业"
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
