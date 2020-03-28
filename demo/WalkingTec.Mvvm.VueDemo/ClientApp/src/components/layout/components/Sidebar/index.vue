<template>
    <div :class="{ 'has-logo': showLogo }" class="bg-wrap" :style="sidebarStyle">
        <sidebar-logo v-if="showLogo" :collapse="isCollapse" />
        <el-divider class="logo-divider"></el-divider>
        <el-scrollbar wrap-class="scrollbar-wrapper">
            <el-menu :default-active="activeMenu" :collapse="isCollapse" :text-color="variables.menuText" :active-text-color="menuActiveTextColor" :unique-opened="false" :collapse-transition="false" :background-color="variables.menuBg" mode="vertical">
                <sidebar-item v-for="(route, index) in routes" :key="index" :item="route" :base-path="route.path || index+''" :is-collapse="isCollapse" />
            </el-menu>
        </el-scrollbar>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";
import { RoutesModule } from "@/store/modules/routes";
import { SettingsModule } from "@/store/modules/settings";
import SidebarItem from "./SidebarItem.vue";
import SidebarLogo from "./SidebarLogo.vue";
import { style as variables } from "@/config/index";

@Component({
    name: "SideBar",
    components: {
        SidebarItem,
        SidebarLogo
    }
})
export default class extends Vue {
    get sidebar() {
        return AppModule.sidebar;
    }

    get routes() {
        return RoutesModule.routes;
    }

    get showLogo() {
        return SettingsModule.showSidebarLogo;
    }

    get menuActiveTextColor() {
        if (SettingsModule.sidebarTextTheme) {
            return SettingsModule.theme;
        } else {
            return variables.menuActiveText;
        }
    }

    get variables() {
        return variables;
    }

    get activeMenu() {
        const route = this.$route;
        const { meta, path } = route;
        // if set path, the sidebar will highlight the path you set
        if (meta.activeMenu) {
            return meta.activeMenu;
        }
        return path;
    }

    get isCollapse() {
        return !this.sidebar.opened;
    }

    get sidebarStyle() {
        return {
            backgroundImage: `url(${SettingsModule.menuBackgroundImg.image})`,
            backgroundColor: "#304156"
        };
    }
}
</script>

<style lang="less">
@import "~@/assets/css/variable.less";
.el-submenu__title:hover,
.el-menu-item:hover {
    background-color: @subMenuHover !important;
}

.sidebar-container {
    background-size: cover;
    // reset element-ui css
    .horizontal-collapse-transition {
        transition: 0s width ease-in-out, 0s padding-left ease-in-out,
            0s padding-right ease-in-out;
    }

    .scrollbar-wrapper {
        overflow-x: hidden !important;
    }

    .el-scrollbar__view {
        height: 100%;
    }

    .el-scrollbar__bar {
        &.is-vertical {
            right: 0px;
        }

        &.is-horizontal {
            display: none;
        }
    }
}
</style>

<style lang="less" scoped>
.el-scrollbar {
    height: 100%;
}

.has-logo {
    .el-scrollbar {
        height: calc(100% - 50px);
    }
}
.logo-divider {
    margin: 0 auto;
    width: 85%;
    background-color: rgba(180, 180, 180, 0.3);
}
.el-menu {
    border: none;
    height: 100%;
    width: 100% !important;
}

.bg-wrap:after {
    display: block;
    content: "";
    position: absolute;
    width: 100%;
    height: 100%;
    background-color: rgba(27, 27, 27, 0.87);
    opacity: 0.85;
    top: 0;
    left: 0;
    z-index: -1;
}
</style>
