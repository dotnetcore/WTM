import { columnsRender, columnsRenderImg, DataViewTable } from 'components/dataView';
import { DesError } from 'components/decorators';
import React from 'react';
import Store from '../store';
import Action from './action';
/**
 * 列 信息配置
 * 完整参数列表 https://ant.design/components/table-cn/#components-table-demo-dynamic-settings
 * dataIndex:属性名称 区分大小写
 * title:表格显示的中文标题
 */
const columns = [

    {
        dataIndex: "ITCode",
        title: "账号",
        render: columnsRender 
    },

    {
        dataIndex: "Name",
        title: "姓名",
        render: columnsRender 
    },

    {
        dataIndex: "Sex",
        title: "性别",
        render: columnsRender 
    },

    {
        dataIndex: "CellPhone",
        title: "手机",
        render: columnsRender 
    },

    {
        dataIndex: "PhotoId",
        title: "照片",
        render: columnsRenderImg 
    },

    {
        dataIndex: "IsValid",
        title: "是否有效",
        render: columnsRender 
    },

    {
        dataIndex: "RoleName_view",
        title: "角色",
        render: columnsRender 
    },

    {
        dataIndex: "GroupName_view",
        title: "用户组",
        render: columnsRender 
    }

]

/**
 * 表格
 */
@DesError
export default class extends React.Component<any, any> {
    /**
     * 操作动作
     */
    renderColumns() {
        const tableColumns: any[] = [...columns];
        // 根据需求 加入行动作
        if (true) {
            tableColumns.push(
                {
                    title: '动作',
                    dataIndex: 'Action',
                    fixed: 'right',//固定 列
                    width: 160,
                    render: (text, record) => <Action.rowAction data={record} />
                }
            )
        }
        return tableColumns
    }
    render() {
        return <DataViewTable Store={Store} columns={this.renderColumns()} />
    }
}
