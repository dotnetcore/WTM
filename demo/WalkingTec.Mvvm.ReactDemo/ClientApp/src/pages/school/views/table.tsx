import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [

    {
        field: "SchoolCode",
        headerName: "学校编码"
    },

    {
        field: "SchoolName",
        headerName: "学校名称"
    },

    {
        field: "SchoolType",
        headerName: "学校类型"
    },

    {
        field: "Remark",
        headerName: "备注"
    },

    {
        field: "Level",
        headerName: "级别"
    },

    {
        field: "Name_view",
        headerName: "地点"
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
