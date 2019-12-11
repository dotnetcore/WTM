import { observable, computed } from 'mobx';
import differenceInYears from 'date-fns/differenceInYears';
import { persist, create } from 'mobx-persist';
import { ReplaySubject } from 'rxjs';

/**
 * 对象 实体
 * @export
 * @class EntitiesPage
 */
export default class EntitiesPage {
    constructor() {
       
    }
    private hydrate = create({
        // storage: window.localStorage,   // or AsyncStorage in react-native.
        // default: localStorage
        // jsonify: true  // if you use AsyncStorage, here shoud be true
        // default: true
    });
    /**
     * 加载状态 （登陆）
     * @memberof EntitiesPage
     */
    @observable
    Loading = false;
}