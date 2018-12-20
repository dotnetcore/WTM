import { Alert, Divider, Popconfirm } from 'antd';
import React from 'react';
import { DecoratorsTableBody } from 'components/table/tableBody';
import Store from '../store';

/**
 * 组件继承 支持重写,
 */
@DecoratorsTableBody({
    Store: Store,
    // 列属性配置
    columns: [
        {
            dataIndex: "id",
            title: "id"
        },
        {
            dataIndex: "updateby",
            title: "修改人"
        }, {
            dataIndex: "address",
            title: "地址"
        },
        {
            dataIndex: "remark",
            title: "备注"
        },
        {
            title: 'Action',
            dataIndex: 'Action',
            render: () => {
                return <div>
                    <a onClick={this.Store.onModalShow.bind(this.Store, this.props.data)} >修改</a>
                    <Divider type="vertical" />
                    <Popconfirm title="Sure to delete?" onConfirm={this.onDelete.bind(this)} >
                        <a >删除</a>
                    </Popconfirm>
                </div>
            },
        }
    ]
})
export default class BodyComponent extends React.Component<any, any> {
    render() {
        return null
    }
}
