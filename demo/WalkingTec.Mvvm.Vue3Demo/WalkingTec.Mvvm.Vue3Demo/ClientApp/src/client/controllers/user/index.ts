/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-04-02 11:49:08
 * @modify date 2021-04-02 11:49:08
 * @desc 用户管理
 */
import { AjaxBasics } from '@/client';
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { UserEntity } from './entity';
import { globalProperties } from "@/client";
@BindAll()
export class UserController extends UserEntity {
    $ajax = globalProperties.$Ajax;
    async onInit() {
        await this.onPersist()
        this.onCheckLogin()
    }
    /**
     * 登录
     */
    async onSignIn(body: { account: string, password: string }) {
        this.onToggleLoading(true)
        const res = await this.$ajax.post<any>('/api/_Account/Login', { rememberLogin: false, ...body }, { 'Content-Type': null })
        this.UserInfo = res
        this.onToggleLoading(false)
    }
    /**
     * 校验登录状态
     */
    async onCheckLogin() {
        this.onToggleLoading(true)
        try {
            const userid = lodash.get(this.UserInfo, 'ITCode');
            if (userid) {
                const res = await this.$ajax.get("/api/_Account/CheckUserInfo");
                this.UserInfo = res
            }
        } catch (error) {
            this.onLogOut()
            throw error
        }
        this.onToggleLoading(false)
    }
    async onLogOut() {
        this.$ajax.get("/api/_Account/Logout");
        this.UserInfo = {};
    }
}
// export const UserStore = new UserController()