/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2020-08-05 14:16:43
 * @modify date 2020-08-05 14:16:43
 * @desc [description]
 */
import lodash from 'lodash';
import { action, computed, observable, toJS } from 'mobx';
import { AjaxRequest } from 'rxjs/ajax';
import { map } from 'rxjs/operators';
import { AjaxBasics } from '../../helpers/ajaxBasics';

export interface PaginationResponse<T> {
    /** 数据集 */
    dataSource: Array<T>,
    /** 页码 */
    current?: number,
    /** 长度 */
    pageSize?: number,
    /** 总数 */
    total?: number
}
export type PaginationOnMapValues = (<T>(res: any) => PaginationResponse<T>) | String;
export interface PaginationOptions {
    /** url */
    url?: string;
    /** 请求方式 */
    method?: string;
    /** 无限滚动 */
    infinite?: boolean;
    /** 滚动加载的方向。默认 bottom */
    direction?: 'top' | 'bottom'
    /** 数据唯一标识 key */
    key?: string;
    /** 默认的当前页数 1 */
    defaultCurrent?: number;
    /** 默认的每页条数 10 */
    defaultPageSize?: number;
    /** 排序 key */
    soryKey?: string;
    /** 统计 key */
    totalKey?: string;
    /** 当前页码 接口使用key */
    currentKey?: string;
    /** 每页条数 接口使用key */
    pageSizeKey?: string;
    /** 请求结果过滤 */
    onMapValues?: PaginationOnMapValues;
}
export interface PaginationInfiniteEvent {
    complete: Function;
    error: Function;
    loaded: Function;
    reset: Function
}
// @BindAll()
export class Pagination<T = any> {
    constructor(protected $ajax: AjaxBasics, options: PaginationOptions) {
        this.onReset(lodash.merge({
            key: 'key',
            method: 'get',
            infinite: false,
            direction: 'bottom',
            defaultCurrent: 1,
            defaultPageSize: 20,
            soryKey: 'SortInfo',
            totalKey: 'Count',
            currentKey: 'Page',
            pageSizeKey: 'Limit',
        }, options))
    }
    /**
     * 配置
     * @protected
     * @type {PaginationOptions}
     * @memberof Pagination
     */
    options: PaginationOptions;
    /**
     * 唯一标识 解决 重置 后 的请求重叠问题
     * @private
     * @memberof Pagination
     */
    @observable
    onlyKey: string = lodash.uniqueId('key_');
    /**
     * 数据key
     * @readonly
     * @memberof Pagination
     */
    get key() {
        return this.options.key;
    }
    /**
     * 数据源
     * @type {Array<T>}
     * @memberof Pagination
     */
    @observable
    dataSource: Array<T> = [];

    // @computed
    // get dataSource(): Array<T> {
    //     // console.time()
    //     // const dataSource = toJS(this._dataSource);
    //     // console.timeEnd()
    //     return lodash.clone(this._dataSource)
    // }
    /**
     * 选择的 行数据
     * @memberof Pagination
     */
    @observable
    selectionDataSource = [];
    /**
     * 当前页
     * @memberof Pagination
     */
    @observable
    current = 1;
    /**
     * 每页条数
     * @memberof Pagination
     */
    @observable
    pageSize = 10;
    @observable
    sory: any = undefined;
    /**
     * 数据总数
     * @memberof Pagination
     */
    @observable
    total = 0;
    /**
     * 加载状态
     * @memberof Pagination
     */
    @observable
    loading = false;
    /**
     * 没有更多数据
     * @memberof Pagination
     */
    @observable
    isUndefined = false;
    /**
     * 错误
     * @memberof Pagination
     */
    @observable
    error = false
    /**
     * 是否已加载完成
     * @readonly
     * @memberof Pagination
     */
    @computed
    get finished() {
        // console.log("LENG: Pagination<T> -> getfinished -> this.isUndefined", this.isUndefined)
        return this.isUndefined;
    }
    /**
     * 加载 数据 body
     * @memberof Pagination
     */
    oldBody = {};
    /**
     * 分页加载
     *
     * @param {*} [body] 参数
     * @param {AjaxRequest} [AjaxRequest] 
     * @param {PaginationInfiniteEvent} [infiniteEvent] 无限滚动 事件
     * @returns
     * @memberof Pagination
     */
    async onLoading(body?, AjaxRequest?: AjaxRequest, infiniteEvent?: PaginationInfiniteEvent) {
        try {
            if (this.options.infinite && this.isUndefined) {
                throw new Error('分页 数据 没有更多数据')
            }
            if (this.loading) {
                return console.warn('分页 数据 加载中')
            }
            this.onToggleLoading(true);
            // 保存请求的参数
            this.oldBody = lodash.cloneDeep(body);
            body = lodash.merge(body, {
                // 转换 current pageSize 对应 的字段名
                [this.options.currentKey]: this.current,
                [this.options.pageSizeKey]: this.pageSize,
                [this.options.soryKey]: this.sory,
            });
            if (lodash.isEmpty(lodash.get(body, this.options.soryKey))) {
                lodash.unset(body, this.options.soryKey)
            }
            // await new Promise((res) => {
            //     lodash.delay(res, 3000);
            //   });
            console.log("LENG: Pagination<T> -> onLoading -> body", body)
            AjaxRequest = lodash.merge({
                url: this.options.url,
                method: this.options.method,
                body
            }, AjaxRequest);
            const onlyKey = this.onlyKey;
            const res = await this.$ajax.request<PaginationResponse<T>>(AjaxRequest)
                .pipe(
                    // filter(() => lodash.eq(onlyKey, this.onlyKey)), 
                    // delay(800),
                    map(res => this.onMapValues(res)),
                )
                .toPromise();
            if (!lodash.eq(this.onlyKey, onlyKey)) {
                console.warn(`onlyKey: ${onlyKey}  失效 当前：${this.onlyKey}`,)
                return
            }
            const dataSource = this.onSetDataSource(res, onlyKey);
            // 滚动 结束
            if (infiniteEvent) {
                if (this.isUndefined) {
                    infiniteEvent.complete()
                } else {
                    infiniteEvent.loaded()
                }
            }
            this.onToggleLoading(false);
            return dataSource
        } catch (error) {
            this.onToggleLoading(false, true);
            infiniteEvent && infiniteEvent.complete()
            console.warn("LENG: Pagination<T> -> onLoading -> error", error)
        }
    }
    @action
    onCurrentChange(params: { current?: number, pageSize?: number, sory?, body?}, loading = true) {
        const { body, current, pageSize, sory } = params
        if (lodash.isNumber(current) || lodash.isString(current)) {
            this.current = lodash.toInteger(current);
        }
        if (lodash.isNumber(pageSize) || lodash.isString(pageSize)) {
            this.pageSize = lodash.toInteger(pageSize)
        }
        if (sory) {
            this.sory = sory
        } else {
            this.sory = undefined
        }
        return this.onLoading(body || this.oldBody)
    }
    /**
     * 处理 过滤 res
     * @param {*} res
     * @returns
     * @memberof Pagination
     */
    // @Bind()
    protected onMapValues(res): PaginationResponse<T> {
        const { onMapValues } = this.options;
        // wtm
        if (lodash.hasIn(res, 'Data')) {
            lodash.set(res, 'dataSource', lodash.get(res, 'Data'))
        }
        if (lodash.isFunction(onMapValues)) {
            return onMapValues(res);
        }
        if (lodash.isString(onMapValues)) {
            const dataSource = lodash.get(res, onMapValues);
            lodash.unset(res, onMapValues)
            return { dataSource, ...res };
        }
        if (lodash.isArray(res)) {
            return { dataSource: res }
        }

        return res
    }
    /**
     * 设置解析数据
     * @private
     * @param {*} dataSource
     * @returns
     * @memberof Pagination
     */
    @action
    onSetDataSource(res: PaginationResponse<T>, onlyKey?) {
        console.log("LENG ~ Pagination<T ~ onSetDataSource ~ res", res)
        if (!lodash.isArray(res.dataSource)) {
            this.isUndefined = true;
            throw new Error('分页 数据 返回值 dataSource 不是数组')
        }
        // 检查 是否是完全的一页数据 完全 可以继续加载
        if (res.dataSource.length < this.options.defaultPageSize) {
            this.isUndefined = true;
            if (res.dataSource.length <= 0) {
                // throw new Error('分页 数据 没有更多数据')
            }
        }
        // 第一页
        if (lodash.eq(this.current, this.options.defaultCurrent) || this.options.infinite === false) {
            this.dataSource = res.dataSource;
        } else {
            if (this.options.direction === 'bottom') {
                this.dataSource = lodash.concat(this.dataSource, res.dataSource);
            } else {
                this.dataSource = lodash.concat(res.dataSource, this.dataSource);
            }
        }
        // debugger
        // 无限滚动
        if (this.options.infinite) {
            this.current += 1;
        }
        this.total = lodash.get(res, this.options.totalKey, 0);
        return res.dataSource;
    }
    @action
    onSet(dataSource) {
        this.dataSource = dataSource
    }
    /**
     * 切换加载状态
     * @private
     * @param {boolean} [loading=!this.loading]
     * @memberof Pagination
     */
    @action
    onToggleLoading(loading: boolean = !this.loading, error = false) {
        this.loading = loading;
        this.error = error;
    }
    /**
     * 追加
     * @param item 
     */
    @action
    onPush(item: T) {
        this.dataSource.push(item)
    }
    /**
     * 追加
     * @param item 
     */
    @action
    onUnshift(item: T) {
        this.dataSource.unshift(item)
    }
    /**
     * 根据 key 查找 数据
     * @param {string} key
     * @returns {T}
     * @memberof Pagination
     */
    onFind(key: string | T): T {
        const data = lodash.find(this.dataSource, this.getPredicate(key));
        if (!lodash.hasIn(data, this.options.key)) {
            throw new Error(`没有找到 Key:${key} 数据`)
        }
        return data;
    }
    /**
     * 修改更新
     * @param key 
     * @param value 
     */
    @action
    onUpdate(key: string | T, updater: (oldValue: any) => any) {
        const dataSource = lodash.clone(this.dataSource);
        const index = lodash.findIndex(dataSource, this.getPredicate(key));
        if (index === -1) {
            return
        }
        lodash.updateWith(dataSource, `[${index}]`, (old) => updater(toJS(old)))
        this.dataSource = dataSource;
        return dataSource
    }
    /**
     * 排序
     * @param key 
     * @param overIndex 
     */
    @action
    onOrderBy(key: string | T, overIndex) {
        let dataSource = lodash.clone(this.dataSource);
        const result = lodash.remove(dataSource, this.getPredicate(key));
        dataSource = lodash.concat(lodash.slice(dataSource, 0, overIndex), result, lodash.slice(dataSource, overIndex))
        this.dataSource = dataSource;
    }
    /**
     * 根据 key 删除数据
     * @param {string} key
     * @returns {T[]}
     * @memberof Pagination
     */
    @action
    onRemove(key: string | T): T[] {
        const dataSource = lodash.clone(this.dataSource);
        const result = lodash.remove(dataSource, this.getPredicate(key));
        this.dataSource = dataSource;
        return result
    }
    /**
     * 获取 lodash Predicate 参数
     * @param key 
     */
    getPredicate(key: string | T | Array<T>): any {
        if (lodash.isArray(key)) {
            return (item) => lodash.some(key, [this.key, lodash.get(item, this.key)])
        }
        if (lodash.isObject(key)) {
            return [this.key, lodash.get(key, this.key)]
        }
        return [this.key, key]
    }
    /**
     * 重置
     * @returns
     * @memberof Pagination
     */
    @action
    onReset(options?: PaginationOptions) {
        this.options = lodash.merge({}, this.options, options);
        this.current = this.options.defaultCurrent;
        this.pageSize = this.options.defaultPageSize;
        this.isUndefined = false;
        this.total = 0;
        this.dataSource = [];
        this.oldBody = null;
        this.loading = false;
        this.error = false;
        // lodash.defer(() => {
        //     runInAction(() => {
        this.onlyKey = lodash.uniqueId('key_')
        //     })
        // })
        return this;
    }
    @action
    onSelectionChanged(rows) {
        this.selectionDataSource = rows;
    }
}
export default Pagination