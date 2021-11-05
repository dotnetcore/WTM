import lodash from "lodash";
import { action, observable, toJS } from "mobx"
import { create, persist } from "mobx-persist"
interface UserInfo {
    ITCode: string
    Name: string
    PhotoId: string
    Roles: Array<{
        ID: string
        RoleCode: string
        RoleName: string
    }>
    Attributes: {
        Actions: Array<string>
        Menus: Array<string>
    }
}
/**
 * 用户实体
 */
export class UserEntity {
    constructor() {
    }
    /**
     * 初始化 persist
     */
    protected async onPersist() {
        const hydrate = create({
            // storage: Storage,   // or AsyncStorage in react-native.
            // default: localStorage
            // jsonify: true  // if you use AsyncStorage, here shoud be true
            // default: true
        });
        await hydrate('x-user', this)
        // persist({ _UserInfo: { type: "object" } })(this)
    }
    /**
     * 用户信息
     * @type {*}
     * @memberof UserEntity
     */
    @persist('object')
    @observable
    private _UserInfo: UserInfo = undefined;
    get UserInfo(): UserInfo {
        return toJS(this._UserInfo);
    }
    set UserInfo(value: UserInfo) {
        this.setUserInfo(value)
    }
    @action.bound
    private setUserInfo(value) {
        this._UserInfo = value;
    }
    /** headers Authorization */
    get Authorization(){
        const access_token = lodash.get(this.UserInfo, 'access_token');
        return `bearer ${access_token}`
    }
    @observable
    loading = false;
    /**
     * 切换加载状态
     * @param loading 
     */
    @action.bound
    protected onToggleLoading(loading: boolean = !this.loading) {
        // console.log("🚀 ~ file: entity.ts ~ line 63 ~ UserEntity ~ onToggleLoading ~ loading", loading)
        // if (lodash.eq(loading, false)) {
        //     return lodash.delay(action(() => { this.loading = loading }), 5000)
        // }
        this.loading = loading;
    }
    /**
     * 登录状态
     * @readonly
     * @memberof UserEntity
     */
    get LoginStatus() {
        return lodash.hasIn(this.UserInfo, 'ITCode')
    }
}
