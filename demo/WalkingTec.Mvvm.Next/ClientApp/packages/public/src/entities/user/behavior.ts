import { action } from 'mobx';
import { timer } from 'rxjs';
import Entities from './entities';
import { Request } from '../../utils/request';
/**
 * 对象 动作 行为 
 * @export
 * @class EntitiesUserBehavior
 * @extends {Entities}
 */
export default class EntitiesUserBehavior extends Entities {
    Request = new Request();
    /**
     * 用户登录
     * @param {*} userid 用户名
     * @param {*} password 密码
     * @memberof EntitiesUserBehavior
     */
    @action
    async  onLogin(userid, password) {
        this.Loading = true;
        // 模拟一个等待 1秒钟
        // const res = await timer(1000).toPromise();
        const res = await this.Request.ajax(
            {
                method: "post",
                url: "/api/_login/Login",
                body: { userid, password },
                headers: { 'Content-Type': null }
            }
        ).toPromise();
        console.log("TCL: EntitiesUserBehavior -> onLogin -> res", res)
        this.onVerifyingLanding(userid, password);
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