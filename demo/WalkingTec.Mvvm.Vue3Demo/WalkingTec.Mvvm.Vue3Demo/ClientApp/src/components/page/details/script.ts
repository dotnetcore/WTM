import { ControllerBasics } from "@/client";
import { Options, Vue } from "vue-property-decorator";
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
    get Entities() {
        return this.PageController.Entities
    }
    // 获取地址栏 ID 有 修改 没有 添加
    get ID() {
        return this.lodash.get(this.$route.query, this.$WtmConfig.detailsVisible)
    }
    get IsEdit() {
        return !!this.ID
    }
    get body() {
        return { ID: this.ID };
    }
    /**
     * 传递给 details 组件的 提交函数 返回一个 Promise
     * @param values 
     * @returns 
     */
    async onFinish(values) {
        console.log("LENG ~ extends ~ onFinish ~ values", values);
        if (this.IsEdit) {
            return this.PageController.onUpdate(this.formState)
        }
        return this.PageController.onInstall(this.lodash.omit(this.formState, ['Entity.ID']))
    }
    async onLoading() {
        if (this.ID) {
            await this.Entities.onLoading(this.body);
            this.formState = this.lodash.assign({}, this.formState, this.Entities.dataSource)
        }
    }
}
