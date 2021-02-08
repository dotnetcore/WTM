import { ColDef, ColGroupDef } from 'ag-grid-community';
import { AgGrid } from 'components/dataView';
import React from 'react';
import Store from '../store';
import Action from './action';
// 列配置
const columnDefs: (ColDef | ColGroupDef)[] = [

    {
        field: "Name",
        headerName: "名称"
    },

    {
        field: "Level",
        headerName: "Level"
    },

    {
        field: "Name_view",
        headerName: "父级"
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
