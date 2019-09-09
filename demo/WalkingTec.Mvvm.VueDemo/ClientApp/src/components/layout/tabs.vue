<template>
  <el-tabs v-model="tabsValue" type="card" closable @tab-remove="removeTab" @tab-click="onTab">
    <el-tab-pane v-for="(item, index) in tabList" :key="index" :label="item.name" :name="item.path" />
  </el-tabs>
</template>

<script lang='ts'>
import { Component, Vue, Watch } from "vue-property-decorator";
@Component
export default class Tabs extends Vue {
    @Watch("$route")
    routeChange() {
        const matched = this["$route"];
        this.addTab({
            path: matched.path,
            name: matched.name
        });
    }
    // 当前
    tabsValue: String = "";
    // 列表
    tabList: Object[] = [];
    // 删除
    removeTab(path) {
        let tabs = this.tabList;
        if (this.tabsValue === path) {
            tabs.forEach((tab, index) => {
                if (tab["path"] === path) {
                    let nextTab = tabs[index + 1] || tabs[index - 1];
                    if (nextTab) {
                        this.tabsValue = nextTab["path"];
                        this.tabList.push(this.tabsValue);
                    }
                }
            });
        }
        this.tabList = tabs.filter(item => item["path"] !== path);
    }
    // 添加
    addTab(item) {
        this.tabsValue = item.path;
        const list = _.find(this.tabList, ["path", item.path]);
        if (!list) {
            this.tabList.push(item);
        }
    }
    // 选中
    onTab(item, event) {
        this["$router"].push(item.name);
    }
}
</script>

<style lang="less" scoped>
</style>
