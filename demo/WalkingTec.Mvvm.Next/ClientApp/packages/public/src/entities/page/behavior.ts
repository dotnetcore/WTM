import { action, runInAction } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import { IRequestOptions, Request } from '../../utils/request';
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
        lodash.merge(this.options, options);
    }
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
    static onError(error: any, type: string, request?: AjaxRequest) {
        console.error(error.response || error.message || error.status);
    };
    options: IPageBehaviorOptions = lodash.merge(EntitiesPageBehavior.options, {});
    Request = new Request(this.options);
    merge(type: string, request: AjaxRequest = {}) {
        try {
            const req = lodash.get(this.options, type);
            if (lodash.isNil(req) && lodash.isNil(request.url)) {
                throw ''
            }
            return lodash.merge<AjaxRequest, AjaxRequest>(req, request)
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
            this.SearchParams = request.body;
            const res = await this.Request.ajax(request).toPromise();
            runInAction(() => {
                this.RowData = res.Data;
                this.Total = res.Count;
                this.PageSize = res.PageCount;
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
            request = this.merge('Details', request)
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
            request = this.merge('Insert', request)
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
            request = this.merge('Update', request)
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
            request = this.merge('Delete', request)
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
            request = this.merge('Import', request)
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
            request = this.merge('Export', request)
            const res = await this.Request.ajax(request).toPromise();
            const disposition = res.xhr.getResponseHeader('content-disposition');
            saveAs(res.response, 'test.xls')
            return res
        } catch (error) {
            EntitiesPageBehavior.onError(error, 'Export', request);
            throw error
        }
        finally {
            this.LoadingExport = false;
        }
    }
}