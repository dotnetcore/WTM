/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:57
 * @modify date 2018-09-12 18:52:57
 * @desc [description]
*/
import { configure } from "mobx";
import User from './system/user';
import Authorize from './system/authorize';
import Meun from './system/menu';
configure({ enforceActions: "observed" });
var store = /** @class */ (function () {
    function store() {
        /** 用户 */
        this.User = User;
        /** 认证 */
        this.Authorize = Authorize;
        /** 菜单 */
        this.Meun = Meun;
        this.ready();
        this.init();
    }
    store.prototype.ready = function () {
        console.log("-----------全局状态------------", this);
    };
    store.prototype.init = function () {
    };
    return store;
}());
;
export default new store();
//# sourceMappingURL=index.js.map