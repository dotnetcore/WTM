import lodash from 'lodash';
import User from './user';
var Store = /** @class */ (function () {
    function Store() {
    }
    /**
     * 认证通道
     * @param props
     */
    Store.prototype.onPassageway = function (props) {
        var location = props.location;
        // 超级管理员直接放行
        if (User.User.role == "administrator") {
            return true;
        }
        // 查找用户菜单配置中是否有当前路径
        return lodash.includes(User.User.subMenu, location.pathname);
    };
    return Store;
}());
export default new Store();
//# sourceMappingURL=authorize.js.map