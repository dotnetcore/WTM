<template>
    <card>
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" />
        <!-- 操作按钮 -->
        <wtm-but-box :assembly="assembly" :action-list="actionList" :selected-data="selectData" :events="actionEvent" />
        <!-- 列表 -->
        <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}" />
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
    name: "dataprivilege",
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: { DialogForm }
})
export default class Index extends Vue {
    @Action
    deleted;
    @Action
    getPrivileges;
    @State
    getPrivilegesData;

    get SEARCH_DATA() {
        return {
            formProps: {
                "label-width": this.$t("dataprivilege.LabelWidth"),
                inline: true
            },
            formItem: {
                TableName: {
                    type: "select",
                    label: this.$t("dataprivilege.TableName"),
                    props: {
                        clearable: true
                    },
                    children: this.getPrivilegesData
                },
                DpType: {
                    type: "radioGroup",
                    label: this.$t("dataprivilege.DpType"),
                    span: 12,
                    children: [
                        {
                            Value: 0,
                            Text: this.$t("dataprivilege.UserGroup")
                        },
                        {
                            Value: 1,
                            Text: this.$t("dataprivilege.UserRights")
                        }
                    ]
                }
            }
        };
    }

    created() {
        this.getPrivileges();
    }

    /**
     * 修改
     * @param data
     */
    onEdit(data) {
        this.openDialog(this.$actionType.edit, data);
    }
    /**
     * 详情
     * @param data
     */
    onDetail(data) {
        this.openDialog(this.$actionType.detail, data);
    }
    /**
     * 单个删除
     * @param params
     */
    onDelete(params) {
        this.onConfirm().then(() => {
            const parameters = {
                ModelName: params.TableName,
                Id: params.TargetId,
                Type: params.DpType
            };
            this.deleted(parameters).then(res => {
                this["$notify"]({
                    title: this.$t("dataprivilege.SuccessfullyDeleted"),
                    type: "success"
                });
                this["onHoldSearch"]();
            });
        });
    }
}
</script>
