import differenceInYears from 'date-fns/differenceInYears';
import lodash from 'lodash';
import { computed, observable, toJS } from 'mobx';
import { create, persist } from 'mobx-persist';
import { BehaviorSubject } from 'rxjs';
import { debounceTime, filter, map } from 'rxjs/operators';
export interface MenuDataItem {
    authority?: string[] | string;
    children?: MenuDataItem[];
    hideChildrenInMenu?: boolean;
    hideInMenu?: boolean;
    icon?: string;
    locale?: string;
    name?: string;
    path: string;
    [key: string]: any;
}
/**
 * 对象 实体
 * @export
 * @class EntitiesUser
 */
export default class EntitiesUser {
    constructor() {
        this.hydrate('entities_user', this).then(() => {
            this.UserSubject.next(this)
        });
    }
    /**
     * 用户 状态变更 Subject
     * @memberof EntitiesUser
     */
    protected UserSubject = new BehaviorSubject<EntitiesUser>({} as EntitiesUser);
    /**
     * 用户 状态变更 UserObservable
     * @memberof EntitiesUser
     */
    UserObservable = this.UserSubject.pipe(
        debounceTime(50),
        filter(user => !lodash.isEmpty(user)),
        map(user => {
            user = toJS(user);
            lodash.unset(user, 'UserSubject');
            lodash.unset(user, 'hydrate');
            lodash.unset(user, 'UserObservable');
            lodash.unset(user, 'formatTree');
            lodash.unset(user, 'constructor');
            return user
        }));
    private hydrate = create({
        // storage: window.localStorage,   // or AsyncStorage in react-native.
        // default: localStorage
        // jsonify: true  // if you use AsyncStorage, here shoud be true
        // default: true
    });
    /**
     * 用户 ID
     * @memberof EntitiesUser
     */
    @persist
    @observable
    Id: string | undefined;
    /**
     * 用户 ITcode
     * @type {string}
     * @memberof EntitiesUser
     */
    ITCode: string | undefined;
    /**
     * 姓名
     * @memberof EntitiesUser
     */
    Name: string;
    /**
     * 角色
     * @type {any[]}
     * @memberof EntitiesUser
     */
    Roles: any[];
    /**
     * 用户组
     * @type {any[]}
     * @memberof EntitiesUser
     */
    Groups: any[];
    /**
     * 头像
     * @memberof EntitiesUser
     */
    Avatar: string;
    private _Birthday: Date;
    /**
    * 生日
    * @memberof EntitiesUser
    */
    public get Birthday(): Date {
        return this._Birthday || new Date();
    }
    public set Birthday(value: Date) {
        this._Birthday = value;
    }
    /**
     * 地址籍贯
     * @memberof EntitiesUser
     */
    Address: string;
    /**
     * 年龄
     * @readonly
     * @memberof EntitiesUser
     */
    get Age() {
        const currentTime = new Date();
        return differenceInYears(currentTime, this.Birthday || currentTime);
    }
    /**
     * 菜单
     * @type {any[]}
     * @memberof EntitiesUser
     */
    Menus: MenuDataItem[];
    @observable
    protected _MenuTrees: MenuDataItem[];
    /**
    * 树结构菜单
    * @type {any[]}
    * @memberof EntitiesUser
    */
    @computed
    public get MenuTrees(): MenuDataItem[] {
        return this._MenuTrees || [];
    }
    protected _Actions: any[];
    /**
     * 页面操作动作权限
     * @type {any[]}
     * @memberof EntitiesUser
     */
    public get Actions(): any[] {
        return this._Actions;
    }
    /**
     * 在线状态 （登陆）
     *
     * @memberof EntitiesUser
     */
    @observable
    OnlineState = false;
    /**
     * 加载状态 （登陆）
     * @memberof EntitiesUser
     */
    @observable
    Loading = true;
}