var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Icon, Layout, Menu } from 'antd';
import { observer } from 'mobx-react';
import * as React from 'react';
import { Link } from 'react-router-dom';
// import routersConfig from '../routersConfig';
import Store from 'store/index';
var SubMenu = Menu.SubMenu;
var Header = Layout.Header, Content = Layout.Content, Sider = Layout.Sider;
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    App.prototype.renderLink = function (menu) {
        if (menu.Path) {
            return React.createElement(Link, { to: menu.Path },
                React.createElement(Icon, { type: menu.Icon || 'appstore' }),
                " ",
                React.createElement("span", null, menu.Name));
        }
        return React.createElement(React.Fragment, null,
            React.createElement(Icon, { type: menu.Icon || 'appstore' }),
            React.createElement("span", null, menu.Name),
            " ");
    };
    App.prototype.renderMenu = function (menus, index) {
        var _this = this;
        return menus.Children.map(function (x, i) {
            return React.createElement(Menu.Item, { key: x.Key, _key: x.Key }, _this.renderLink(x));
        });
    };
    App.prototype.runderSubMenu = function () {
        var _this = this;
        return Store.Meun.subMenu.map(function (menu, index) {
            if (menu.Children && menu.Children.length > 0) {
                return React.createElement(SubMenu, { key: menu.Key, title: React.createElement("span", null,
                        React.createElement(Icon, { type: menu.Icon }),
                        React.createElement("span", null, menu.Name)) }, _this.renderMenu(menu, index));
            }
            return React.createElement(Menu.Item, { key: menu.Key, _key: menu.Key }, _this.renderLink(menu));
        });
    };
    App.prototype.render = function () {
        var _this = this;
        var selectedKeys = "/", openKeys = "";
        Store.Meun.subMenu.filter(function (x) {
            if (_this.props.location.pathname == "/" && selectedKeys == "/") {
                return;
            }
            if (x.Path == _this.props.location.pathname) {
                selectedKeys = x.Key;
            }
            else {
                x.Children.filter(function (y) {
                    if (_this.props.location.pathname == "/" && selectedKeys == "/") {
                        return;
                    }
                    if (y.Path == _this.props.location.pathname) {
                        selectedKeys = y.Key;
                        openKeys = x.Key;
                    }
                });
            }
        });
        var config = {
            selectedKeys: [selectedKeys],
            defaultOpenKeys: [openKeys]
        };
        // if (openKeys == "") {
        //   delete config.openKeys;
        // }
        // console.log(selectedKeys, openKeys);
        var width = Store.Meun.collapsed ? 80 : 250;
        return (React.createElement("div", { className: "app-layout-sider", style: { width: width } },
            React.createElement("div", { className: "app-layout-logo" }, "Logo"),
            React.createElement(Menu, __assign({ theme: "dark", mode: "inline", defaultSelectedKeys: [selectedKeys] }, config, { style: { borderRight: 0, width: width }, inlineCollapsed: Store.Meun.collapsed }),
                React.createElement(Menu.Item, { key: "/" },
                    React.createElement(Link, { to: "/" },
                        React.createElement(Icon, { type: "home" }),
                        React.createElement("span", null, "\u9996\u9875"))),
                this.runderSubMenu())));
    };
    App = __decorate([
        observer
    ], App);
    return App;
}(React.Component));
export default App;
//# sourceMappingURL=sider.js.map