import { action } from 'mobx';
import { timer } from 'rxjs';
import Entities from './entities';
/**
 * 对象 动作 行为 
 * @export
 * @class EntitiesUserBehavior
 * @extends {Entities}
 */
export default class EntitiesUserBehavior extends Entities {
    /**
     * 用户登录
     * @param {*} UserName 用户名
     * @param {*} Password 密码
     * @memberof EntitiesUserBehavior
     */
    @action
    async  onLogin(UserName, Password) {
        this.Loading = true;
        // 模拟一个等待 1秒钟
        const res = await timer(1000).toPromise();
        this.onVerifyingLanding(UserName, Password);
    }
    /**
     * 验证登陆
     * @private
     * @memberof EntitiesUserBehavior
     */
    @action
    private onVerifyingLanding(UserName, Password) {
        this.OnlineState = true;
        this.Loading = false;
        this.Name = UserName;
    }
    /**
     * 退出登陆
     * @memberof EntitiesUserBehavior
     */
    @action
    onOutLogin() {
        this.OnlineState = false;
    }
}