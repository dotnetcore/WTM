/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import Exception from 'components/other/Exception';
import lodash from 'lodash';
import React from 'react';
import PageStore from '../dataSource/index';
import Menu from './menu';
import User from './user';
import { Request } from 'utils/Request';
import globalConfig from 'global.config';

class Store {
    constructor() {

    }
    /**
     * 认证动作
     * @param url 
     */
    onAuthorizeActions(url) {
        return lodash.includes(User.Actions, lodash.toLower(url))
    }
    /**
     * 认证通道 
     * @param props  
     */
    onPassageway(props) {
        const { location } = props;
        // 查找用户菜单配置中是否有当前路径
        return lodash.some(Menu.ParallelMenu, ["Url", location.pathname])
    }
}
const AuthorizeStore = new Store();
export default AuthorizeStore
/**
 * 权限装饰器
 * @param PageParams 
 */
export function AuthorizeDecorator(PageParams: { PageStore: PageStore }) {
    return function <T extends { new(...args: any[]): React.Component<any, any> }>(constructor: T) {
        return class extends constructor {
            // constructor(props) {
            //     super(props);
            //     // PageParams.PageStore.defaultSearchParams = lodash.get(this.props, "defaultSearchParams", {});
            // }
            // // shouldComponentUpdate() {
            // //     return false
            // // }
            // componentWillMount() {

            //     // console.log(this.props)
            //     // 假设 所有 为 false
            //     // notification.info({ message: "假设有权限 2 秒后" });
            //     super.componentWillMount && super.componentWillMount()
            // }
            render(): any {
                if (AuthorizeStore.onPassageway(this.props)) {
                    return super.render();
                }
                return <Exception type="404" desc={<h3>无法匹配 <code>{this.props.location.pathname}</code></h3>} />
            }
        }
    }
}
/**
* url 类型
*/
declare type UrlKeyType = "search" | "details" | "insert" | "update" | "delete" | "import" | "export" | "exportIds" | "template"
/**
 * 认证动作
 * @param PageStore 页面 Store
 * @param UrlsKey 按钮对应 URL 权限 在 PageStore apis 中 未匹配 将 拿原始值 比对。
 */
export function onAuthorizeActions(PageStore: PageStore, UrlsKey: UrlKeyType | any) {
    if (globalConfig.development) {
        return true;
    }
    // 查找配置中的 值 
    const actionUrl = lodash.get(PageStore, `options.Apis.${UrlsKey}.url`);
    if (actionUrl) {
        return AuthorizeStore.onAuthorizeActions(Request.compatibleUrl(globalConfig.target, actionUrl));
    }
    // 未查找到。使用原始 值 比较
    else {
        return AuthorizeStore.onAuthorizeActions(UrlsKey);
    }
    // return lodash.get(PageStore, `Urls.${UrlsKey}.url`, GlobalConfig.development)// 开发 环境 默认返回 true
}
