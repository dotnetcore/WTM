import * as React from 'react';
import { Tabs } from 'antd';
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import From from "./form"
import DataSource from "./form/dataSource"

const TabPane = Tabs.TabPane;
@BindAll()
export default class App extends React.Component<any, any> {
    onChange(key) {
        // this.props.history.replace(`/demo/${key}`)
    }
    render() {
        // const activeKey = lodash.get(this.props, 'match.params.activeKey', '1');
        return (
            <Tabs
                defaultActiveKey="2"
                tabPosition="left"
                onChange={this.onChange}
            >
                <TabPane tab="表单" key="1">
                    <From />
                </TabPane>
                <TabPane tab="数据获取 " key="2">
                    <DataSource />
                </TabPane>
                <TabPane tab="Tab 3" key="3">
                    Content of Tab Pane 3
                </TabPane>
            </Tabs>
        );
    }
}
