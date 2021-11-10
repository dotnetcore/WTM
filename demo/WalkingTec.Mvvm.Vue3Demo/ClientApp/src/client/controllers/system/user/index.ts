/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2021-04-02 11:49:08
 * @modify date 2021-04-02 11:49:08
 * @desc 用户管理
 */
import { AjaxBasics, globalProperties } from "@/client";
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { UserEntity } from './entity';
import { UserMenus } from './menus';
@BindAll()
export class UserController extends UserEntity {
    UserMenus = new UserMenus()
    $ajax: AjaxBasics;
    async onInit() {
        this.$ajax = globalProperties.$Ajax;
        await this.onPersist()
        await this.onCheckLogin()
    }
    onSetUserInfo(info) {
        this.UserInfo = info;
        this.UserMenus.onInit(this.UserInfo?.Attributes?.Menus)
        this.onToggleLoading(false)
    }
    /**
     * 登录
     */
    async onSignIn(body: { account: string, password: string }) {
        this.onToggleLoading(true)
        try {
            const res = await this.$ajax.post<any>('/api/_Account/Login', { rememberLogin: false, ...body }, { 'Content-Type': null })
            this.onSetUserInfo(res)
        } catch (error) {
            throw error
        }
        finally {
            this.onToggleLoading(false)
        }
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
                this.onSetUserInfo(res)
            }
            this.onToggleLoading(false)
        } catch (error) {
            this.onToggleLoading(false)
            this.onLogOut()
            throw error
        }
    }
    /**
     * 修改密码
     * @param body 
     * @returns 
     */
    onChangePassword(body) {
        return this.$ajax.post("/api/_Account/ChangePassword", body);
    }
    async onLogOut() {
        this.$ajax.get("/api/_Account/Logout");
        this.onSetUserInfo({})
    }
}
// export const UserStore = new UserController()