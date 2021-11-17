import { ControllerBasics } from "@/client";
import { Options, Vue, Emit } from "vue-property-decorator";
/**
 * 详情基础操作
 */
@Options({ components: {} })
export class PageDetailsBasics extends Vue {
    /**
      * 页面控制器
      */
    readonly PageController: ControllerBasics;
    formState = {}
    get queryKey() {
        return this.$WtmConfig.detailsVisible
    }
    get Entities() {
        return this.PageController.Entities
    }
    // 获取地址栏 ID 有 修改 没有 添加
    get ID() {
        return this.lodash.get(this.$route.query, this.queryKey)
    }
    // 编辑
    get IsEdit() {
        return !!this.ID
    }
    // 详情
    get IsInfo() {
        return this.lodash.has(this.$route.query, '_readonly')
    }
    // 多编辑
    get IsBatch() {
        return this.lodash.has(this.$route.query, '_batch')
    }
    get body() {
        return { ID: this.ID };
    }

    @Emit("refreshGrid")
    async onRefreshGrid(){

    }

    /**
     * 传递给 details 组件的 提交函数 返回一个 Promise
     * @param values 
     * @returns 
     */
    async onFinish(values): Promise<unknown> {
        console.log("LENG ~ extends ~ onFinish ~ values", values);
        if (this.IsBatch) {
            const Ids = this.lodash.map(this.PageController.Pagination.selectionDataSource, this.PageController.key)
            if (this.lodash.size(Ids)) {
                return this.PageController.onBatchUpdate(this.lodash.assign({Ids}, this.formState))
            }
            throw this.$t(this.$locales.action_update_batch_null)
        }
        if (this.IsEdit) {
            return this.PageController.onUpdate(this.formState)
        }
       await this.PageController.onInsert(this.lodash.omit(this.formState, ['Entity.ID']))
       await this.PageController.Pagination.onCurrentChange({ current: 1 });
    }
    async onLoading() {
        if (this.ID) {
            await this.Entities.onLoading(this.body);
            this.formState = this.lodash.assign({}, this.formState, this.Entities.dataSource)
        }
    }
}
