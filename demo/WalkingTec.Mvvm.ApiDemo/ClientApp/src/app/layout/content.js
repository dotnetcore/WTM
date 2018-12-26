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
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Dropdown, Icon, Layout, Menu, Tabs } from 'antd';
import lodash from 'lodash';
import { action, observable } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import ReactDOM from 'react-dom';
import { renderRoutes } from 'react-router-config';
import { Link } from 'react-router-dom';
import Store from 'store/index';
import Rx from 'rxjs';
var Header = Layout.Header, Content = Layout.Content, Sider = Layout.Sider;
var TabPane = Tabs.TabPane;
/**
 * 历史tabs 状态
 */
var History = /** @class */ (function () {
    function History() {
        this.activeKey = "";
        this.TabPane = [];
    }
    /**
     * 添加标签
     * @param props
     */
    History.prototype.onPush = function (props) {
        var history = props.history, location = props.location;
        var pathname = location.pathname;
        var title = '';
        if (pathname == "/") {
            title = 'Home';
        }
        if (lodash.some(this.TabPane, { key: pathname })) {
            this.activeKey = pathname;
        }
        else {
            Store.Meun.subMenu.filter(function (x) {
                if (x.Path == pathname) {
                    title = x.Name;
                }
                else {
                    x.Children.filter(function (y) {
                        if (y.Path == pathname) {
                            title = y.Name;
                        }
                    });
                }
            });
            this.activeKey = pathname;
            this.TabPane.push({ key: pathname, title: title || pathname });
        }
    };
    /**
     * 删除标签,返回一个可以重定向标签
     * @param pathname
     */
    History.prototype.onDelete = function (pathname) {
        if (pathname == "/") {
            return;
        }
        var TabPane = this.TabPane.slice();
        var index = lodash.findIndex(TabPane, { key: pathname });
        lodash.remove(TabPane, function (x) { return x.key == pathname; });
        var length = TabPane.length;
        this.TabPane = TabPane;
        var activeKey = null;
        if (length == 0) {
            activeKey = "/";
        }
        else if (length > index) {
            activeKey = TabPane[index].key;
        }
        else {
            activeKey = TabPane[index - 1].key;
        }
        this.activeKey = pathname;
        return activeKey;
    };
    __decorate([
        observable
    ], History.prototype, "activeKey", void 0);
    __decorate([
        observable
    ], History.prototype, "TabPane", void 0);
    __decorate([
        action.bound
    ], History.prototype, "onPush", null);
    __decorate([
        action.bound
    ], History.prototype, "onDelete", null);
    return History;
}());
var HistoryStore = new History();
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        // shouldComponentUpdate() {
        //   return false
        // }
        _this.minHeight = 0;
        _this.renderRoutes = renderRoutes(_this.props.route.routes);
        return _this;
    }
    App.prototype.setHeight = function () {
        if (this.body) {
            var content = this.body.querySelector(".app-animate-content");
            if (!content) {
                content = this.body.querySelector(".antd-pro-exception-exception");
            }
            if (content) {
                content.style.minHeight = (this.body.offsetHeight - 80) + "px";
            }
        }
    };
    App.prototype.componentDidMount = function () {
        var _this = this;
        this.setHeight();
        HistoryStore.onPush(this.props);
        this.resize = Rx.Observable.fromEvent(window, "resize").debounceTime(800).subscribe(function (e) {
            var scrollTop = _this.body.scrollTop;
            _this.body.scrollBy(0, -scrollTop);
            // setTimeout(() => {
            //   this.body.scrollBy(0, scrollTop) 
            // },100);
        });
    };
    App.prototype.componentWillUnmount = function () {
        this.resize.unsubscribe();
    };
    App.prototype.componentDidUpdate = function () {
        this.setHeight();
        HistoryStore.onPush(this.props);
    };
    App.prototype.render = function () {
        var _this = this;
        return (React.createElement(Layout, { className: "app-layout-body", ref: function (e) { return _this.body = ReactDOM.findDOMNode(e); } },
            React.createElement(Content, { className: "app-layout-content" }, this.renderRoutes)));
    };
    return App;
}(React.Component));
export default App;
var HistoryTabs = /** @class */ (function (_super) {
    __extends(HistoryTabs, _super);
    function HistoryTabs() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    HistoryTabs.prototype.onChange = function (e) {
        this.props.history.replace(e);
    };
    HistoryTabs.prototype.onEdit = function (e) {
        this.props.history.replace(HistoryStore.onDelete(e));
    };
    HistoryTabs.prototype.renderOverlay = function () {
        return HistoryStore.TabPane.map(function (pane) { return React.createElement(Menu.Item, { key: pane.key },
            React.createElement(Link, { to: pane.key }, pane.title)); });
    };
    HistoryTabs.prototype.render = function () {
        if (HistoryStore.TabPane.length <= 0) {
            return null;
        }
        return (React.createElement("div", { className: "app-history-tabs" },
            React.createElement("div", null,
                React.createElement(Tabs, { onChange: this.onChange.bind(this), activeKey: HistoryStore.activeKey, type: "editable-card", hideAdd: true, onEdit: this.onEdit.bind(this) }, HistoryStore.TabPane.map(function (pane) { return React.createElement(TabPane, { tab: pane.title, key: pane.key }); }))),
            React.createElement("div", { className: "tabs-menu" },
                React.createElement(Dropdown, { overlay: React.createElement(Menu, null, this.renderOverlay()), placement: "bottomLeft" },
                    React.createElement(Icon, { type: "bars" })))));
    };
    HistoryTabs = __decorate([
        observer
    ], HistoryTabs);
    return HistoryTabs;
}(React.Component));
//# sourceMappingURL=content.js.map