import { action, runInAction, toJS } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import { timer } from 'rxjs';
import Entities, { MenuDataItem } from './entities';
import Ajax, { Request } from '../../utils/request';
import lodash from 'lodash';
import { filter } from 'rxjs/operators';
// import { MenuDataItem } from '@ant-design/pro-layout';

/**
 * 对象 动作 行为 
 * @export
 * @class EntitiesUserBehavior
 * @extends {Entities}
 */
export default class EntitiesUserBehavior extends Entities {
    // Request = new Request();
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
     * 验证登陆
     * @private
     * @memberof EntitiesUserBehavior
     */
    @action
    private onVerifyingLanding(UserInfo: any = {}) {
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
            // 格式化 菜单 
            const Menus = lodash.get(UserInfo, 'Attributes.Menus', []).map(data => {
                return {
                    ...data,
                    key: data.Id,
                    path: data.Url || '',
                    name: data.Text,
                    icon: data.Icon || "pic-right",
                    // children: data.Children
                }
            });
            const onAnalysisMenus = async () => {
                await this.onAnalysisMenus(Menus);
                this.UserSubject.next(this);
            }
            lodash.defer(() => {
                onAnalysisMenus();
            })
        }

    }
    /**
     * 解析菜单 
     * @memberof EntitiesUserBehavior
     */
    async onAnalysisMenus(Menus) {
        runInAction(() => {
            this.Menus = Menus;
            this._MenuTrees = this.formatTree(Menus, null, []);
            this.Loading = false;
            this.OnlineState = true;
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
    /**
    * 递归 格式化 树
    * @param datalist 
    * @param ParentId 
    * @param children 
    */
    formatTree(datalist, ParentId, children: MenuDataItem[] = []) {
        lodash.filter(datalist, ['ParentId', ParentId]).map(data => {
            data = lodash.cloneDeep(data)
            data.children = this.formatTree(datalist, data.Id, data.children || []);
            children.push(data);
        });
        return children;
    }
}