import { Component, Vue } from "vue-property-decorator";
import { listToString, exportXlsx } from "@/util/string";

/**
 * 首页中的按钮部分，添加/修改/删除/导出/导出
 */
@Component
export default class actionMixins extends Vue {
    // 表单弹框ref名称
    formRefName: string = "dialogForm";
    // 表单数据的key，下方相同，主要用在dialog-form组件中传入数据
    formDialogKey: string = "formDialog";

    // 表单弹出框内容 ★★★★☆
    formDialog = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // 打开详情弹框 ★★★★☆
    openDialog(status, data = {}) {
        this[this.formDialogKey].isShow = true;
        this[this.formDialogKey].dialogStatus = status;
        this[this.formDialogKey].dialogData = data;
        this.$nextTick(() => {
            this.$refs[this.formRefName].onGetFormData();
        });
    }

    // 查询接口 ★★★★★
    privateRequest(params) {
        return this["search"](params);
    }
    /**
     * 单个删除 ★★★★★
     * @param params
     */
    onDelete(params) {
        this["onConfirm"]().then(() => {
            const parameters = {
                ids: [params.ID]
            };
            this["batchDelete"](parameters).then(res => {
                this["$notify"]({
                    title: "删除成功",
                    type: "success"
                });
                this["onHoldSearch"]();
            });
        });
    }
    /**
     * 多个删除★★★★★
     */
    onBatchDelete() {
        this["onConfirm"]().then(() => {
            const parameters = {
                ids: listToString(this["selectData"], "ID")
            };
            this["batchDelete"](parameters).then(res => {
                this["$notify"]({
                    title: "删除成功",
                    type: "success"
                });
                this["onHoldSearch"]();
            });
        });
    }
    /**
     * 导出全部 ★★★★☆
     */
    onExportAll() {
        const parameters = {
            ...this["searchFormClone"],
            Page: this["pageDate"].currentPage,
            Limit: this["pageDate"].pageSize
        };
        this["exportExcel"](parameters).then(res => {
            exportXlsx(res, "exportExcel");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    /**
     * 导出单个 ★★★★☆
     */
    onExport() {
        const parameters = listToString(this["selectData"], "ID");
        this["exportExcelByIds"](parameters).then(res => {
            exportXlsx(res, "exportExcelByIds");
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
}
