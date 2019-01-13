
import { Icon, Layout, Menu } from 'antd';
import { observer } from 'mobx-react';
import * as React from 'react';
import { Link } from 'react-router-dom';
// import routersConfig from '../routersConfig';
import Store from 'store/index';
import lodash from 'lodash';
const { SubMenu } = Menu;
const { Header, Content, Sider } = Layout;
@observer
export default class App extends React.Component<any, any> {
  renderLink(menu) {
    if (menu.Path) {
      return <Link to={menu.Path}><Icon type={menu.Icon || 'appstore'} /> <span>{menu.Name}</span></Link>
    }
    return <><Icon type={menu.Icon || 'appstore'} /><span>{menu.Name}</span> </>
  }
  renderMenu(menus, index) {
    return menus.Children.map((x, i) => {
      return <Menu.Item key={x.Key} >
        {this.renderLink(x)}
      </Menu.Item>
    })
  }
  runderSubMenu() {
    return Store.Meun.subMenu.map((menu, index) => {
      if (menu.Children && menu.Children.length > 0) {
        return <SubMenu key={menu.Key} title={<span><Icon type={menu.Icon} /><span>{menu.Name}</span></span>}>
          {
            this.renderMenu(menu, index)
          }
        </SubMenu>
      }
      return <Menu.Item key={menu.Key} >
        {this.renderLink(menu)}
      </Menu.Item>
    })
  }
  render() {
    let selectedKeys = "/", openKeys = "";
    Store.Meun.subMenu.filter(x => {
      if (this.props.location.pathname == "/" && selectedKeys == "/") {
        return
      }
      if (x.Path == this.props.location.pathname) {
        selectedKeys = x.Key;
      } else {
        x.Children.filter(y => {
          if (this.props.location.pathname == "/" && selectedKeys == "/") {
            return
          }
          if (y.Path == this.props.location.pathname) {
            selectedKeys = y.Key;
            openKeys = x.Key;
          }
        })
      }
    })
    let config = {
      selectedKeys: [selectedKeys],
      defaultOpenKeys: [openKeys]
    }
    // if (openKeys == "") {
    //   delete config.openKeys;
    // }
    // console.log(selectedKeys, openKeys);
    const width = Store.Meun.collapsed ? 80 : 250
    return (
      <div className="app-layout-sider" style={{ width, minWidth: width }} >
        <div className="app-layout-logo" >Logo</div>
        <Menu
          theme="dark"
          mode="inline"
          defaultSelectedKeys={[selectedKeys]}
          {...config}
          style={{ borderRight: 0, width }}
          inlineCollapsed={Store.Meun.collapsed}
        >
          <Menu.Item key="/">
            <Link to="/">
              <Icon type="home" /><span>首页</span>
            </Link>
          </Menu.Item>
          {/* <Menu.Item key="/user">
            <Link to="/user">
              <Icon type="home" /><span>个人中心</span>
            </Link>
          </Menu.Item> */}
          {this.runderSubMenu()}

        </Menu>
      </div>
    );
  }
}

