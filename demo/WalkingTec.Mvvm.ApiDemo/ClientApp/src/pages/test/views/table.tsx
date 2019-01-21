import Table from 'components/dataView/content/table';
import Visible from 'components/dataView/help/visible';
import ToImg from 'components/dataView/help/toImg';
import React from 'react';
import Store from '../store';
import { Divider, Popconfirm } from 'antd';

/**
 * 表格
 */
export default class extends React.Component<any, any> {
    async onDelete(data) {
        Store.onDelete(data.ID)
    }
    async onUpdate(data) {
        Store.onModalShow(data, "Update")
    }
    async onInfo(data) {
        Store.onModalShow(data, "Info")
    }
    /**
     * 操作动作
     */
    renderColumns() {
        return [...columns,
        {
            title: 'Action',
            dataIndex: 'Action',
            // fixed: 'right',
            render: (text, record) => {
                return <div style={{ whiteSpace: "nowrap", width: 155, textAlign: "center" }}>
                    <a onClick={this.onInfo.bind(this, record)} >详情</a>
                    <Visible visible={Store.Actions.update}>
                        <Divider type="vertical" />
                        <a onClick={this.onUpdate.bind(this, record)} >修改</a>
                    </Visible>
                    <Visible visible={Store.Actions.delete}>
                        <Divider type="vertical" />
                        <Popconfirm title="确定删除?" onConfirm={this.onDelete.bind(this, record)} >
                            <a >删除</a>
                        </Popconfirm>
                    </Visible>
                </div>
            }
        }];
    }
    render() {
        return <Table Store={Store} columns={this.renderColumns()} />
    }
}
/**
 * 重写 列渲染 函数 
 * @param text 
 * @param record 
 */
const columnsRender = (text, record) => {
    return <div style={{ maxHeight: 60, overflow: "hidden" }} title={text}>
        <span>{text}</span>
    </div>
}
/**
 * 列 信息配置
 * dataIndex:属性名称 区分大小写
 * title:表格显示的中文标题
 */
const columns = [
    {
        dataIndex: "ID",
        title: "ID",
        render: columnsRender
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
        render: (text, record) => {
            return <div>
                <ToImg style={{ height: 60, width: 100 }} download={Store.onFileDownload(text)} url={Store.onGetFile(text)} />
            </div>
        }
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
