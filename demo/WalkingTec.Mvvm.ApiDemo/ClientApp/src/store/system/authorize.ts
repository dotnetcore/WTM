/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import { notification } from 'antd';
import GlobalConfig from 'global.config';
import lodash from 'lodash';
import PageStore from '../dataSource/index'
import User from './user';
import { runInAction } from 'mobx';

class Store {
    constructor() {

    }

    /**
     * 认证通道 
     * @param props  
     */
    onPassageway(props) {
        const { location } = props;
        // 超级管理员直接放行
        if (User.User.role == "administrator") {
            return true;
        }
        // 查找用户菜单配置中是否有当前路径
        return lodash.includes(User.User.subMenu, location.pathname)
    }
}
export default new Store();
/**
 * 权限装饰器
 * @param PageParams 
 */
export function AuthorizeDecorator(PageParams: { PageStore: PageStore }) {
    return function (Component: React.ComponentClass<any, any>) {
        return class extends Component {
            componentWillMount() {
                // console.log(this.props)
                // 假设 所有 为 false
                // notification.info({ message: "假设有权限 2 秒后" });
                super.componentWillMount && super.componentWillMount()
            }
            render(): any {
                return super.render();
            }
        }
    }
}
/**
* url 类型
*/
type UrlKeyType = "search" | "details" | "insert" | "update" | "delete" | "import" | "export" | "exportIds" | "template"
/**
 * 认证动作
 * @param PageStore 页面 Store
 * @param UrlsKey 按钮对应 URL 权限
 */
export function onAuthorizeActions(PageStore: PageStore, UrlsKey: UrlKeyType) {
    return lodash.get(PageStore, `Urls.${UrlsKey}.url`, GlobalConfig.development)// 开发 环境 默认返回 true
}
