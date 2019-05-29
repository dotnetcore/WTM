import { columnsRender, columnsRenderImg, columnsRenderDownload, DataViewTable } from 'components/dataView';
import { DesError } from 'components/decorators';
import React from 'react';
import Store from '../store';
import Action from './action';
import RequestFiles from 'utils/RequestFiles';
import { Icon } from 'antd';
/**
 * 列 信息配置
 * 完整参数列表 https://ant.design/components/table-cn/#components-table-demo-dynamic-settings
 * dataIndex:属性名称 区分大小写
 * title:表格显示的中文标题
 */
const columns = [

    {
        dataIndex: "PageName",
        title: "页面名称",
        render: columnsRender
    },

    {
        dataIndex: "DisplayOrder",
        title: "顺序",
        render: columnsRender
    },

    {
        dataIndex: "ICon",
        title: "图标",
        render: (text, record) => {
            if (text) {
                return <div style={{ textAlign: "center" }} >
                    <img style={{ height: 20, width: 20, objectFit: "cover" }} src={RequestFiles.onFileDownload(text)} alt="" />
                </div>
            }
            if (record.CustomICon) {
                return <div style={{ textAlign: "center" }} >
                    <Icon type={record.CustomICon} style={{ fontSize: 20 }} />
                </div>
            }
            return null
        }
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
        return <DataViewTable Store={Store} columns={this.renderColumns()} pagination={false} childrenColumnName="Children" defaultExpandAllRows={true} />
    }
}
