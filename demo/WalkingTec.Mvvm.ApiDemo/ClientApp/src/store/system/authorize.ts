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
                notification.info({ message: "假设有权限 2 秒后" });
                lodash.delay(() => {
                    runInAction(() => {
                        PageParams.PageStore.Actions = {
                            ...PageParams.PageStore.Actions,
                            // insert: false,
                            // update: false,
                            delete: false,
                            import: false,
                            export: false,
                        }
                    })
                }, 2000)
                PageParams.PageStore.Actions
                super.componentWillMount && super.componentWillMount()
            }
            render(): any {
                return super.render();
            }
        }
    }
}

/**
* 权限枚举
*/
export enum EnumAuthorizeActions {
    /** 添加 */
    insert = 'insert',
    /** 修改 */
    update = 'update',
    /** 删除 */
    delete = 'update',
    /** 导入 */
    import = 'import',
    /** 导出 */
    export = 'export',
}
/**
     * 认证 动作 权限
     * @param Actions 页面动作对象。
     * @param EnumAut 枚举
     */
export function onAuthorizeActions(PageStore: PageStore, EnumAut: EnumAuthorizeActions) {
    return lodash.get(PageStore,`Actions.${EnumAut}`, GlobalConfig.development)// 开发 环境 默认返回 true
}
