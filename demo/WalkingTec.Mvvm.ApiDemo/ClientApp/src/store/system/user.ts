/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-09-12 18:52:54
 * @modify date 2018-09-12 18:52:54
 * @desc [description]
*/
import Request from 'utils/Request';
import lodash from 'lodash';
import { action, observable, runInAction } from "mobx";
import { create, persist } from 'mobx-persist';
import Menu from './menu'
const hydrate = create({
    storage: window.localStorage,   // or AsyncStorage in react-native.
    jsonify: true,  // if you use AsyncStorage, here shoud be true
    debounce: 1000,
})
class Store {
    constructor() {
        hydrate(`__User`, this).then(() => {
            this.CheckLogin()
        })
    }
    @observable loding = true;
    @observable isLogin = false;
    // 用户信息
    @persist("object")
    @observable UserInfo: any = {};
    @action
    onSetUserInfo(userInfo) {
        this.UserInfo = userInfo;
        this.isLogin = true;
        Menu.onInitMenu(lodash.get(userInfo, 'Attributes.Menus', []))
    }
    @action.bound
    async CheckLogin() {
        try {
            const userid = lodash.get(this.UserInfo, 'Id');
            if (userid) {
                const res = await Request.ajax("/api/_login/CheckLogin/" + userid).toPromise();
                this.onSetUserInfo(res);
            }
        } catch (error) {
            console.log(error)
            throw error
        } finally {
            runInAction(() => this.loding = false)
        }
    }
    async Login(params) {
        try {
            const res = await Request.ajax({
                method: "post",
                url: "/api/_login/login",
                body: params,
                headers: { 'Content-Type': null }
            }).toPromise();
            this.onSetUserInfo(res);
        } catch (error) {
            console.log(error)
            throw error
        }
        finally {
            runInAction(() => this.loding = false)
        }
    }
    @action.bound
    async outLogin() {
        this.isLogin = false;
        const userid = lodash.get(this.UserInfo, 'Id');
        if (userid) {
            Request.ajax("/api/_login/Logout/" + userid).toPromise();
        }
        window.localStorage.clear();
    }

}
export default new Store();