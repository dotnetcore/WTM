import { Component, Vue } from "vue-property-decorator";
import { listToString, exportXlsx } from "@/util/string";
import { createBlob } from "@/util/files";
import UploadBox from "@/components/page/upload/index.vue";
import { Action, State } from "vuex-class";

type EventFunction = (data: Object | String | Array<any>) => void;
type DefaultFunction = () => void;

export interface IActionEvent {
  onAdd?: DefaultFunction;
  onEdit?: EventFunction;
  onDetail?: EventFunction;
  onDelete?: EventFunction;
  onBatchDelete?: EventFunction;
  onImported?: DefaultFunction;
  onExportAll?: DefaultFunction;
  onExport?: EventFunction;
}

/**
 * 首页中的按钮部分，添加/修改/删除/导出/导出
 */

function mixinFunc(ASSEMBLIES: Array<string> = []) {
  @Component({
    components: {
      UploadBox
    }
  })
  class actionMixins extends Vue {
    @Action("search") search;
    @Action("batchDelete") batchDelete;
    @Action("deleted") deleted;
    @Action("exportExcel") exportExcel;
    @Action("exportExcelByIds") exportExcelByIds;
    @Action("detail") detail;
    @Action("imported") imported;
    @Action("getExcelTemplate") getExcelTemplate;
    // api 授权权限列表
    @State actionList;
    // 展示的动作按钮
    assembly: Array<string> = ASSEMBLIES;
    // 表单弹出框内容
    dialogIsShow: Boolean = false;
    // 打开选中数据
    dialogData: Object = {};
    // 打开详情状态（增删改查）
    dialogStatus: String = "";
    // 导入
    uploadIsShow: Boolean = false;

    /**
     * 事件方法list
     */
    get actionEvent(): IActionEvent {
      return {
        onAdd: this.onAdd,
        onEdit: this.onEdit,
        onDetail: this.onDetail,
        onDelete: this.onDelete,
        onBatchDelete: this.onBatchDelete,
        onImported: this.onImported,
        onExportAll: this.onExportAll,
        onExport: this.onExport
      };
    }
    /**
     * 打开详情弹框（默认框）
     * @param status
     * @param data
     */
    openDialog(status, data = {}) {
      this.dialogIsShow = true;
      this.dialogStatus = status;
      this.dialogData = data;
    }

    /**
     * 查询接口 ★★★★★
     * @param params
     */
    privateRequest(params) {
      return this.search(params);
    }
    /**
     * 添加
     */
    onAdd() {
      this.openDialog(this.$actionType.add);
    }
    /**
     * 修改
     * @param data
     */
    onEdit(data) {
      this.openDialog(this.$actionType.edit, { ID: data.ID });
    }
    /**
     * 详情
     * @param data
     */
    onDetail(data) {
      this.openDialog(this.$actionType.detail, { ID: data.ID });
    }
    /**
     * 单个删除
     * @param params
     */
    onDelete(params) {
      this.onConfirm().then(() => {
        const parameters = [params.ID];
        this.batchDelete(parameters).then(res => {
          this["$notify"]({
            title: this.$t("form.successfullyDeleted"),
            type: "success"
          });
          this["onHoldSearch"]();
        });
      });
    }
    /**
     * 多个删除
     */
    onBatchDelete() {
      this.onConfirm().then(() => {
        const parameters = listToString(this["selectData"], "ID");
        console.log('this["selectData"]', this["selectData"], parameters);
        this.batchDelete(parameters)
          .then(res => {
            this["$notify"]({
              title: this.$t("form.successfullyDeleted"),
              type: "success"
            });
            this["onHoldSearch"]();
          })
          .catch(err => {
            this["$notify"]({
              title: this.$t("form.failedToDelete"),
              type: "error"
            });
          });
      });
    }
    /**
     * 导出全部
     */
    onExportAll() {
      const parameters = {
        ...this["searchFormClone"],
        Page: this["pageDate"].currentPage,
        Limit: this["pageDate"].pageSize
      };
      this.exportExcel(parameters).then(res => {
        createBlob(res);
        this["$notify"]({
          title: this.$t("form.ExportSucceeded"),
          type: "success"
        });
      });
    }
    /**
     * 导出单个
     */
    onExport() {
      const parameters = listToString(this["selectData"], "ID");
      this.exportExcelByIds(parameters).then(res => {
        createBlob(res);
        this["$notify"]({
          title: this.$t("form.ExportSucceeded"),
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
     * 导入
     * @param fileData
     */
    onImport(fileData) {
      const parameters = {
        UploadFileId: fileData.Id
      };
      this.imported(parameters).then(res => {
        this["$notify"]({
          title: this.$t("form.ImportSucceeded"),
          type: "success"
        });
        this["onHoldSearch"]();
      });
    }
    /**
     * 删除确认
     * @param title
     */
    onConfirm(title?: string) {
      if (!title) {
        title = this.$t("form.confirmDeletion");
      }
      return this["$confirm"](title, this.$t("form.prompt"), {
        confirmButtonText: this.$t("buttom.delete"),
        cancelButtonText: this.$t("buttom.cancel"),
        type: "warning"
      });
    }
  }
  return actionMixins;
}
export default mixinFunc;
