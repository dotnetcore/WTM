import Table from 'components/dataView/content/table';
import Visible from 'components/dataView/help/visible';
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
    renderColumns() {
        return [...columns,
        {
            title: 'Action',
            dataIndex: 'Action',
            // fixed: 'right',
            render: (text, record) => {
                return <div>
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
/** 列 信息配置 */
const columns = [
    {
        dataIndex: "ID",
        title: "ID"
    },
    {
        dataIndex: "ITCode",
        title: "ITCode"
    }, {
        dataIndex: "Name",
        title: "Name"
    },
    {
        dataIndex: "PhotoId",
        title: "PhotoId"
    },
    {
        dataIndex: "Roles",
        title: "Roles"
    },
    {
        dataIndex: "Sex",
        title: "Sex"
    }
]