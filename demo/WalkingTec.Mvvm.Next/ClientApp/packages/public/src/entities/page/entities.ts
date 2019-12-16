import { ColDef, ColGroupDef } from 'ag-grid-community';
import { observable } from 'mobx';
import { create } from 'mobx-persist';

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
     * 每页条数
     * @memberof EntitiesPage
     */
    @observable
    PageSize = 0;
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
}