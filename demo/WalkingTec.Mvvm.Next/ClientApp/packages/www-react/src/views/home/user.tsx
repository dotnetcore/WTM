import { EntitiesUserStore } from '@leng/public/src';
import { Avatar, Button, Card, Descriptions } from 'antd';
import { inject, observer } from 'mobx-react';
import * as React from 'react';
export interface IAppProps {
    UserStore?: EntitiesUserStore,
}
@inject('UserStore')
@observer
export default class UserInfo extends React.Component<IAppProps> {
    componentDidMount() {
        console.log("TCL: App -> componentDidMount -> this.props", this.props)
    }
    render() {
        const { UserStore } = this.props;
        return <Card title="用户面板" extra={<Button type="link" onClick={() => UserStore.onOutLogin()} >退出</Button>} style={{ width: 300 }}>
            <Descriptions title="用户信息" column={1}>
                <Descriptions.Item label="UserName">{UserStore.Name}</Descriptions.Item>
                <Descriptions.Item label="Birthday">{UserStore.Birthday.toLocaleDateString()}</Descriptions.Item>
                <Descriptions.Item label="Age">{UserStore.Age}</Descriptions.Item>
                <Descriptions.Item label="Avatar"> <Avatar src={UserStore.Avatar} /></Descriptions.Item>
                <Descriptions.Item label="Address">{UserStore.Address}</Descriptions.Item>
            </Descriptions>
        </Card>
    }
}
