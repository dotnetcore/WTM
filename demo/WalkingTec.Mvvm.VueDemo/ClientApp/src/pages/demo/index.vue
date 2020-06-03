<template>
    <card class="dataprivilege">
        <el-container style="height: 500px; border: 1px solid #eee">
            <el-aside width="200px" style="background-color: rgb(238, 241, 246)">
                <el-menu :default-openeds="['1', '3']">
                    <el-submenu index="1">
                        <template slot="title"><i class="el-icon-message"></i>导航一</template>
                        <el-menu-item-group>
                            <template slot="title">分组一</template>
                            <el-menu-item index="1-1" @click="onClickTest('admin')">选项1</el-menu-item>
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
                    <wtm-table-box :attrs="{...searchAttrs, actionList}" :events="{...searchEvent, ...actionEvent}">
                    </wtm-table-box>
                </el-main>
            </el-container>
        </el-container>
        <!-- 列表 -->
    </card>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import searchMixin from "@/vue-custom/mixin/search";
import actionMixin from "@/vue-custom/mixin/action-mixin";
import store from "../frameworkuser/store/index";
// 查询参数/列表 ★★★★★
import { ASSEMBLIES, TABLE_HEADER } from "../frameworkuser/config";

@Component({
    name: "demo",
    mixins: [searchMixin(TABLE_HEADER), actionMixin(ASSEMBLIES)],
    store,
    components: {}
})
export default class Index extends Vue {
    searchForm = {
        ITCode: "",
        orderByColumn: null, // 排序字段
        isAsc: null // asc desc
    };
    private onClickTest(item) {
        this.searchForm.ITCode = item;
        this.onSearch();
    }
}
</script>
