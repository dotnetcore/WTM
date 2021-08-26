import { $System, ControllerBasics, EnumActionType } from "@/client";
import { ICellRendererParams } from "ag-grid-community";
import { ButtonProps } from "ant-design-vue/lib/button/buttonTypes";
import { mixins, Options, Prop, Vue } from "vue-property-decorator";
@Options({ components: {} })
export class PageActionBasics extends Vue {
    @Prop({ default: false }) debug;
    /**
      * 页面控制器
      */
    readonly PageController: ControllerBasics;
    get Pagination() {
        return this.PageController.Pagination;
    }
    /**
     * 页面操作
     * @readonly
     * @memberof ActionBasics
     */
    get isPageAction() {
        return !this.lodash.has(this, 'params.node')
    }
    /**
     * 鉴权
     * @param type 
     * @returns 
     */
    onAuthority(type: EnumActionType) {
        if (this.debug || this.debug === '') {
            return true
        }
        return this.__wtmAuthority(type, this.PageController)
    }
}

@Options({ components: {} })
export class RowActionBasics extends Vue {
    /**
    * 行 操作需要 aggrid 传入
    * @type {ICellRendererParams}
    * @memberof Action
    */
    @Prop() readonly params;
    get rowParams(): ICellRendererParams {
        return this.lodash.get(this, 'params', {}) as ICellRendererParams
    }
    /**
     * 当前数据的 id key aggrid getRowNodeId 属性返回
     * @readonly
     * @memberof RowActionBasics
     */
    get rowKey() {
        return this.lodash.get(this, 'params.node.id', '')
    }
    /**
     * 行数据操作 有 aggrid 传入属性
     * @readonly
     * @memberof Action
     */
    get isRowAction() {
        return this.lodash.has(this.rowParams, 'node')
    }
    /**
     * 按钮配置参数
     * @readonly
     * @memberof ActionBasics
     */
    get ButtonProps() {
        const styles: any = { size: 'default' }
        if (this.isRowAction) {
            styles.type = "link"
            styles.size = 'small'
        }
        return styles
    }
}

@Options({ components: {} })
export class ActionBasics extends mixins(PageActionBasics, RowActionBasics) {
    get isInfo() {
        return this.onAuthority(EnumActionType.Info)
    }
    get isInsert() {
        if (this.isRowAction) {
            return false
        }
        return this.onAuthority(EnumActionType.Insert)
    }
    get isUpdate() {
        return this.onAuthority(EnumActionType.Update)
    }
    get isDelete() {
        return this.onAuthority(EnumActionType.Delete)
    }
    get isImport() {
        if (this.isRowAction) {
            return false
        }
        return this.onAuthority(EnumActionType.Import)
    }
    get isExport() {
        if (this.isRowAction) {
            return false
        }
        return this.onAuthority(EnumActionType.Export)
    }
}
export default ActionBasics
