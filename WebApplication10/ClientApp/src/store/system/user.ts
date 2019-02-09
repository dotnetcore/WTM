/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import { Request } from 'utils/Request';
import { action, observable, runInAction } from "mobx";
const Http = new Request('/user/');
class Store {
    constructor() {

    }
    @observable loding = true;
    @observable isLogin = true;
    // 用户信息
    @observable User = {
        role: "administrator",//administrator ordinary
        subMenu: [
            "/"
        ]
    };
    @action.bound
    async Login(params) {
        this.User = params;
        // const result = await Http.post("doLogin", params).toPromise();
        runInAction(() => {
            // this.User = result;
            this.isLogin = true;

        });
    }
    @action.bound
    async outLogin() {
        this.isLogin = false;
    }

}
export default new Store();