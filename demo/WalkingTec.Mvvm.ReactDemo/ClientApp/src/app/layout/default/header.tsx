
import { Col, Icon, Layout, Row } from 'antd';
import GlobalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import Store from 'store/index';
import SetUp from './setUp';
import { AppLogo, AppMenu } from './sider';
import UserMenu from './userMenu';
const { Header } = Layout;
export default class App extends React.Component<any, any> {
  shouldComponentUpdate() {
    return GlobalConfig.menuMode === "horizontal"
  }
  render() {
    return (
      <PageHeader {...this.props} />
    );
  }
}
@observer
class PageHeader extends React.Component<any, any> {
  render() {
    const isMenu = GlobalConfig.menuMode === "horizontal";
    return (
      <Header className="app-layout-header" style={isMenu ? {} : { marginLeft: Store.Meun.collapsedWidth }}>
        <Row type="flex">
          {isMenu && <Col >
            <AppLogo {...this.props} />
          </Col>}
          <Col >
            {isMenu ?
              <AppMenu mode="horizontal" {...this.props} />
              : <Icon onClick={() => { Store.Meun.onCollapsed() }} className="app-collapsed-trigger" type="menu-fold" theme="outlined" />}
          </Col>
          <Col style={{ textAlign: "right", flex: 1, overflow: "hidden" }}>
            <Row type="flex" justify="end" style={{ height: "100%" }}>
              <Col style={{ height: "100%", marginRight: 10 }}>
                <SetUp />
              </Col>
              <Col style={{ height: "100%" }}>
                <UserMenu {...this.props} />
              </Col>
            </Row>
          </Col>
        </Row>
      </Header>
    );
  }
}
