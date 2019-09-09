<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item v-for="(item,index) in levelList" :key="index">
        <span v-if="item.name">
          <span v-if="index==0" class="no-direct">
            {{ item.name }}
          </span>
          <span v-else-if="index=== levelList.length - 1" class="no-direct current">
            {{ item.name }}
          </span>
          <router-link v-else :to="item.path" class="click-router">
            {{ item.name }}
          </router-link>
        </span>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>
<script lang='ts'>
import { Component, Vue, Watch } from "vue-property-decorator";
import { Route } from "vue-router";
import { pathList } from "@/config/crumb";
type paramType = {
    path: any;
} & Route;
@Component
export default class Breadcrumd extends Vue {
    levelList: Route[] | undefined = [];
    @Watch("$route")
    listenRoute() {
        this.getBreadcrumb();
    }
    mounted() {
        this.getBreadcrumb();
    }
    get name() {
        return this.$route.name;
    }
    getBreadcrumb() {
        const query = this.$route.query;
        const matched = [this.$route];
        let meta = this.$route.meta;
        while (meta && meta.parentMenu) {
            const param: paramType = {
                ...meta.parentMenu
            };
            if (pathList.indexOf(param.path) > -1) {
                const queryList = Object.keys(query).map(item => {
                    return item + "=" + query[item];
                });
                param.path = param.path + "?" + queryList.join("&");
            }
            matched.unshift(param);
            meta = meta.parentMenu.meta;
        }
        this.levelList = matched;
    }
}
</script>
<style lang="less" scoped>
@import "../../assets/css/transition";
.app-breadcrumb {
    padding: 16px;
    .no-direct {
        color: #c0c4cc;
        &.current {
            color: #0d849a;
            text-decoration: underline;
        }
    }
    a.click-router {
        color: #c0c4cc !important;
        cursor: pointer !important;
        text-decoration: underline;
        &:hover {
            color: #0d849a !important;
        }
    }
}
</style>

