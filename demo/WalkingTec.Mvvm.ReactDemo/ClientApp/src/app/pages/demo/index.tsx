import * as React from 'react';
import { Tabs } from 'antd';
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import From from "./form"

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
                // activeKey={activeKey}
                tabPosition="left"
                onChange={this.onChange}
            >
                <TabPane tab="表单" key="1">
                    <From/>
                </TabPane>
                <TabPane tab="Tab 2" key="2">
                    Content of Tab Pane 2
                    </TabPane>
                <TabPane tab="Tab 3" key="3">
                    Content of Tab Pane 3
                </TabPane>
            </Tabs>
        );
    }
}
