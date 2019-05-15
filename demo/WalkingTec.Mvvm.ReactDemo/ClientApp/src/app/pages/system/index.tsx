/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-10 05:07:39
 * @modify date 2018-09-10 05:07:39
 * @desc [description]
*/
import { Tabs } from 'antd';
import * as React from 'react';
import SubMenu from './subMenu';
import Authorize from './authorize';

const TabPane = Tabs.TabPane;
export default class IApp extends React.Component<any, any> {
    public render() {
        const TreeNodeConfig = {
            disableCheckbox: true,
            selectable: false
        }
        return (
            <Tabs
                defaultActiveKey="subMenu"
                tabPosition="left"
            >
                {/* <TabPane tab="基础设置" key="2">Content of tab 2</TabPane> */}
                <TabPane tab="菜单设置" key="subMenu">
                    <SubMenu />
                </TabPane>
                <TabPane tab="权限设置" key="authorize">
                    <Authorize />
                </TabPane>
            </Tabs>
        );
    }
}
