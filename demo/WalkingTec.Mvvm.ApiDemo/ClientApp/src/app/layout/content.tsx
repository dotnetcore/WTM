
import { Dropdown, Icon, Layout, Menu, Tabs } from 'antd';
import lodash from 'lodash';
import { action, observable } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import ReactDOM from 'react-dom';
import { renderRoutes } from 'react-router-config';
import { Link } from 'react-router-dom';
import Rx, { Subscription } from 'rxjs';
import Store from 'store/index';
const { Header, Content, Sider } = Layout;
const TabPane = Tabs.TabPane;
/**
 * 历史tabs 状态
 */
class History {
  constructor() {

  }
  @observable activeKey = "";
  @observable TabPane: { title: string, key: string }[] = [];
  /**
   * 添加标签
   * @param props 
   */
  @action.bound
  onPush(props) {
    const { history, location } = props;
    const { pathname } = location;
    let title = '';
    if (pathname == "/") {
      title = 'Home'
    }
    if (lodash.some(this.TabPane, { key: pathname })) {
      this.activeKey = pathname;
    } else {
      Store.Meun.subMenu.filter(x => {
        if (x.Path == pathname) {
          title = x.Name;
        } else {
          x.Children.filter(y => {
            if (y.Path == pathname) {
              title = y.Name;
            }
          })
        }
      })
      this.activeKey = pathname;
      this.TabPane.push({ key: pathname, title: title || pathname });
    }
  }
  /**
   * 删除标签,返回一个可以重定向标签
   * @param pathname 
   */
  @action.bound
  onDelete(pathname) {
    if (pathname == "/") {
      return
    }
    const TabPane = this.TabPane.slice();
    const index = lodash.findIndex(TabPane, { key: pathname });
    lodash.remove(TabPane, x => x.key == pathname);
    const length = TabPane.length;
    this.TabPane = TabPane;
    let activeKey = null;
    if (length == 0) {
      activeKey = "/"
    } else if (length > index) {
      activeKey = TabPane[index].key;
    } else {
      activeKey = TabPane[index - 1].key;
    }
    this.activeKey = pathname;
    return activeKey
  }
}
const HistoryStore = new History();

export default class App extends React.Component<any, any> {
  // shouldComponentUpdate() {
  //   return false
  // }
  minHeight = 0;
  body: HTMLDivElement;
  setHeight() {
    if (this.body) {
      let content: HTMLDivElement = this.body.querySelector(".app-animate-content");
      if (!content) {
        content = this.body.querySelector(".antd-pro-exception-exception");

      }
      if (content) {
        content.style.minHeight = (this.body.offsetHeight - 20) + "px";
      }
    }
  }
  resize: Subscription;
  componentDidMount() {
    this.setHeight();
    // HistoryStore.onPush(this.props);
    // this.resize = Rx.Observable.fromEvent(window, "resize").debounceTime(800).subscribe(e => {
    //   const scrollTop = this.body.scrollTop;
    //   this.body.scrollBy(0, -scrollTop)
    //   // setTimeout(() => {
    //   //   this.body.scrollBy(0, scrollTop) 
    //   // },100);
    // });
  }
  componentWillUnmount() {
    // this.resize.unsubscribe();
  }
  componentDidUpdate() {
    this.setHeight();
    // HistoryStore.onPush(this.props);
  }
  renderRoutes = renderRoutes(this.props.route.routes);
  render() {
    return (
      <Layout className="app-layout-body" ref={e => this.body = ReactDOM.findDOMNode(e) as any}>
        <Content className="app-layout-content" >
          {/* <Affix  target={() => this.body}>
            <HistoryTabs {...this.props} target={this.body} />
          </Affix> */}
          {this.renderRoutes}
        </Content>
      </Layout>
    );

  }
}
@observer
class HistoryTabs extends React.Component<any, any> {
  onChange(e) {
    this.props.history.replace(e);
  }
  onEdit(e) {
    this.props.history.replace(HistoryStore.onDelete(e));
  }
  renderOverlay() {
    return HistoryStore.TabPane.map(pane => <Menu.Item key={pane.key}>
      <Link to={pane.key}>{pane.title}</Link>
    </Menu.Item>)
  }
  render() {
    if (HistoryStore.TabPane.length <= 0) {
      return null
    }
    return (
      <div className="app-history-tabs">
        <div>
          <Tabs
            onChange={this.onChange.bind(this)}
            activeKey={HistoryStore.activeKey}
            type="editable-card"
            hideAdd
            onEdit={this.onEdit.bind(this)}
          >
            {HistoryStore.TabPane.map(pane => <TabPane tab={pane.title} key={pane.key} />)}
          </Tabs>
        </div>
        <div className="tabs-menu">
          <Dropdown overlay={<Menu>
            {this.renderOverlay()}
          </Menu>} placement="bottomLeft">
            <Icon type="bars" />
          </Dropdown>
        </div>
      </div>
    );
  }
}