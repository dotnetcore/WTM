import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { action, runInAction } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import Ajax from '../../utils/request';
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
            request = lodash.merge({
                method: "post",
                url: "/api/_login/Login",
                body: { userid, password },
                headers: { 'Content-Type': null }
            }, request);
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
                    const res = await Ajax.ajax(lodash.merge({
                        url: "/api/_login/CheckLogin/" + Id
                    }, request)).toPromise();
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