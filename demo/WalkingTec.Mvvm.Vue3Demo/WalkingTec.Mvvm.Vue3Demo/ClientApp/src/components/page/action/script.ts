import { ICellRendererParams } from "ag-grid-community";
import { ControllerBasics, Pagination } from "@/client";
import { Vue, Options, Prop, mixins } from "vue-property-decorator";
import { EnumActionType } from "@/client";
import { ButtonProps } from "ant-design-vue/lib/button/buttonTypes";
@Options({ components: {} })
export class PageActionBasics extends Vue {
    EnumActionType = EnumActionType
    /**
      * 页面控制器
      */
    PageController: ControllerBasics;
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
        return true
    }
}

@Options({ components: {} })
export class RowActionBasics extends mixins(PageActionBasics) {
    /**
    * 行 操作需要 aggrid 传入
    * @type {ICellRendererParams}
    * @memberof Action
    */
    @Prop() params;
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
}

@Options({ components: {} })
export class ActionBasics extends mixins(RowActionBasics) {
    /**
     * 按钮样式等
     * @readonly
     * @memberof ActionBasics
     */
    styles(type: EnumActionType | string): number | ButtonProps {
        const styles: ButtonProps = { size: 'default' }
        switch (type) {
            case 'space':
                return 10
            case EnumActionType.Insert:
                styles.type = 'primary'
                break;
            default:
        }
        if (this.isRowAction) {
            styles.type = "link"
            styles.size = 'small'
        }
        return styles
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
