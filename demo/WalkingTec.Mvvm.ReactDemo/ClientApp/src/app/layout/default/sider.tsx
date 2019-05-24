
import { Icon, Menu } from 'antd';
import GlobalConfig from 'global.config';
import { observer } from 'mobx-react';
import * as React from 'react';
import { Link } from 'react-router-dom';
import Store from 'store/index';
import lodash from 'lodash';
import RequestFiles from 'utils/RequestFiles';

const { SubMenu } = Menu;
@observer
export default class App extends React.Component<any, any> {
  renderIcon(menu) {
    let icon = null;
    if (menu.Icon && menu.Icon.length === 36) {
      icon = <img className='ant-menu-item-img' src={RequestFiles.onFileDownload(menu.Icon)} alt="" />
    } else {
      icon = <Icon type={menu.Icon || 'appstore'} />
    }
    return <>{icon}<span>{menu.Text}</span> </>
  }
  renderLink(menu) {
    if (menu.Url) {
      return <Link to={menu.Url}>{this.renderIcon(menu)}</Link>
    }
    return this.renderIcon(menu)
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
        return <SubMenu key={menu.Id} title={<span>{this.renderIcon(menu)}</span>}>
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
    const config = {
      selectedKeys: [],
      defaultOpenKeys: []
    };
    const find = lodash.find(Store.Meun.ParallelMenu, ["Url", this.props.location.pathname]);
    config.selectedKeys.push(lodash.get(find, 'Id', '/'));
    config.defaultOpenKeys.push(lodash.get(find, 'ParentId', ''));
    let width = this.props.LayoutStore.collapsedWidth;
    let title = GlobalConfig.default.title;
    if (this.props.LayoutStore.collapsed) {
      title = "";
    }
    return (
      <>
        <div className="app-layout-sider" style={{ width, minWidth: width }} >
          <div className="app-layout-logo" >
            <img src={GlobalConfig.default.logo} /><span>{title}</span>
          </div>
          <Menu
            theme="dark"
            mode="inline"
            {...config}
            style={{ borderRight: 0, width }}
            inlineCollapsed={this.props.LayoutStore.collapsed}
          >
            <Menu.Item key="/">
              <Link to="/">
                <Icon type="home" /><span>首页</span>
              </Link>
            </Menu.Item>
            {this.runderSubMenu()}
          </Menu>
        </div>
        <div className="app-layout-sider-stance" style={{ width, minWidth: width }}>
        </div>
      </>
    );
  }
}

