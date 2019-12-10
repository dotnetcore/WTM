import format from 'date-fns/format/index';
import { BindAll } from 'lodash-decorators';
import { action, observable } from 'mobx';
import { interval, Subscription, timer } from 'rxjs';
import { map } from 'rxjs/operators';
/**
 * 时间实体
 */
// @BindAll()
export class EntitiesTimeStore {
    constructor() {
        this.onSetTime();
    }
    /**
     * 当前时间
     * @memberof EntitiesTimeStore
     */
    @observable
    currentTime = '';
    /**
     * 开启计时
     * @memberof EntitiesTimeStore
     */
    startInterval = false;
    /**
     * 设置当前时间
     *
     * @param {*} [time=Date.now()]
     * @memberof EntitiesTimeStore
     */
    @action
    onSetTime(time = Date.now()) {
        this.currentTime = format(time, "yyyy-MM-dd HH:mm:ss");
    }
    /**
     * 计时器
     * @type {(Subscription | undefined)}
     * @memberof EntitiesTimeStore
     */
    // TimeSubscription: Subscription | undefined;
    /**
     * 切换 计时器状态
     *
     * @param {*} [start=!this.TimeSubscription]
     * @returns
     * @memberof EntitiesTimeStore
     */
    onToggleTime(start = !this.startInterval) {
        if (start) {
            this.startInterval = true;
            this.onEndTime();
            this.onStartTime();
        } else {
            this.startInterval = false;
            this.onEndTime();
        }
        return start
    }
    /**
     * 开始计时
     *
     * @private
     * @memberof EntitiesTimeStore
     */
    private async onStartTime() {
        // this.onEndTime();
        this.onSetTime();
        if (this.startInterval) {
            await timer(1000).toPromise();
            this.onStartTime();
        }
        // this.TimeSubscription = interval(1000).pipe(map(() => Date.now())).subscribe(obs => {
        //     this.onSetTime(obs);
        // });
        // setInterval(()=>{
        //     this.onSetTime();
        // },1000)
    }
    /**
     * 结束 计时
     *
     * @private
     * @memberof EntitiesTimeStore
     */
    private onEndTime() {
        // this.TimeSubscription && this.TimeSubscription.unsubscribe();
        // this.TimeSubscription = undefined;
        // this.startInterval = false;
    }
}