import { saveAs } from 'file-saver';
import lodash from 'lodash';
import { BindAll } from 'lodash-decorators';
import { action, runInAction } from 'mobx';
import { AjaxRequest } from "rxjs/ajax";
import { Regulars } from '../../utils/regulars';
import EntitiesBehavior, { IPageBehaviorOptions } from './behavior';
import { map } from 'rxjs/operators';
/**
 * 用户状态
 * @export
 * @class EntitiesUserStore
 * @extends {EntitiesBehavior}
 */
@BindAll()
export class EntitiesPageStore extends EntitiesBehavior {
    constructor(options: IPageBehaviorOptions) {
        super(options);
    }
    static SearchMap = {
        /**
         * 接口返回数据集合 key
         */
        RowData: 'Data',
        /**
         * 接口返回 数据总数 key
         */
        Total: 'Count',
        /**
         * 参数 每页条数  key
         */
        PageSize: 'Limit',
        /**
         * 接口返回 当前页码  key
         */
        Current: 'Page',
    }
    /**
     * Request.ajax  工厂
     * @param type 
     * @param request 
     */
    onObservableFactory(type: string, request: AjaxRequest = {}) {
        let observable = this.Request.ajax(this.merge(type, request))
        switch (type) {
            case 'Search':
                observable = observable.pipe(map(data => {
                    return {
                        RowData: lodash.get(data, EntitiesPageStore.SearchMap.RowData, []),
                        Total: lodash.get(data, EntitiesPageStore.SearchMap.Total, 0),
                        PageSize: lodash.get(request, `body.${EntitiesPageStore.SearchMap.PageSize}`, 0),
                        Current: lodash.get(data, EntitiesPageStore.SearchMap.Current, 0),
                        SearchParams: lodash.get(request, 'body', {}),
                    }
                }));
                break;
            // case 'Details':
            //     observable = observable.pipe(map(data => {
            //         return 
            //     }));
            //     break;
        }
        return observable
    }
    /**
     * 解析 onSearch数据
     * @param response 
     */
    onAnalysisSearch(response: any) {
        runInAction(() => {
            lodash.mapValues(response, (value, key, object) => {
                lodash.set(this, key, value);
            })
            this.onSelectionChanged([]);
        })
    }
    /**
     * 搜索列表
     * @param {AjaxRequest} [request]
     * @returns
     * @memberof EntitiesPageStore
     */
    @action
    async onSearch(request: AjaxRequest = {}) {
        try {
            if (this.Loading) {
                return console.warn("已请求~")
            }
            this.Loading = true;
            const res = await this.onObservableFactory('Search', request).toPromise();
            this.onAnalysisSearch(res)
        } catch (error) {
            EntitiesPageStore.onError(error, 'Search', request);
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
     * @memberof EntitiesPageStore
     */
    @action
    async onDetails(request: AjaxRequest = {}) {
        try {
            if (this.LoadingDetails) {
                return console.warn("已请求~")
            }
            this.LoadingDetails = true;
            const res = await this.onObservableFactory('Details', request).toPromise();
            runInAction(() => {
                this.Details = res;
            })
            return res
        } catch (error) {
            EntitiesPageStore.onError(error, 'Details', request);
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
     * @memberof EntitiesPageStore
     */
    @action
    async onInsert(request: AjaxRequest = {}) {
        try {
            if (this.LoadingEdit) {
                return console.warn("已请求~")
            }
            this.LoadingEdit = true;
            const res = await this.onObservableFactory('Insert', request).toPromise();
            return res
        } catch (error) {
            EntitiesPageStore.onError(error, 'Insert', request);
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
     * @memberof EntitiesPageStore
     */
    @action
    async onUpdate(request: AjaxRequest = {}) {
        try {
            if (this.LoadingEdit) {
                return console.warn("已请求~")
            }
            this.LoadingEdit = true;
            const res = await this.onObservableFactory('Update', request).toPromise();
            return res
        } catch (error) {
            EntitiesPageStore.onError(error, 'Update', request);
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
     * @memberof EntitiesPageStore
     */
    @action
    async onDelete(request: AjaxRequest = {}) {
        try {
            if (this.LoadingEdit) {
                return console.warn("已请求~")
            }
            this.LoadingEdit = true;
            const res = await this.onObservableFactory('Delete', request).toPromise();
            return res
        } catch (error) {
            EntitiesPageStore.onError(error, 'Delete', request);
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
     * @memberof EntitiesPageStore
     */
    @action
    async onImport(request: AjaxRequest = {}) {
        try {
            if (this.LoadingImport) {
                return console.warn("已请求~")
            }
            this.LoadingImport = true;
            const res = await this.onObservableFactory('Import', request).toPromise();
            return res
        } catch (error) {
            EntitiesPageStore.onError(error, 'Import', request);
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
     * @memberof EntitiesPageStore
     */
    @action
    async onExport(type: 'Export' | 'Template' | 'ExportByIds' = 'Export', request: AjaxRequest = {}) {
        try {
            if (this.LoadingExport) {
                return console.warn("已请求~")
            }
            this.LoadingExport = true;
            const res = await this.onObservableFactory(type, request).toPromise();
            const disposition = res.xhr.getResponseHeader('content-disposition');
            Regulars.filename.test(disposition);
            saveAs(res.response, RegExp.$1 || `${Date.now()}.xls`);
            return res
        } catch (error) {
            EntitiesPageStore.onError(error, type, request);
            throw error
        }
        finally {
            this.LoadingExport = false;
        }
    }
}
// export default new EntitiesUserStore();