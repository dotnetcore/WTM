import { observable, computed } from 'mobx';
import { Random } from 'mockjs';
import differenceInYears from 'date-fns/differenceInYears';
/**
 * 对象 实体
 * @export
 * @class EntitiesUser
 */
export default class EntitiesUser {
    /**
     * 用户 ID
     *
     * @memberof EntitiesUser
     */
    @observable
    Id = Random.guid();
    /**
     * 姓名
     *
     * @memberof EntitiesUser
     */
    @observable
    Name = Random.cname();
    /**
     * 头像
     *
     * @memberof EntitiesUser
     */
    @observable
    Avatar = Random.image('200x100');
    /**
     * 生日
     *
     * @memberof EntitiesUser
     */
    @observable
    Birthday = new Date(1995, 1, 1);
    /**
     * 地址籍贯
     *
     * @memberof EntitiesUser
     */
    @observable
    Address = Random.city();
    /**
     * 年龄
     *
     * @readonly
     * @memberof EntitiesUser
     */
    @computed
    get Age() {
        return differenceInYears(new Date(), this.Birthday)
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
    Loading = false;
}