import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { action, runInAction } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import Ajax, { Request } from '../../utils/request';
import EntitiesBehavior from './behavior';
/**
 * 用户状态
 * @export
 * @class EntitiesUserStore
 * @extends {EntitiesBehavior}
 */
@BindAll()
export class EntitiesUserStore extends EntitiesBehavior {
    /**
     * 登录 ajax 默认配置
     * @static
     * @type {AjaxRequest}
     * @memberof EntitiesUserStore
     */
    static loginAjaxRequest: AjaxRequest = {
        method: "post",
        url: "/api/_login/Login",
        headers: { 'Content-Type': null }
    };
    /**
     * CheckLogin ajax 默认配置
     * @static
     * @type {AjaxRequest}
     * @memberof EntitiesUserStore
     */
    static checkLoginAjaxRequest: AjaxRequest = {
        url: "/api/_login/CheckLogin/{Id}"
    };
    /**
     * 用户登录
     * @param {*} userid 用户名
     * @param {*} password 密码
     * @memberof EntitiesUserBehavior
     */
    @action
    async  onLogin(userid, password, request: AjaxRequest = {}) {
        if (this.Loading) {
            return console.warn('onLogin Loadinged')
        }
        this.Loading = true;
        // 模拟一个等待 1秒钟
        // const res = await timer(1000).toPromise();
        try {
            request = lodash.merge(
                EntitiesUserStore.loginAjaxRequest,
                { body: { userid, password } }, request);
            const res = await Ajax.ajax(request).toPromise();
            this.onVerifyingLanding(res);
            return res;
        } catch (error) {
            runInAction(() => {
                this.Loading = false;
            })
            throw error
        }
    }
    /**
     * CheckLogin
     * @memberof EntitiesUserBehavior
     */
    @action
    onCheckLogin(request: AjaxRequest = {}) {
        // if (this.Loading) {
        //     return console.warn('onCheckLogin Loadinged')
        // }
        // this.Loading = true;
        const subscribe = this.UserObservable.subscribe(async ({ Id, Loading, OnlineState }) => {
            subscribe && subscribe.unsubscribe();
            try {
                if (Id && Loading) {
                    const res = await Ajax.ajax(lodash.merge(
                        EntitiesUserStore.checkLoginAjaxRequest,
                        { body: { Id } },
                        request)).toPromise();
                    return this.onVerifyingLanding(res);
                }
                throw ''
            } catch (error) {
                this.onOutLogin();
                // throw error
            }
        })
    }
    /**
    * 解析登录信息
    * @protected
    * @memberof EntitiesUserBehavior
    */
    @action
    onVerifyingLanding(UserInfo: any = {}) {
        // jwt
        if (UserInfo.access_token) {
            lodash.set(Request.headers, 'Authorization', `${UserInfo.token_type} ${UserInfo.access_token}`);
        } else {
            // this.Loading = false;
            this.Name = lodash.get(UserInfo, 'Name');
            this.Id = lodash.get(UserInfo, 'Id');
            this.ITCode = lodash.get(UserInfo, 'ITCode');
            // 动作
            this._Actions = lodash.get(UserInfo, 'Attributes.Actions', []);
            const PhotoId = lodash.get(UserInfo, 'PhotoId');
            this.Avatar = PhotoId ? `/api/_file/getFile/${PhotoId}` : 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png';
            const onAnalysisMenus = async () => {
                await this.onAnalysisMenus(lodash.get(UserInfo, 'Attributes.Menus', []));
                this.UserSubject.next(this);
            }
            lodash.defer(() => {
                onAnalysisMenus();
            })
        }
    }
    /**
    * 退出登陆
    * @memberof EntitiesUserBehavior
    */
    @action
    onOutLogin() {
        this.OnlineState = false;
        this.Loading = false;
        this.Id = undefined;
        this.ITCode = undefined;
        this.Menus = [];
        this._MenuTrees = [];
        this._Actions = [];
        this.UserSubject.next(this)
    }
}
// export default new EntitiesUserStore();