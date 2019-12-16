import { EntitiesUserStore,EntitiesPageStore } from '@leng/public/src';
import { Avatar, Button, Card, Descriptions, Pagination } from 'antd';
import { inject, observer } from 'mobx-react';
import AgGridReact from 'components/grid';
import * as React from 'react';
export interface IAppProps {
    UserStore?: EntitiesUserStore,
    PageStore?: EntitiesPageStore,
}
@inject('UserStore','PageStore')
@observer
export default class UserInfo extends React.Component<IAppProps> {
    componentDidMount() {
        console.log("TCL: App -> componentDidMount -> this.props", this.props)
    }
    render() {
        const { UserStore,PageStore } = this.props;
        return <Card title="用户面板" extra={<Button type="link" onClick={() => {
            // UserStore.onOutLogin()
            PageStore.onExport()
        }} >退出</Button>} >
            <Descriptions title="用户信息" column={1}>
                <Descriptions.Item label="UserName">{UserStore.Name}</Descriptions.Item>
                <Descriptions.Item label="Birthday">{UserStore.Birthday.toLocaleDateString()}</Descriptions.Item>
                <Descriptions.Item label="Age">{UserStore.Age}</Descriptions.Item>
                <Descriptions.Item label="Avatar"> <Avatar src={UserStore.Avatar} /></Descriptions.Item>
                <Descriptions.Item label="Address">{UserStore.Address}</Descriptions.Item>
            </Descriptions>
            <TestTable />
        </Card>
    }
}

class TestTable extends React.Component<any> {
    componentDidMount() {
    }
    state = {
        columnDefs: [
            { headerName: "Make", field: "make" },
            { headerName: "Model", field: "model" },
            { headerName: "Price", field: "price" }],
        rowData: [
            { make: "Toyota", model: "Celica", price: 35000 },
            { make: "Ford", model: "Mondeo", price: 32000 },
            { make: "Porsche", model: "Boxter", price: 72000 }]
    }
    render() {
        const { UserStore } = this.props;
        return <AgGridReact
            columnDefs={this.state.columnDefs}
            rowData={this.state.rowData} >
        </AgGridReact>
    }
}