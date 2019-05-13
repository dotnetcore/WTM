/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-07-24 05:02:33
 * @modify date 2018-07-24 05:02:33
 * @desc [description]
 */
import { LocaleProvider, Skeleton } from 'antd';
import zhCN from 'antd/lib/locale-provider/zh_CN';
import Exception from 'components/other/Exception';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import Pages from 'pages/index';
import Animate from 'rc-animate';
import * as React from 'react';
import Loadable from 'react-loadable';
import { renderRoutes, RouteConfig } from 'react-router-config';
import { BrowserRouter } from 'react-router-dom';
import Store from 'store/index';
import Layout from "./layout/default/index";
import External from "./pages/external";
import Home from "./pages/home";
import Login from "./pages/login";
import System from "./pages/system";
@observer
class Entrance extends React.Component<any, any> {
    componentDidMount() {
        // console.clear()
        document.title = globalConfig.default.title;
        // console.log('aaaaa')
    }
    render() {
        if (Store.User.loding) {
            return <div></div>
        }
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
    public routes: RouteConfig[] = [
        // {
        //     // 外部页面
        //     path: "/mainExternal/:url",
        //     exact: true,
        //     component: this.createCSSTransition(External)
        // },
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
                    // 外部页面
                    path: "/external/:url",
                    exact: true,
                    component: this.createCSSTransition(External)
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
            return <Skeleton paragraph={{ rows: 10 }} />
        } else {
            return <div></div>;
        }
    };
    /**
     * 
     * @param Component 
     */
    Loadable(Component) {
        const loadable = Loadable({ loader: Component, loading: (props) => this.Loading(props) });
        return this.createCSSTransition(loadable);
    };
    /**
     *  过渡动画
     * @param Component 
     * @param classNames 
     */
    createCSSTransition(Component: any, classNames = "fade") {
        return class extends React.Component<any, any> {
            shouldComponentUpdate() {
                return !globalConfig.tabsPage
            }
            render() {
                return <Animate transitionName={classNames}
                    transitionAppear={true} component="" >
                    <div className="app-animate-content" key="app-animate-content" style={...this.props.style} >
                        <Component {...this.props} />
                    </div>
                </Animate  >
            }
        }
    };
    render() {
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