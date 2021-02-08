import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [

    {
        field: "MajorCode",
        headerName: "专业编码"
    },

    {
        field: "MajorName",
        headerName: "专业名称"
    },

    {
        field: "MajorType",
        headerName: "专业类别"
    },

    {
        field: "Remark",
        headerName: "备注"
    },

    {
        field: "SchoolName_view",
        headerName: "所属学校"
    },

    {
        field: "Name_view",
        headerName: "学生"
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
