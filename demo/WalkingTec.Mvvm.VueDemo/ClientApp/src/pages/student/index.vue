<template>
    <card>
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" :needCollapse="true" :isActive.sync="isActive" />
        <!-- 操作按钮 -->
        <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
        <!-- 列表 -->
        <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
      <template #PhotoId="rowData">
        <el-image v-if="!!rowData.row.PhotoId" style="width: 100px; height: 100px" :src="'/api/_file/downloadFile/'+rowData.row.PhotoId" fit="cover" />
      </template>

      <template #IsValid="rowData">
        <el-switch :value="rowData.row.IsValid === 'true' || rowData.row.IsValid === true" disabled />
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
import { ASSEMBLIES, TABLE_HEADER, SexTypes } from "./config";
import i18n from "@/lang";

@Component({
    name: "student",
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: {
        DialogForm
    }
})
export default class Index extends Vue {
    isActive: boolean = false;
    @Action
    getMajor;
    @State
    getMajorData;

    get SEARCH_DATA() {
        return {
            formProps: {
                "label-width": "75px",
                inline: true
            },
            formItem: {
                "Name":{
                    label: "姓名",
                    rules: [],
                    type: "input"
              },
                "Sex":{
                    label: "性别",
                    rules: [],
                    type: "select",
                    children: SexTypes,
                    props: {
                        clearable: true,
                        placeholder: this.$t("form.all")
                    }
              },
                "ZipCode":{
                    label: "邮编",
                    rules: [],
                    type: "input"
                    ,isHidden: !this.isActive
              },
                "SelectedStudentMajorIDs":{
                    label: "专业",
                    rules: [],
                    type: "select",
                    children: this.getMajorData,
                    props: {
                        clearable: true ,
                        multiple: true,
                        "collapse-tags": true
                    }
                    ,isHidden: !this.isActive
              },

            }
        };
    }

     mounted() {
        this.getMajor();

    }
}
</script>
