/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import { Request } from 'utils/Request';
import { action, observable, runInAction } from "mobx";
import lodash from 'lodash';
import User from './user';
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