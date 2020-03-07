<template>
  <card>
    <!-- 操作按钮 -->
    <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
    <!-- 列表 -->
    <wtm-table-box :default-expand-all="true" :row-key="'ID'" :tree-props="{children: 'children'}" :data="treeData" :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
      <template #ICon="rowData">
        <i :class="[rowData.row.ICon]"></i>
      </template>
    </wtm-table-box>
    <!-- 弹出框 -->
    <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
    <!-- 导入 -->
    <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
  </card>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import store from "@/store/system/frameworkmenu";
import DialogForm from "./dialog-form.vue";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, TABLE_HEADER } from "./config";

@Component({
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: { DialogForm }
})
export default class Index extends Vue {
    // tabledata转tree格式
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
