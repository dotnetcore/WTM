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
import Layout from "./layout/index";
import Home from "./pages/home";
import Login from "./pages/login";
import System from "./pages/system";
@observer
class Entrance extends React.Component<any, any> {
    componentDidMount() {
        // console.clear()
    }
    render() {
        // 用户登陆菜单加载完成进入主界面
        if (Store.User.isLogin) {
            return <Layout {...this.props} />
        }
        return <Login {...this.props} />

    }
}
@observer
export default class RootRoutes extends React.Component<any, any> {
    /**
     * 路由列表
     */
    public routes: any[] = [
        {
            /**
             * 主页布局 
             */
            path: "/",
            component: Entrance,
            //  业务路由
            routes: [
                {
                    path: "/",
                    exact: true,
                    component: this.createCSSTransition(Home)
                },
                {
                    path: "/system",
                    exact: true,
                    component: this.createCSSTransition(System)
                },
                ...this.initRouters(),
                // 404  首页
                {
                    component: this.createCSSTransition(NoMatch)
                }
            ]
        }
    ];
    /**
     * 初始化路由数据
     */
    initRouters() {
        return lodash.map(Pages, (component: any, key) => {
            if (typeof component === "object") {
                return {
                    "path": component.path,
                    "component": this.Loadable(component.component)
                };
            }
            return {
                "path": "/" + key,
                "component": this.Loadable(component)
            };
        })
    }

    // 组件加载动画
    Loading = (props) => {
        if (props.error) {
            return <div>Error! {props.error}</div>;
        } else if (props.timedOut) {
            return <div>Taking a long time...</div>;
        } else if (props.pastDelay) {
            return <>
                <Skeleton active />
                <Skeleton active />
                <Skeleton active />
                <Skeleton active />
                <Skeleton active />
            </>
        } else {
            return <div></div>;
        }
    };
    /**
     * 
     * @param Component 组件
     * @param Animate 路由动画
     * @param Loading 组件加载动画
     * @param cssTranParams 路由动画参数
     */
    Loadable(Component, Animate = true, Loading = this.Loading, cssTranParams = { content: true, classNames: "fade" }) {
        if (!Loading) {
            Loading = (props) => this.Loading(props);
        }
        const loadable = Loadable({ loader: Component, loading: Loading });
        if (Animate) {
            return this.createCSSTransition(loadable, cssTranParams.content, cssTranParams.classNames);
        }
        return loadable;
    };
    /**
     * 过渡动画
     * @param Component 组件
     * @param content 
     * @param classNames 动画
     */
    createCSSTransition(Component: any, content = true, classNames = "fade") {
        return class extends React.Component<any, any>{
            state = {
                error: null,
                errorInfo: null
            };
            content: HTMLDivElement;
            componentDidCatch(error, info) {
                this.setState({
                    error: error,
                    errorInfo: info
                })
            }
            componentDidMount() {

            }
            render() {
                const { location } = this.props;
                // 组件出错
                if (this.state.errorInfo) {
                    return (
                        <Exception type="500" desc={<div>
                            <h2>组件出错~</h2>
                            <details >
                                {this.state.error && this.state.error.toString()}
                                <br />
                                {this.state.errorInfo.componentStack}
                            </details>
                        </div>} />
                    );
                }
                if (Component == NoMatch) {
                    return <Animate transitionName={classNames}
                        transitionAppear={true} component="" key={Component.name} ><NoMatch {...this.props} />
                    </Animate>
                }
                // 认证通过
                if (Store.Authorize.onPassageway(this.props)) {
                    return (
                        // <Animate transitionName={classNames}
                        //     transitionAppear={true} component="" key={Component.name} >
                        <div className="app-animate-content" key="app-animate-content" >
                            <Component  {...this.props} />
                        </div>
                        // </Animate  >
                    );
                }
                return <Animate transitionName={classNames}
                    transitionAppear={true} component="" key={Component.name} >
                    <Exception type="404" desc={<h3>无权限访问 {this.props.location.pathname}
                        <span>认证位置：store/system/authorize.ts</span>
                        <code>{location.pathname}</code>
                    </h3>} />
                </Animate  >
            }
        }
    };
    render() {
        // react-dom.development.js:492 Warning: Provider: It is not recommended to assign props directly to state because updates to props won't be reflected in state. In most cases, it is better to use props directly.
        return (
            <LocaleProvider locale={zhCN}>
                <BrowserRouter >
                    {renderRoutes(this.routes)}
                </BrowserRouter>
            </LocaleProvider>
        );
    }

}
class NoMatch extends React.Component<any, any> {
    render() {
        return <Exception type="404" desc={<h3>无法匹配 <code>{this.props.location.pathname}</code></h3>} />
    }
}