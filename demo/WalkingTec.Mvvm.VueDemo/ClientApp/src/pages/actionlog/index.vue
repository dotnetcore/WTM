<template>
    <card>
        <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" :needCollapse="true" :isActive.sync="isActive" />
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

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import DialogForm from "./views/dialog-form.vue";
import store from "./store/index";
// 查询参数, table列 ★★★★★
import { ASSEMBLIES, TABLE_HEADER, logTypes } from "./config";

@Component({
    name: "actionlog",
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
                "label-width": this.$t("actionlog.LabelWidth"),
                inline: true
            },
            formItem: {
                ITCode: {
                    type: "input",
                    label: "ITCode"
                },
                ActionUrl: {
                    type: "input",
                    label: "Url"
                },
                ActionTime: {
                    type: "datePicker",
                    label: this.$t("actionlog.ActionTime"),
                    span: 12,
                    props: {
                        type: "datetimerange",
                        "value-format": "yyyy-MM-dd HH:mm:ss",
                        "range-separator": "-",
                        "start-placeholder": this.$t(
                            "actionlog.StartPlaceholder"
                        ),
                        "end-placeholder": this.$t("actionlog.EndPlaceholder")
                    },
                    isHidden: !this.isActive
                },
                IP: {
                    type: "input",
                    label: "IP",
                    isHidden: !this.isActive
                },
                LogType: {
                    type: "select",
                    label: this.$t("actionlog.LogType"),
                    children: logTypes,
                    props: {
                        multiple: true,
                        "collapse-tags": true
                    },
                    isHidden: !this.isActive
                }
            }
        };
    }

    // 查询接口 ★★★★★
    privateRequest(params) {
        if (params.ActionTime) {
            params.StartActionTime = params.ActionTime[0];
            params.EndActionTime = params.ActionTime[1];
        }
        return this.search(params);
    }
}
</script>
