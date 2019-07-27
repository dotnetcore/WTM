
import { Layout, Tabs, Skeleton } from 'antd';
import globalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import { renderRoutes, matchRoutes } from 'react-router-config';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { observable, runInAction, action, toJS } from 'mobx';
import lodash from 'lodash';
import Store from 'store/index';
import { renderIconTitle } from './sider'
import { Help } from 'utils/Help';

const { Content } = Layout;
@observer
class Pages extends React.Component<any, any> {
  render() {
    if (globalConfig.tabsPage) {
      return <TabsPages {...this.props} />
    }
    return <SwitchPages {...this.props} />
  }
}
export default Pages
// class App extends React.Component<any, any> {
//   shouldComponentUpdate() {
//     return false
//   }
//   render() {
//     return <Pages {...this.props} />
//   }
// }


/**
 * 普通 页面 布局
 */
class SwitchPages extends React.Component<any, any> {
  componentDidMount() {
  }
  componentWillUnmount() {
  }
  componentDidUpdate() {
  }
  renderRoutes = renderRoutes(this.props.route.routes);
  render() {
    return (
      <Content className="app-layout-content" >
        <React.Suspense fallback={<Skeleton paragraph={{ rows: 20 }} />}>
          {this.renderRoutes}
        </React.Suspense>
      </Content>
    );
  }
}
/**
 * tabs 页面布局
 */
@observer
class TabsPages extends React.Component<any, any> {
  TabsPagesStore = new TabsPagesStore(this.props.route.routes);
  componentDidMount() {
    this.TabsPagesStore.pushTabPane(this.props.location.pathname);
  }
  componentWillUnmount() {
    this.TabsPagesStore.componentWillUnmount();
  }
  componentDidUpdate() {
    this.TabsPagesStore.pushTabPane(this.props.location.pathname);
    // console.log("TCL: TabsPages -> componentDidUpdate -> this.props", this.props)
  }
  onChange(event) {
    this.props.history.replace(event)
  }
  onEdit(event) {
    const path = this.TabsPagesStore.onClosable(event);
    if (lodash.eq(this.props.location.pathname, event))
      this.onChange(path)
  }
  render() {
    const tabPane = this.TabsPagesStore.tabPane;
    const height = this.TabsPagesStore.height;
    return (
      <Content className={`app-layout-content app-layout-content-tabs app-lockingTableRoll-${globalConfig.lockingTableRoll}`}>
        <Tabs
          activeKey={this.props.location.pathname}
          // size="small"
          className="app-layout-tabs"
          tabPosition={lodash.get(globalConfig, "tabPosition", "top") as any}
          onChange={this.onChange.bind(this)}
          animated={globalConfig.tabsAnimated}
          type="editable-card"
          onEdit={this.onEdit.bind(this)}
        >
          {tabPane.map(item => {
            const router = item.router;
            const props = { ...this.props, match: router.match };
            return <Tabs.TabPane
              tab={renderIconTitle({ Icon: item.icon, Text: item.title })}
              key={item.pathname}
              closable={item.closable}
              style={{ height: height }}>
              <React.Suspense fallback={<Skeleton paragraph={{ rows: 20 }} />}>
                {React.createElement(router.component, props)}
              </React.Suspense>
            </Tabs.TabPane>
          })}
        </Tabs>
      </Content>
    );
  }
}
class TabsPagesStore {
  constructor(private routes) {
  }
  componentWillUnmount() {
    this.resize.unsubscribe();
  }
  @observable height = this.getHeight();
  @observable tabPane = [{
    title: '首页',
    pathname: "/",
    closable: false,
    icon: "home",
    router: this.getRoutes("/")
  }];
  @action
  pushTabPane(pathname) {
    const router = this.getRoutes(pathname);
    if (lodash.some(this.tabPane, item => lodash.eq(item.pathname, pathname))) return;
    const menu = lodash.find(Store.Meun.ParallelMenu, ['Url', pathname]);
    const title = lodash.get(menu, 'Text', "Null")
    const icon = lodash.get(menu, 'Icon', "appstore")
    this.tabPane.push({
      title: title,//renderIconTitle(menu || { Text: "NULL", Icon: "appstore", Id: Help.GUID() }),
      pathname,
      closable: true,
      icon,
      router
    });
  }
  @action
  onClosable(pathname) {
    const index = lodash.findIndex(this.tabPane, ['pathname', pathname]);
    const path = lodash.get(this.tabPane, `[${index - 1}].pathname`, "/");
    lodash.remove(this.tabPane, ['pathname', pathname]);
    return path
  }
  getHeight() {
    return window.innerHeight - (lodash.some(["top", "bottom"], data => lodash.eq(data, globalConfig.tabPosition)) ? 90 : 50);
  }
  getRoutes(pathname) {
    const router = matchRoutes(this.routes, pathname);
    return {
      component: router[0].route.component,
      match: router[0].match
    }
  }
  resize = fromEvent(window, "resize").pipe(debounceTime(200)).subscribe(e => {
    const height = this.getHeight()
    if (this.height != height) {
      runInAction(() => this.height = height)
    }
  });
}