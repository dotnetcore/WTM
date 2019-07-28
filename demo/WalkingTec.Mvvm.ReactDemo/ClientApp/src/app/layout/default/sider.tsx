
import { Icon, Menu } from 'antd';
import GlobalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import { Link } from 'react-router-dom';
import Store from 'store/index';
import lodash from 'lodash';
import RequestFiles from 'utils/RequestFiles';
import { MenuProps } from 'antd/lib/menu';

const { SubMenu } = Menu;
@observer
export default class App extends React.Component<any, any> {
  render() {
    if (GlobalConfig.menuMode === "horizontal") {
      return null
    }
    let width = Store.Meun.collapsedWidth;

    return (
      <>
        <div className="app-layout-sider" style={{ width, minWidth: width }} >
          <AppLogo  {...this.props} />
          <AppMenu {...this.props} />
        </div>
        <div className="app-layout-sider-stance" style={{ width, minWidth: width }}>
        </div>
      </>
    );
  }
}
export class AppLogo extends React.Component<any, any> {
  render() {
    let title = GlobalConfig.default.title;
    if (Store.Meun.collapsed) {
      title = "";
    }
    return (
      <div className="app-layout-logo" >
        <img src={GlobalConfig.default.logo} /><span>{title}</span>
      </div>
    );
  }
}

export class AppMenu extends React.Component<{ mode?: "horizontal" | "inline", [key: string]: any }, any> {
  renderLink(menu) {
    if (menu.Url) {
      return <Link to={menu.Url}>{renderIconTitle(menu)}</Link>
    }
    return renderIconTitle(menu)
  }
  renderMenu(menus, index) {
    return menus.Children.map((x, i) => {
      return <Menu.Item key={x.Id} >
        {this.renderLink(x)}
      </Menu.Item>
    })
  }
  runderSubMenu() {
    return Store.Meun.subMenu.map((menu, index) => {
      if (menu.Children && menu.Children.length > 0) {
        return <SubMenu key={menu.Id} title={<span>{renderIconTitle(menu)}</span>}>
          {
            this.renderMenu(menu, index)
          }
        </SubMenu>
      }
      return <Menu.Item key={menu.Id} >
        {this.renderLink(menu)}
      </Menu.Item>
    })
  }
  render() {
    const props: MenuProps = {
      theme: "dark",
      mode: this.props.mode || 'inline',
      selectedKeys: [],
      defaultOpenKeys: [],
      style: { borderRight: 0 },
      // inlineCollapsed: Store.Meun.collapsed,
    }
    const find = lodash.find(Store.Meun.ParallelMenu, ["Url", this.props.location.pathname]);
    props.selectedKeys.push(lodash.get(find, 'Id', '/'));
    if (props.mode === "inline") {
      props.defaultOpenKeys.push(lodash.get(find, 'ParentId', ''));
      props.style.width = Store.Meun.collapsedWidth;
      props.inlineCollapsed = Store.Meun.collapsed
    }
    let width = Store.Meun.collapsedWidth;
    return (
      <Menu
        {...props}
      >
        <Menu.Item key="/">
          <Link to="/">
            <Icon type="home" /><span>首页</span>
          </Link>
        </Menu.Item>
        {this.runderSubMenu()}
      </Menu>
    );
  }
}
export function renderIconTitle(menu) {
  let icon = null;
  if (menu.Icon && menu.Icon.length === 36) {
    icon = <img className='ant-menu-item-img' src={RequestFiles.onFileDownload(menu.Icon)} alt="" />
  } else {
    icon = <Icon type={menu.Icon || 'appstore'} />
  }
  return <>{icon}<span>{menu.Text}</span> </>
}