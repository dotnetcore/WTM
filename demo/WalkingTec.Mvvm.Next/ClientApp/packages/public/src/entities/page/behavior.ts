import { action, runInAction } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import { IRequestOptions, Request } from '../../utils/request';
import { Regulars } from '../../utils/regulars';
import Entities from './entities';
import lodash from 'lodash';
import { saveAs } from 'file-saver';
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
        lodash.merge(this.options, lodash.cloneDeep(options));
    }
    /**
     * 默认的配置
     *
     * @static
     * @type {IPageBehaviorOptions}
     * @memberof EntitiesPageBehavior
     */
    static options: IPageBehaviorOptions = {
        target: "/api",
        Search: { method: "POST", body: {} },
        Insert: { method: "POST", body: {} },
        Update: { method: "PUT", body: {} },
        Delete: { method: "POST", body: {} },
        Import: { method: "POST", body: {} },
        Export: { method: "POST", responseType: "blob", body: {} },
        ExportByIds: { method: "POST", responseType: "blob", body: {} },
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
        console.error(error.response || error.message || error.status);
    };
    /**
     * 配置选项
     *
     * @type {IPageBehaviorOptions}
     * @memberof EntitiesPageBehavior
     */
    options: IPageBehaviorOptions = lodash.merge({}, lodash.cloneDeep(EntitiesPageBehavior.options));
    /**
     * Request Ajax 
     *
     * @memberof EntitiesPageBehavior
     */
    Request = new Request(this.options);
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
            const req = lodash.cloneDeep(lodash.get(this.options, type));
            if (lodash.isNil(req) && lodash.isNil(request.url)) {
                throw ''
            }
            let newRequest = lodash.merge<AjaxRequest, AjaxRequest>(req, request);
            newRequest.body = lodash.assign(req.body, request.body);
            return newRequest
        } catch (error) {
            throw `${type} AjaxRequest Null`
        }
    }
    /**
     * 搜索列表
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onSearch(request?: AjaxRequest) {
        try {
            if (this.Loading) {
                return console.warn("已请求~")
            }
            this.Loading = true;
            request = this.merge('Search', request);
            const res = await this.Request.ajax(request).toPromise();
            this.SearchParams = request.body;
            runInAction(() => {
                this.RowData = res.Data;
                this.Total = res.Count;
                this.PageSize = lodash.get(this.SearchParams, 'Limit');
                this.Current = res.Page;
            })
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Search', request);
            throw error
        }
        finally {
            this.Loading = false;
        }
    }
    /**
     * 详情
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onDetails(request?: AjaxRequest) {
        try {
            if (this.LoadingDetails) {
                return console.warn("已请求~")
            }
            this.LoadingDetails = true;
            request = this.merge('Details', request);
            const res = await this.Request.ajax(request).toPromise();
            runInAction(() => {
                this.Details = res.Entity;
            })
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Details', request);
            throw error
        }
        finally {
            this.LoadingDetails = false;
        }
    }
    /**
     * 添加数据
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onInsert(request?: AjaxRequest) {
        try {
            if (this.LoadingEdit) {
                return console.warn("已请求~")
            }
            this.LoadingEdit = true;
            request = this.merge('Insert', request);
            const res = await this.Request.ajax(request).toPromise();
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Insert', request);
            throw error
        }
        finally {
            this.LoadingEdit = false;
        }
    }
    /**
     * 修改数据
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onUpdate(request?: AjaxRequest) {
        try {
            if (this.LoadingEdit) {
                return console.warn("已请求~")
            }
            this.LoadingEdit = true;
            request = this.merge('Update', request);
            const res = await this.Request.ajax(request).toPromise();
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Update', request);
            throw error
        }
        finally {
            this.LoadingEdit = false;
        }
    }
    /**
     * 删除数据
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onDelete(request?: AjaxRequest) {
        try {
            if (this.LoadingEdit) {
                return console.warn("已请求~")
            }
            this.LoadingEdit = true;
            request = this.merge('Delete', request);
            const res = await this.Request.ajax(request).toPromise();
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Delete', request);
            throw error
        }
        finally {
            this.LoadingEdit = false;
        }
    }
    /**
     * 导入
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onImport(request?: AjaxRequest) {
        try {
            if (this.LoadingImport) {
                return console.warn("已请求~")
            }
            this.LoadingImport = true;
            request = this.merge('Import', request);
            const res = await this.Request.ajax(request).toPromise();
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Import', request);
            throw error
        }
        finally {
            this.LoadingImport = false;
        }
    }
    /**
     * 导出
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageBehavior
     */
    @action
    async onExport(request?: AjaxRequest) {
        try {
            if (this.LoadingExport) {
                return console.warn("已请求~")
            }
            this.LoadingExport = true;
            request = this.merge('Export', request);
            const res = await this.Request.ajax(request).toPromise();
            const disposition = res.xhr.getResponseHeader('content-disposition');
            Regulars.filename.test(disposition);
            saveAs(res.response, RegExp.$1 || `${Date.now()}.xls`);
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Export', request);
            throw error
        }
        finally {
            this.LoadingExport = false;
        }
    }
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
     * 订阅 事件处理
     * @memberof EntitiesPageBehavior
     */
    onSubscribe() {
        this.Subscription = this.EventSubject.subscribe(event => {
            console.warn("TCL: PageView -> mounted -> event", event);
            switch (event.EventType) {
                case "onSearch":
                    this.onSearch(event.AjaxRequest);
                    break;
                case "onDetails":
                    this.onDetails(event.AjaxRequest);
                    break;
                case "onDelete":
                    this.onDelete(event.AjaxRequest);
                    break;
                case "onInsert":
                    this.onInsert(event.AjaxRequest);
                    break;
                case "onUpdate":
                    this.onUpdate(event.AjaxRequest);
                    break;
                case "onImport":
                    this.onImport(event.AjaxRequest);
                    break;
                case "onExport":
                    this.onExport(event.AjaxRequest);
                    break;

            }
        });
    }
    /**
     * 取消订阅
     * @memberof EntitiesPageBehavior
     */
    onUnsubscribe() {
        this.Subscription && this.Subscription.unsubscribe();
        this.Subscription = undefined;
    }
}