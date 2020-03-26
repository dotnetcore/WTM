<template>
    <card>
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" :needCollapse="true" :isActive.sync="isActive" />
        <!-- 操作按钮 -->
        <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
        <!-- 列表 -->
        <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
      <template #FileId="rowData">
        <el-link icon="el-icon-edit" v-if="!!rowData.row.FileId" :href="'/api/_file/downloadFile/'+rowData.row.FileId">下载</el-link>
      </template>


        </wtm-table-box>
        <!-- 弹出框 -->
        <dialog-form :is-show.sync="dialogIsShow" :dialog-data="dialogData" :status="dialogStatus" @onSearch="onHoldSearch" />
        <!-- 导入 -->
        <upload-box :is-show.sync="uploadIsShow" @onImport="onImport" @onDownload="onDownload" />
    </card>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./views/dialog-form.vue";
import store from "./store/index";
// 查询参数, table列 ★★★★★
import { ASSEMBLIES, TABLE_HEADER, SchoolTypeTypes } from "./config";

@Component({
    name: "school",
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    isActive: boolean = false;

    get SEARCH_DATA() {
        return {
            formProps: {
                "label-width": "75px",
                inline: true
            },
            formItem: {
                "SchoolCode":{
                    label: "学校编码",
                    rules: [],
                    type: "input"
            },
                "SchoolName":{
                    label: "学校名称",
                    rules: [],
                    type: "input"
            },
                "SchoolType":{
                    label: "学校类型",
                    rules: [],
                    type: "select",
                    children: SchoolTypeTypes
                    ,isHidden: !this.isActive
            },
                "Remark":{
                    label: "备注",
                    rules: [],
                    type: "input"
                    ,isHidden: !this.isActive
            }

            }
        };
    }

     created() {

    }
}
</script>
