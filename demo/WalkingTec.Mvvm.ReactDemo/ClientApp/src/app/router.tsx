import { Skeleton } from 'antd';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import Pages from 'pages/index';
import Animate from 'rc-animate';
import React from 'react';
import { renderRoutes, RouteConfig } from 'react-router-config';
import Store from 'store';
import Layout from "./layout/default/index";
import Demo from "./pages/demo";
import External from "./pages/external";
import Home from "./pages/home";
import Login from "./pages/login";
import System from "./pages/system";
/**
 *  react-router-config 配置文档  https://github.com/ReactTraining/react-router/tree/master/packages/react-router-config
 *  react-router 配置文档 https://github.com/ReactTraining/react-router/blob/master/packages/react-router/docs/api/Route.md
 *  path 配置文档 https://github.com/pillarjs/path-to-regexp/tree/v1.7.0
 */
const router: RouteConfig[] = [
    {  /**
        * 主页布局 
        */
        path: "/",
        component: AuthentRouter(Layout),
        //  业务路由
        routes: [
            {
                path: "/",
                exact: true,
                component: createCSSTransition(Home)
            },
            {
                path: "/demo/:path?",
                exact: true,
                component: createCSSTransition(Demo),
            },
            {
                // 外部页面
                path: "/external/:url",
                exact: true,
                component: createCSSTransition(External)
            },
            {
                path: "/system",
                exact: true,
                component: createCSSTransition(System)
            },
            ...initRouters(),
            // 404  首页
            {
                component: createCSSTransition(External)
            }
        ]
    }
]
/**
 * 路由认证 
 * @param Component 
 */
function AuthentRouter(Component: React.ComponentClass) {
    return (props) => (
        <AuthentComponent {...props}>
            <Component {...props} />
        </AuthentComponent>
    )
}
@observer
class AuthentComponent extends React.Component<any> {
    componentDidMount() {
    }
    render() {
        if (Store.User.loding) {
            return <div style={{ height: '100vh', overflow: "hidden", padding: '60px 30px' }}>
                <Skeleton active paragraph={{ rows: 20 }} />
            </div>
        }
        // 用户登陆菜单加载完成进入主界面
        if (Store.User.isLogin) {
            return this.props.children
        }
        return <Login {...this.props} />
    }
}
/**
* 初始化路由数据
*/
function initRouters() {
    return lodash.map(Pages, (component: any, key) => {
        if (typeof component === "object") {
            return {
                "path": component.path,
                "component": createCSSTransition(component.component)
            };
        }
        return {
            "path": "/" + key,
            "component": createCSSTransition(component)
        };
    })
}
/**
 *  过渡动画
 * @param Component 
 * @param classNames 
 */
function createCSSTransition(Component: any, classNames = "fade") {
    return class extends React.Component<any, any> {
        shouldComponentUpdate() {
            return !globalConfig.tabsPage
        }
        componentDidMount() {
            // console.log("TCL: extends -> componentDidMount -> this.props", this.props)
        }
        render() {
            return <Animate
                transitionName={classNames}
                transitionAppear component="" >
                <div className="app-animate-content" key="app-animate-content" >
                    <Component {...this.props} />
                </div>
            </Animate  >
        }
    }
};
export default renderRoutes(router) 
