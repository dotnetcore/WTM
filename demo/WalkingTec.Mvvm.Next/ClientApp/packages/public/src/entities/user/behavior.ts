import { action, runInAction } from 'mobx';
import { timer } from 'rxjs';
import Entities, { MenuDataItem } from './entities';
import Request from '../../utils/request';
import lodash from 'lodash';
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
    async  onLogin(userid, password) {
        if (this.Loading) {
            return console.warn('onLogin Loadinged')
        }
        this.Loading = true;
        // 模拟一个等待 1秒钟
        // const res = await timer(1000).toPromise();
        try {
            const res = await Request.ajax(
                {
                    method: "post",
                    url: "/api/_login/Login",
                    body: { userid, password },
                    headers: { 'Content-Type': null }
                }
            ).toPromise();
            this.onVerifyingLanding(res);
        } catch (error) {
            runInAction(() => {
                this.Loading = false;
            })
            throw error
        }
    }
    @action
    onCheckLogin() {
        if (this.Loading) {
            return console.warn('onCheckLogin Loadinged')
        }
        this.Loading = true;
        const subscribe = this.InitialState.subscribe(async (Id) => {
            if (Id) {
                const res = await Request.ajax("/api/_login/CheckLogin/" + Id).toPromise();
                this.onVerifyingLanding(res);
            }
            runInAction(() => {
                this.Loading = false;
            })
            subscribe.unsubscribe();
        })
    }
    /**
     * 验证登陆
     * @private
     * @memberof EntitiesUserBehavior
     */
    @action
    private onVerifyingLanding(UserInfo: any = {}) {
        this.OnlineState = true;
        this.Loading = false;
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
                path: data.Url,
                name: data.Text,
                icon: data.Icon || "pic-right",
                // children: data.Children
            }
        });
        this.Menus = lodash.cloneDeep(Menus);
        this._MenuTrees = this.formatTree(Menus, null, []);
        this.Loading = false;
    }
    /**
     * 退出登陆
     * @memberof EntitiesUserBehavior
     */
    @action
    onOutLogin() {
        this.OnlineState = false;
        this.Id = undefined;
        this.ITCode = undefined;
        this.Menus = [];
        this._MenuTrees = [];
        this._Actions = [];
    }
    /**
    * 递归 格式化 树
    * @param datalist 
    * @param ParentId 
    * @param children 
    */
    formatTree(datalist, ParentId, children: MenuDataItem[] = []) {
        lodash.filter(datalist, ['ParentId', ParentId]).map(data => {
            data.children = this.formatTree(datalist, data.Id, data.children || []);
            children.push({
                ...data,
                key: data.Id,
                path: data.Url,
                name: data.Text,
                icon: data.Icon || "pic-right",
            });
        });
        return children;
    }
}