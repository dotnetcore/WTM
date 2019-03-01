/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import Request from 'utils/Request';
import { action, observable, runInAction } from "mobx";
// const Http = new Request('/user/');
class Store {
    constructor() {

    }
    @observable loding = true;
    @observable isLogin = false;
    // 用户信息
    @observable User = {
        role: "administrator",//administrator ordinary
        subMenu: [
            "/"
        ]
    };
    @action.bound
    async Login(params) {
        const res = await Request.post("/_login/login", params).toPromise();
        console.log(res)
        runInAction(() => {
            this.User = {
                ...this.User,
                ...res
            };
            this.isLogin = true;

        });
    }
    @action.bound
    async outLogin() {
        this.isLogin = false;
    }

}
export default new Store();