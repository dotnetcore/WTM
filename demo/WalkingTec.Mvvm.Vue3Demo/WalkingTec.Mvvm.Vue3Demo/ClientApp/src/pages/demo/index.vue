<template>
    <card class="dataprivilege">
        <el-container style="height: 500px; border: 1px solid #eee">
            <el-aside width="200px" style="background-color: rgb(238, 241, 246)">
                <el-menu :default-openeds="['1', '3']">
                    <el-submenu index="1">
                        <template slot="title"><i class="el-icon-message"></i>导航一</template>
                        <el-menu-item-group>
                            <template slot="title">分组一</template>
                            <el-menu-item index="1-1" @click="onTest404()">404测试</el-menu-item>
                            <el-menu-item index="1-2">选项2</el-menu-item>
                        </el-menu-item-group>
                        <el-menu-item-group title="分组2">
                            <el-menu-item index="1-3">选项3</el-menu-item>
                        </el-menu-item-group>
                        <el-submenu index="1-4">
                            <template slot="title">选项4</template>
                            <el-menu-item index="1-4-1">选项4-1</el-menu-item>
                        </el-submenu>
                    </el-submenu>
                </el-menu>
            </el-aside>
            <el-container>
                <el-main>
                    <div>lang: <LangSelect /></div>
                    <wtm-search-box :ref="searchRefName" :events="searchEvent" :formOptions="SEARCH_DATA" :needCollapse="true" collapseShowSpan="18" />
                    <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
                    </wtm-table-box>
                </el-main>
            </el-container>
        </el-container>
        <!-- 列表 -->
    </card>
</template>

<script lang='ts'>
import LangSelect from "@/components/frame/LangSelect/index.vue";
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import store from "../actionlog/store/index";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, TABLE_HEADER, logTypes, Provinces } from "./config";

@Component({
    name: "demo",
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: {
        LangSelect
    }
})
export default class Index extends Vue {
    @Action
    testApi;

    searchForm = {
        ITCode: "",
        orderByColumn: null, // 排序字段
        isAsc: null // asc desc
    };

    citys = [];

    private onTest404(item) {
        this.testApi()
    }

    privateRequest(params) {
        console.log('筛选参数', params)
        return this.search(params);
    }

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
                ActionTime: {
                    type: "datePicker",
                    label: this.$t("actionlog.ActionTime"),
                    span: 12,
                    props: {
                        type: "datetimerange",
                        // "value-format": "yyyy-MM-dd HH:mm:ss",
                        "value-format": "timestamp",
                        "range-separator": "-",
                        "start-placeholder": this.$t(
                            "actionlog.StartPlaceholder"
                        ),
                        "end-placeholder": this.$t("actionlog.EndPlaceholder")
                    },
                    defaultValue: [new Date().getTime(), new Date().getTime() + 60 * 60 * 1000]
                },
                LogType: {
                    type: "select",
                    label: this.$t("actionlog.LogType"),
                    children: logTypes,
                    props: {
                        multiple: true,
                        "collapse-tags": true
                    }
                },
                Province: {
                    type: "select",
                    label: this.$t("actionlog.Province"),
                    children: Provinces,
                    events: {
                        change: this.onPrivileges
                    }
                },
                City: {
                    type: "select",
                    label: this.$t("actionlog.City"),
                    children: this.citys
                }
            }
        };
    }
    /**
     * select 组件 change 事件
     * 实际使用方式可参考：dataprivilege模块
     */
    onPrivileges(val) {
        const data =Provinces.find(item => item.Value === val)
        setTimeout(() => {
            this.citys = data.children;
        }, 200)
    }


}
</script>
