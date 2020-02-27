<template>
  <div class="dataprivilege">
    <card>
      <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :eventFn="eventFn" />
      <wtm-table-box :default-expand-all="true" :row-key="'ID'" :tree-props="{children: 'children'}" :is-selection="true" :tb-column="tableHeader" :data="treeData" :loading="loading" @selection-change="onSelectionChange" @sort-change="onSortChange">
        <template #ICon="rowData">
          <i :class="[rowData.row.ICon]"></i>
        </template>
        <template #operate="rowData">
          <el-button v-visible="actionList.detail" type="text" size="small" class="view-btn" @click="onDetail(rowData.row)">
            详情
          </el-button>
          <el-button v-visible="actionList.edit" type="text" size="small" class="view-btn" @click="onEdit(rowData.row)">
            修改
          </el-button>
          <el-button v-visible="actionList.deleted" type="text" size="small" class="view-btn" @click="onDelete(rowData.row)">
            删除
          </el-button>
        </template>
      </wtm-table-box>
    </card>
    <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </div>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./dialog-form.vue";
import store from "@/store/system/frameworkmenu";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, SEARCH_DATA, TABLE_HEADER } from "./config";

@Component({
    mixins: [searchMixin(SEARCH_DATA, TABLE_HEADER), actionMixin],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    // 动作
    assembly = ASSEMBLIES;
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
