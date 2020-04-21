import lodash from 'lodash';
import { action } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import { filter } from 'rxjs/operators';
import { IRequestOptions, Request } from '../../utils/request';
import Entities, { EnumEventType } from './entities';
export interface IPageBehaviorOptions extends IRequestOptions {
    /** 搜索 */
    Search?: AjaxRequest;
    /** 详情 */
    Details?: AjaxRequest;
    /** 添加 */
    Insert?: AjaxRequest;
    /** 修改 */
    Update?: AjaxRequest;
    /** 删除 */
    Delete?: AjaxRequest;
    /** 导入 */
    Import?: AjaxRequest;
    /** 导出 */
    Export?: AjaxRequest;
    /** 导出 */
    ExportByIds?: AjaxRequest;
    /** 模板 */
    Template?: AjaxRequest;
    /** 其他 */
    [key: string]: AjaxRequest | any;
}
/**
 * 对象 动作 行为 
 * @export
 * @class EntitiesUserBehavior
 * @extends {Entities}
 */
export default class EntitiesPageBehavior extends Entities {
    constructor(options?: IPageBehaviorOptions) {
        super();
        lodash.merge(this.options, options);
        this.Request = new Request(this.options);
    }
    /**
     * 默认的配置
     *
     * @static
     * @type {IPageBehaviorOptions}
     * @memberof EntitiesPageBehavior
     */
    static options: IPageBehaviorOptions = {
        target: "",
        Search: { method: "POST", body: {} },
        Insert: { method: "POST", body: {} },
        Update: { method: "PUT", body: {} },
        Delete: { method: "POST", body: [] },
        Import: { method: "POST", body: {} },
        Export: { method: "POST", responseType: "blob", body: {} },
        ExportByIds: { method: "POST", responseType: "blob", body: [] },
        Template: { responseType: "blob", body: {} },
    }
    /**
     * 默认的错误处理函数
     *
     * @static
     * @param {*} error
     * @param {string} type
     * @param {AjaxRequest} [request]
     * @memberof EntitiesPageBehavior
     */
    static onError(error: any, type: string, request?: AjaxRequest) {
        console.error(error);
    };
    /**
     * 配置选项
     *
     * @type {IPageBehaviorOptions}
     * @memberof EntitiesPageBehavior
     */
    options: IPageBehaviorOptions = lodash.cloneDeep(EntitiesPageBehavior.options);
    /**
     * Request Ajax 
     *
     * @memberof EntitiesPageBehavior
     */
    Request: Request;
    /**
     * mrge AjaxRequest
     *
     * @param {string} type
     * @param {AjaxRequest} [request={}]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    merge(type: string, request: AjaxRequest = {}) {
        try {
            const req = lodash.get(this.options, type);
            const newRequest = lodash.merge({}, req, request) as AjaxRequest;
            if (lodash.isNil(newRequest.url)) {
                throw req
            }
            newRequest.body = request.body//lodash.assign(req.body, request.body);
            this.DeBugLog && console.table(newRequest);
            return lodash.cloneDeep(newRequest);
        } catch (error) {
            throw {
                message: `${type} AjaxRequest Null`,
                request: error
            }
        }
    };

    /**
     * 切换 FilterCollapse
     * @param {*} [Collapse=!this.FilterCollapse]
     * @memberof EntitiesPageBehavior
     */
    @action
    onToggleFilterCollapse(Collapse = !this.FilterCollapse) {
        this.FilterCollapse = Collapse
    };
    /**
     * 设置 SelectedRows
     * @param SelectedRows 
     */
    @action
    onSelectionChanged(SelectedRows) {
        this.SelectedRows = SelectedRows;
    }
    /**
     * 订阅 事件处理
     * 默认只处理 'onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport' 内置事件
     * @memberof EntitiesPageBehavior
     */
    // onCreateSubscribe() {
    //     this.Subscription = this.EventSubject.pipe(
    //         filter(obj => lodash.includes<EnumEventType>(['onSearch', 'onDetails', 'onDelete', 'onInsert', 'onUpdate', 'onImport', 'onExport'], obj.EventType))
    //     ).subscribe(event => {
    //         // 查找事件函数
    //         const EventFn = lodash.get(this, event.EventType, () => { console.warn(`未解析事件 ${event.EventType}`) });
    //         // 日志
    //         this.DeBugLog && console.warn(`TCL: ${event.EventType} ->`, event);
    //         // 执行
    //         EventFn(event.AjaxRequest);
    //     });
    // }
    // /**
    //  * 取消订阅
    //  * @memberof EntitiesPageBehavior
    //  */
    // onUnsubscribe() {
    //     this.Subscription && this.Subscription.unsubscribe();
    //     this.Subscription = undefined;
    // }
}