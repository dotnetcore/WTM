
import { Layout, Tabs } from 'antd';
import globalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import { renderRoutes, matchRoutes } from 'react-router-config';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { observable, runInAction, action } from 'mobx';
import lodash from 'lodash';
import subMenu from 'subMenu.json';

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
        {this.renderRoutes}
      </Content>
    );
  }
}
/**
 * tabs 页面布局
 */
@observer
class TabsPages extends React.Component<any, any> {
  TabsPagesStore = new TabsPagesStore();
  componentDidMount() {
    this.TabsPagesStore.pushTabPane(this.props.location.pathname);
  }
  componentWillUnmount() {
    this.TabsPagesStore.componentWillUnmount();
  }
  componentDidUpdate() {
    this.TabsPagesStore.pushTabPane(this.props.location.pathname);
  }
  getRoutes(pathname) {
    const router = matchRoutes(this.props.route.routes, pathname);
    return {
      component: router[0].route.component,
      match: router[0].match
    }
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
      <Content className="app-layout-content app-layout-content-tabs">
        <Tabs
          activeKey={this.props.location.pathname}
          // size="small"
          className="app-layout-tabs"
          tabPosition={lodash.get(globalConfig, "tabPosition", "top") as any}
          onChange={this.onChange.bind(this)}
          animated={false}
          type="editable-card"
          onEdit={this.onEdit.bind(this)}
        >
          {tabPane.map(item => {
            const router = this.getRoutes(item.pathname);
            return <Tabs.TabPane tab={item.title} key={item.pathname} style={{ height: height }} closable={item.closable}>
              {React.createElement(router.component, { ...this.props, match: router.match } as any)}
            </Tabs.TabPane>
          })}
        </Tabs>
      </Content>
    );
  }
}
class TabsPagesStore {
  constructor() {

  }
  componentWillUnmount() {
    this.resize.unsubscribe();
  }
  @observable height = this.getHeight();
  @observable tabPane = [{
    title: "首页",
    pathname: "/",
    closable: false,
  }];
  @action
  pushTabPane(pathname) {
    if (lodash.some(this.tabPane, item => lodash.eq(item.pathname, pathname))) return;
    const title = lodash.get(lodash.find(subMenu, ['Path', pathname]), 'Name', "Null")
    this.tabPane.push({
      title: title,
      pathname,
      closable: true
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
    return window.innerHeight - (lodash.some(["top", "bottom"], data => lodash.eq(data, globalConfig.tabPosition)) ? 90 : 52);
  }
  resize = fromEvent(window, "resize").pipe(debounceTime(300)).subscribe(e => {
    const height = this.getHeight()
    if (this.height != height) {
      runInAction(() => this.height = height)
    }
  });
}