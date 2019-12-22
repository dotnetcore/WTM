import { Component, Vue } from "vue-property-decorator";
import { listToString, exportXlsx } from "@/util/string";
import { createBlob } from "@/util/files";
import UploadBox from "@/components/common/upload/index.vue";
import { Action } from "vuex-class";

/**
 * 首页中的按钮部分，添加/修改/删除/导出/导出
 */
@Component({
    components: {
        UploadBox
    }
})
export default class actionMixins extends Vue {
    @Action("search") search;
    @Action("batchDelete") batchDelete;
    @Action("deleted") deleted;
    @Action("exportExcel") exportExcel;
    @Action("exportExcelByIds") exportExcelByIds;
    @Action("detail") detail;
    @Action("imported") imported;
    @Action("getExcelTemplate") getExcelTemplate;
    // 表单弹框ref名称
    formRefName: string = "dialogForm";
    // 表单数据的key，下方相同，主要用在dialog-form组件中传入数据
    formDialogKey: string = "dialogInfo";

    // 表单弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // 导入
    uploadIsShow = false;
    // 打开详情弹框 ★★★★☆
    openDialog(status, data = {}) {
        console.log('status', status, this.formDialogKey, this[this.formDialogKey])
        this[this.formDialogKey].isShow = true;
        this[this.formDialogKey].dialogStatus = status;
        this[this.formDialogKey].dialogData = data;
        // this.$nextTick(() => {
        //     // onGetFormData在form-mixin.tx中
        //     this.$refs[this.formRefName].onGetFormData();
        // });
    }

    // 查询接口 ★★★★★
    privateRequest(params) {
        return this.search(params);
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
            this.batchDelete(parameters).then(res => {
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
            this.batchDelete(parameters).then(res => {
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
        this.exportExcel(parameters).then(res => {
            createBlob(res, this["$route"].name + "all");
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
        this.exportExcelByIds(parameters).then(res => {
            createBlob(res, this["$route"].name);
            this["$notify"]({
                title: "导出成功",
                type: "success"
            });
        });
    }
    /**
     * open importbox
     */
    onImported() {
        this.uploadIsShow = true;
    }
    /**
     * 下载
     */
    onDownload() {
        this.getExcelTemplate().then(res => createBlob(res));
    }
    /**
     * 导入★★★★☆
     * @param fileData
     */
    onImport(fileData) {
        const parameters = {
            UploadFileId: fileData.Id
        };
        this.imported(parameters).then(res => {
            this["$notify"]({
                title: "导入成功",
                type: "success"
            });
            this["onHoldSearch"]();
        });
    }
}
