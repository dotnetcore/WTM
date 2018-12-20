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
            dataIndex: "dateFormatted",
            title: "dateFormatted"
        },
        {
            dataIndex: "summary",
            title: "summary"
        }, {
            dataIndex: "temperatureC",
            title: "temperatureC"
        },
        {
            dataIndex: "temperatureF",
            title: "temperatureF"
        },
        {
            title: 'Action',
            dataIndex: 'Action',
            render: () => {
                return <div>
                    <a  >修改</a>
                    <Divider type="vertical" />
                    <Popconfirm title="Sure to delete?" >
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
