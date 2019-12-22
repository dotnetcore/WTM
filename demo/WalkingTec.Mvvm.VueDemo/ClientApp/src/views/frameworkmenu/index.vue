<template>
  <div class="dataprivilege">
    <card>
      <but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" @onAdd="openDialog(dialogType.add)" @onEdit="openDialog(dialogType.edit, arguments[0])" @onDelete="onBatchDelete" @onExport="onExport" @onExportAll="onExportAll" @onImported="onImported" />
      <table-box :default-expand-all="true" :row-key="'ID'" :tree-props="{children: 'children'}" :is-selection="true" :tb-column="tableHeader" :data="treeData" :loading="loading" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="openDialog(dialogType.detail, rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="openDialog(dialogType.edit, rowData.row)">
            修改
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </table-box>
    </card>
    <dialog-form ref="dialogform" :is-show.sync="dialogInfo.isShow" :dialog-data="dialogInfo.dialogData" :status="dialogInfo.dialogStatus" @onSearch="onSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import baseMixin from "@/util/mixin/base";
import searchMixin from "@/util/mixin/search";
import actionMixin from "@/util/mixin/action-mixin";
import FuzzySearch from "@/components/tables/fuzzy-search.vue";
import TableBox from "@/components/tables/table-box.vue";
import ButBox from "@/components/tables/but-box.vue";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkmenu";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config.js";

@Component({
    mixins: [baseMixin, searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        FuzzySearch,
        TableBox,
        DialogForm,
        ButBox
    }
})
export default class Index extends Vue {
    @State searchData;

    // 动作
    assembly = ASSEMBLIES;
    // 弹出框内容 ★★★★☆
    dialogInfo = {
        isShow: false,
        dialogData: {},
        dialogStatus: ""
    };
    // 打开详情弹框 ★★★★☆
    openDialog(status, data = {}) {
        this.dialogInfo.isShow = true;
        this.dialogInfo.dialogStatus = status;
        this.dialogInfo.dialogData = data;
        this.$nextTick(() => {
            this.$refs["dialogform"].onGetFormData();
        });
    }
    // tabledata返回tree
    get treeData() {
        const list = this["tableData"];
        const tree = list.filter(parent => {
            if (!parent.ParentID) {
                const branchArr = list.filter(child => {
                    return parent.ID === child.ParentID;
                });
                if (branchArr.length > 0) {
                    parent.children = branchArr;
                }
            }
            return !parent.ParentID;
        });
        return tree;
    }
}
</script>
