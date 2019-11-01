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
import globalConfig from 'global.config';
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
    /** 操作接口数组 */
    Actions = [];
    @action
    async onSetUserInfo(userInfo) {
        // jwt
        if ('access_token' in userInfo) {
            globalConfig.headers.Authorization = `${userInfo.token_type} ${userInfo.access_token}`;
            const user = await Request.ajax("/api/_FrameworkUserBase/GetUserInfo").toPromise();
            userInfo = { ...user, Authorization: userInfo };
        }
        this.Actions = lodash.map(lodash.get(userInfo, 'Attributes.Actions', []), lodash.toLower);
        await Menu.onInitMenu(lodash.get(userInfo, 'Attributes.Menus', []));
        runInAction(() => {
            this.UserInfo = userInfo;
            this.isLogin = true;
        })
    }
    @action.bound
    async CheckLogin() {
        try {
            const userid = lodash.get(this.UserInfo, 'Id');
            const Authorization = lodash.get(this.UserInfo, 'Authorization');
            // jwt
            if (Authorization) {
                // const res = await Request.ajax("/api/_FrameworkUserBase/GetUserInfo").toPromise();
                // await this.onSetUserInfo(res);
                await this.onSetUserInfo(Authorization);
            } else if (userid) {
                const res = await Request.ajax("/api/_account/CheckLogin/" + userid).toPromise();
                await this.onSetUserInfo(res);
            }
        } catch (error) {
            this.outLogin(false);
            throw error
        } finally {
            runInAction(() => this.loding = false)
        }
    }
    async Login(params) {
        try {
            const res = await Request.ajax({
                method: "post",
                url: "/api/_Account/Login",
                body: params,
                headers: { 'Content-Type': null }
            }).toPromise();
            await this.onSetUserInfo(res);
        } catch (error) {
            console.log(error)
            throw error
        }
        finally {
            runInAction(() => this.loding = false)
        }
    }
    @action.bound
    async outLogin(Logout = true) {
        this.isLogin = false;
        this.UserInfo = null;
        const userid = lodash.get(this.UserInfo, 'Id');
        if (Logout && userid) {
            try {
                Request.ajax("/api/_account/Logout/" + userid).toPromise();
            } catch (error) {
            }
        }
        globalConfig.token.clear()
    }

}
export default new Store();
