
import { Layout } from 'antd';
import * as React from 'react';
import Animate from 'rc-animate';
import ContentComponent from './content';
import HeaderComponent from './header';
import SiderComponent from './sider';
import './style.less';
export default class App extends React.Component<any, any> {
  render() {
    return (
      <Animate transitionName='fade'
        transitionAppear={true} component=""  >
        <Layout tagName="main" className="app-layout-root">
          <SiderComponent {...this.props} />
          <Layout tagName="main" style={{ overflow: "hidden" }}>
            <HeaderComponent {...this.props} />
            <ContentComponent {...this.props} />
          </Layout>
        </Layout>
      </Animate  >
    );
  }
}

