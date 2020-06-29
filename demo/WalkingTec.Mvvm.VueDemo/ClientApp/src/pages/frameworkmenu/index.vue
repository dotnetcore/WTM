<template>
    <card>
        <!-- 操作按钮 -->
        <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent">
            <el-button v-visible="actionList['refreshMenu']" type="primary" icon="el-icon-refresh-right" @click="onRefreshMenu">
                {{$t('buttom.refresh')}}
            </el-button>
        </wtm-but-box>
        <!-- 列表 -->
        <wtm-table-box :default-expand-all="true" :row-key="'ID'" :tree-props="{children: 'children'}" :data="treeData" :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
            <template #ICon="rowData">
                <wtm-icon :icon="rowData.row.ICon" />
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
import store from "./store/index";
import DialogForm from "./views/dialog-form.vue";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, TABLE_HEADER } from "./config";

@Component({
    name: "frameworkmenu",
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: { DialogForm }
})
export default class Index extends Vue {
    @Action
    refreshMenu;

    // tabledata转tree格式
    get treeData() {
        const list = this["tableData"];
        const getChilders = (pid, children = []) => {
            _.filter(list, ["ParentID", pid]).map(item => {
                const itemChild = getChilders(item.ID, item.children);
                children.push({ ...item, children: itemChild });
            });
            return children;
        };
        const tree = getChilders("", []);
        return tree;
    }

    onRefreshMenu() {
        this.refreshMenu().then(() => {
            this.onSearch();
        });
    }
}
</script>
