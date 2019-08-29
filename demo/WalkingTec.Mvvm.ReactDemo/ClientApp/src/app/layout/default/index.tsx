
import { Layout, Skeleton } from 'antd';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { action, computed, observable } from 'mobx';
import Animate from 'rc-animate';
import * as React from 'react';
import ContentComponent from './content';
import HeaderComponent from './header';
import SiderComponent from './sider';
import './style.less';
/**
 * 默认布局
 */
export default class DefaultLayout extends React.Component<any, any> {
  render() {
    return (
        <Animate transitionName='fade'
          transitionAppear={true} component=""  >
          <Layout className="app-layout-root">
            <SiderComponent {...this.props} />
            <HeaderComponent {...this.props} />
            <ContentComponent {...this.props}  />
          </Layout>
        </Animate  >
    );
  }
}

