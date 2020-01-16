import { ColDef, ColGroupDef } from 'ag-grid-community';
import { observable, computed } from 'mobx';
import { create } from 'mobx-persist';
import { Subject, Subscription } from 'rxjs';
import { AjaxRequest } from 'rxjs/ajax';
/**
 * 事件类型
 */
export declare type EnumEventType = "onSearch" | "onDetails" | "onInsert" | "onUpdate" | "onDelete" | "onImport" | "onImport" | "onExport";
declare class EntitiesPageEventSubject {
    EventType: EnumEventType;
    AjaxRequest?: AjaxRequest;
    EventData?: Object;
}
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
     * 事件发布
     * @memberof EntitiesPage
     */
    EventSubject = new Subject<EntitiesPageEventSubject>();
    /**
     * 事件订阅
     * @type {Subscription}
     * @memberof EntitiesPage
     */
    Subscription: Subscription | undefined;
    /**
     * 每页条数
     * @memberof EntitiesPage
     */
    @observable
    PageSize = 20;
    /**
     * 数据总数
     * @memberof EntitiesPage
     */
    @observable
    Total = 0;
    /**
     * 当前页数
     * @memberof EntitiesPage
     */
    @observable
    Current = 0;
    /**
     * 表格数据
     * @type {any[]}
     * @memberof EntitiesPage
     */
    @observable
    RowData: any[] = [];
    /**
     * 表格列属性配置
     * @type {((ColDef | ColGroupDef)[])}
     * @memberof EntitiesPage
     */
    @observable
    ColumnDefs: (ColDef | ColGroupDef)[] = [

    ];
    /**
     * 选择的行 数据
     * @type {((ColDef | ColGroupDef)[])}
     * @memberof EntitiesPage
     */
    @observable
    SelectedRows: any[] = [];
    /**
     * 计算 是否有已 选择 数据
     * @readonly
     * @memberof EntitiesPage
     */
    @computed
    get IsSelectedRows() {
        return this.SelectedRows.length > 0
    }
    /**
     * 搜索的参数
     * @type {*}
     * @memberof EntitiesPage
     */
    @observable
    SearchParams: any = {};
    /**
    * 详情数据
    * @type {any[]}
    * @memberof EntitiesPage
    */
    @observable
    Details: any[] = [];
    /**
    * 加载状态 （搜索）
    * @memberof EntitiesPage
    */
    @observable
    Loading = false;
    /**
     * 加载状态 （编辑）
     * @memberof EntitiesPage
     */
    @observable
    LoadingEdit = false;
    /**
     * 加载状态 （详情）
     * @memberof EntitiesPage
     */
    @observable
    LoadingDetails = false;
    /**
     * 加载状态 （导入）
     * @memberof EntitiesPage
     */
    @observable
    LoadingImport = false;
    /**
     * 加载状态 （导出）
     * @memberof EntitiesPage
     */
    @observable
    LoadingExport = false;
    /**
     * 搜索条件的 收缩
     * @memberof EntitiesPage
     */
    @observable
    FilterCollapse = false;
    /**
     * 调试 日志
     * @memberof EntitiesPage
     */
    DeBugLog = false;
}