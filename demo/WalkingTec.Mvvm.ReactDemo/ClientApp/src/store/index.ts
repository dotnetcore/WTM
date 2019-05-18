/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:57
 * @modify date 2018-09-12 18:52:57
 * @desc [description]
*/
import Authorize from './system/authorize';
import Meun from './system/menu';
import User from './system/user';
class store {
    constructor() {
        this.ready();
        this.init();
    }
    /** 用户 */
    User = User;
    /** 认证 */
    Authorize = Authorize;
    /** 菜单 */
    Meun = Meun;
    ready() {
        console.log("-----------全局状态------------", this);
    }
    init() {

    }
};
export default new store();