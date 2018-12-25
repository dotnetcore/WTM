import Table from 'components/dataView/content/table';
import Visible from 'components/dataView/help/visible';
import React from 'react';
import Store from '../store';
import { Divider, Popconfirm } from 'antd';
export default class extends React.Component<any, any> {
    Store = Store;
    async onDelete(data) {
        this.Store.onDelete(data.ID)
    }
    async onUpdate(data) {
        this.Store.onModalShow(data)
    }
    render() {
        return <Table Store={Store} columns={
            [
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
                    title: 'Action',
                    dataIndex: 'Action',
                    render: (text, record) => {
                        return <div>
                            <Visible visible={this.Store.Actions.update}>
                                <a onClick={this.onUpdate.bind(this, record)} >修改</a>
                            </Visible>
                            <Visible visible={this.Store.Actions.delete}>
                                <Divider type="vertical" />
                                <Popconfirm title="确定删除?" onConfirm={this.onDelete.bind(this, record)} >
                                    <a >删除</a>
                                </Popconfirm>
                            </Visible>
                        </div>
                    },
                }
            ]
        } />
    }
}
