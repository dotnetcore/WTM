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
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spread = (this && this.__spread) || function () {
    for (var ar = [], i = 0; i < arguments.length; i++) ar = ar.concat(__read(arguments[i]));
    return ar;
};
/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-07-24 05:02:33
 * @modify date 2018-07-24 05:02:33
 * @desc [description]
 */
import Exception from 'ant-design-pro/lib/Exception';
import { LocaleProvider, Skeleton } from 'antd';
import zhCN from 'antd/lib/locale-provider/zh_CN';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import Pages from 'pages/index';
import Animate from 'rc-animate';
import * as React from 'react';
import Loadable from 'react-loadable';
import { renderRoutes } from 'react-router-config';
import { BrowserRouter } from 'react-router-dom';
import Store from 'store/index';
import layout from "./layout/index";
import Home from "./pages/home";
import Login from "./pages/login";
import System from "./pages/system";
var RootRoutes = /** @class */ (function (_super) {
    __extends(RootRoutes, _super);
    function RootRoutes() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        /**
         * 路由列表
         */
        _this.routes = [
            {
                /**
                 * 主页布局
                 */
                path: "/",
                component: layout,
                //  业务路由
                routes: __spread([
                    {
                        path: "/",
                        exact: true,
                        component: _this.createCSSTransition(Home)
                    },
                    {
                        path: "/system",
                        exact: true,
                        component: _this.createCSSTransition(System)
                    }
                ], _this.initRouters(), [
                    // 404  首页
                    {
                        component: _this.createCSSTransition(NoMatch)
                    }
                ])
            }
        ];
        // 组件加载动画
        _this.Loading = function (props) {
            if (props.error) {
                return React.createElement("div", null,
                    "Error! ",
                    props.error);
            }
            else if (props.timedOut) {
                return React.createElement("div", null, "Taking a long time...");
            }
            else if (props.pastDelay) {
                return React.createElement(React.Fragment, null,
                    React.createElement(Skeleton, { active: true }),
                    React.createElement(Skeleton, { active: true }),
                    React.createElement(Skeleton, { active: true }),
                    React.createElement(Skeleton, { active: true }),
                    React.createElement(Skeleton, { active: true }));
            }
            else {
                return React.createElement("div", null);
            }
        };
        return _this;
    }
    /**
     * 初始化路由数据
     */
    RootRoutes.prototype.initRouters = function () {
        var _this = this;
        return lodash.map(Pages, function (component, key) {
            return {
                "path": "/" + key,
                "component": _this.Loadable(component)
            };
        });
    };
    /**
     *
     * @param Component 组件
     * @param Animate 路由动画
     * @param Loading 组件加载动画
     * @param cssTranParams 路由动画参数
     */
    RootRoutes.prototype.Loadable = function (Component, Animate, Loading, cssTranParams) {
        var _this = this;
        if (Animate === void 0) { Animate = true; }
        if (Loading === void 0) { Loading = this.Loading; }
        if (cssTranParams === void 0) { cssTranParams = { content: true, classNames: "fade" }; }
        if (!Loading) {
            Loading = function (props) { return _this.Loading(props); };
        }
        var loadable = Loadable({ loader: Component, loading: Loading });
        if (Animate) {
            return this.createCSSTransition(loadable, cssTranParams.content, cssTranParams.classNames);
        }
        return loadable;
    };
    ;
    /**
     * 过渡动画
     * @param Component 组件
     * @param content
     * @param classNames 动画
     */
    RootRoutes.prototype.createCSSTransition = function (Component, content, classNames) {
        if (content === void 0) { content = true; }
        if (classNames === void 0) { classNames = "fade"; }
        return /** @class */ (function (_super) {
            __extends(class_1, _super);
            function class_1() {
                var _this = _super !== null && _super.apply(this, arguments) || this;
                _this.state = {
                    error: null,
                    errorInfo: null
                };
                return _this;
            }
            class_1.prototype.componentDidCatch = function (error, info) {
                this.setState({
                    error: error,
                    errorInfo: info
                });
            };
            class_1.prototype.componentDidMount = function () {
            };
            class_1.prototype.render = function () {
                var location = this.props.location;
                // 组件出错
                if (this.state.errorInfo) {
                    return (React.createElement(Exception, { type: "500", desc: React.createElement("div", null,
                            React.createElement("h2", null, "\u7EC4\u4EF6\u51FA\u9519~"),
                            React.createElement("details", null,
                                this.state.error && this.state.error.toString(),
                                React.createElement("br", null),
                                this.state.errorInfo.componentStack)) }));
                }
                if (Component == NoMatch) {
                    return React.createElement(Animate, { transitionName: classNames, transitionAppear: true, component: "", key: Component.name },
                        React.createElement(NoMatch, __assign({}, this.props)));
                }
                // 认证通过
                if (Store.Authorize.onPassageway(this.props)) {
                    return (React.createElement(Animate, { transitionName: classNames, transitionAppear: true, component: "", key: Component.name },
                        React.createElement("div", { className: "app-animate-content", key: "app-animate-content" },
                            React.createElement(Component, __assign({}, this.props)))));
                }
                return React.createElement(Animate, { transitionName: classNames, transitionAppear: true, component: "", key: Component.name },
                    React.createElement(Exception, { type: "404", desc: React.createElement("h3", null,
                            "\u65E0\u6743\u9650\u8BBF\u95EE ",
                            this.props.location.pathname,
                            React.createElement("span", null, "\u8BA4\u8BC1\u4F4D\u7F6E\uFF1Astore/system/authorize.ts"),
                            React.createElement("code", null, location.pathname)) }));
            };
            return class_1;
        }(React.Component));
    };
    ;
    /**
     * 根据用户是否登陆渲染主页面或者 登陆界面
     */
    RootRoutes.prototype.renderApp = function () {
        if (Store.User.isLogin) {
            console.log("-----------路由列表-----------", lodash.find(this.routes, function (x) { return x.path == "/"; }).routes);
            return React.createElement(LocaleProvider, { locale: zhCN },
                React.createElement(React.Fragment, null, renderRoutes(this.routes)));
        }
        return React.createElement(Login, null);
    };
    RootRoutes.prototype.render = function () {
        // react-dom.development.js:492 Warning: Provider: It is not recommended to assign props directly to state because updates to props won't be reflected in state. In most cases, it is better to use props directly.
        return (
        // <Provider
        //     {...store}
        // >
        React.createElement(BrowserRouter, null, this.renderApp())
        // </Provider>
        );
    };
    RootRoutes = __decorate([
        observer
    ], RootRoutes);
    return RootRoutes;
}(React.Component));
export default RootRoutes;
var NoMatch = /** @class */ (function (_super) {
    __extends(NoMatch, _super);
    function NoMatch() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    NoMatch.prototype.render = function () {
        return React.createElement(Exception, { type: "404", desc: React.createElement("h3", null,
                "\u65E0\u6CD5\u5339\u914D ",
                React.createElement("code", null, this.props.location.pathname)) });
    };
    return NoMatch;
}(React.Component));
//# sourceMappingURL=index.js.map