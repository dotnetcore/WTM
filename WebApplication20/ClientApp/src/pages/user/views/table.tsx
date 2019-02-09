import { DataViewTable, ToImg } from 'components/dataView';
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
        dataIndex: "ID",
        title: "ID",
        render: columnsRender // 普通文本渲染 可忽略
    },
    {
        dataIndex: "ITCode",
        title: "ITCode",
        render: columnsRender
    }, {
        dataIndex: "Name",
        title: "Name",
        render: columnsRender
    },
    {
        dataIndex: "PhotoId",
        title: "PhotoId",
        render: columnsRenderImg // 图片文件 渲染
    },
    {
        dataIndex: "Roles",
        title: "Roles",
        render: columnsRender
    },
    {
        dataIndex: "Sex",
        title: "Sex",
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
                    title: 'Action',
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
/**
 * 重写 列渲染 函数 
 * @param text 
 * @param record 
 */
function columnsRender(text, record) {
    return <div style={{ maxHeight: 60, overflow: "hidden" }} title={text}>
        <span>{text}</span>
    </div>
}
/**
 * 重写 图片 函数 
 * @param text 
 * @param record 
 */
function columnsRenderImg(text, record) {
    return <div>
        <ToImg fileID={text} style={{ height: 60, width: 100 }} />
    </div>
}
