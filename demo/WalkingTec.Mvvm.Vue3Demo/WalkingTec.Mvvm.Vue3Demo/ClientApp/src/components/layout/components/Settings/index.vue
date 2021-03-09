<template>
    <div>
        <el-tooltip :content="$t('navbar.set')" effect="dark" placement="bottom">
            <i class="el-icon-setting" @click="settingShow = true" />
        </el-tooltip>
        <el-drawer :with-header="false" :modal-append-to-body="false" style="z-index: 2007" :visible.sync="settingShow" direction="rtl" size="260px">
            <div class="drawer-container">
                <div>
                    <h3 class="drawer-title">
                        {{ $t("settings.title") }}
                    </h3>

                    <div class="drawer-item">
                        <span>{{ $t("settings.theme") }}</span>
                        <theme-picker style="float: right;height: 26px;margin: -3px 8px 0 0;" @change="themeChange" />
                    </div>

                    <div class="drawer-item">
                        <span>{{ $t("settings.showTagsView") }}</span>
                        <el-switch v-model="showTagsView" class="drawer-switch" />
                    </div>

                    <div class="drawer-item">
                        <span>{{ $t("settings.showSidebarLogo") }}</span>
                        <el-switch v-model="showSidebarLogo" class="drawer-switch" />
                    </div>

                    <div class="drawer-item">
                        <span>{{ $t("settings.fixedHeader") }}</span>
                        <el-switch v-model="fixedHeader" class="drawer-switch" />
                    </div>

                    <div class="drawer-item">
                        <span>{{ $t("settings.sidebarTextTheme") }}</span>
                        <el-switch v-model="sidebarTextTheme" class="drawer-switch" />
                    </div>
                    <div class="drawer-item">
                        <span>Tabs</span>
                        <el-switch v-model="isDialog" class="drawer-switch" />
                    </div>
                    <div class="drawer-item">
                        <span>Menu Img</span>
                        <ul class="dropdown-menu">
                            <li v-for="item in sidebarImages" :key="item.key" :class="{ active: item.key === menuBackgroundImg.key }" @click="changeSidebarImage(item)">
                                <a class="img-holder switch-trigger">
                                    <img :src="item.image" alt="" />
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </el-drawer>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { SettingsModule } from "@/store/modules/settings";
import ThemePicker from "@/components/frame/ThemePicker/index.vue";

@Component({
    name: "Settings",
    components: {
        ThemePicker
    }
})
export default class extends Vue {
    private settingShow = false;

    sidebarImages: any[] = [
        { image: require("static/img/sidebar-1.jpg"), key: "sidebar-1" },
        { image: require("static/img/sidebar-2.jpg"), key: "sidebar-2" },
        { image: require("static/img/sidebar-3.jpg"), key: "sidebar-3" },
        { image: require("static/img/sidebar-4.jpg"), key: "sidebar-4" }
    ];

    get fixedHeader() {
        return SettingsModule.fixedHeader;
    }

    set fixedHeader(value) {
        SettingsModule.ChangeSetting({ key: "fixedHeader", value });
    }

    get showTagsView() {
        return SettingsModule.showTagsView;
    }

    set showTagsView(value) {
        SettingsModule.ChangeSetting({ key: "showTagsView", value });
    }

    get showSidebarLogo() {
        return SettingsModule.showSidebarLogo;
    }

    set showSidebarLogo(value) {
        SettingsModule.ChangeSetting({ key: "showSidebarLogo", value });
    }

    get sidebarTextTheme() {
        return SettingsModule.sidebarTextTheme;
    }

    set sidebarTextTheme(value) {
        SettingsModule.ChangeSetting({ key: "sidebarTextTheme", value });
    }
    get isDialog() {
        return SettingsModule.isDialog;
    }

    set isDialog(value) {
        SettingsModule.ChangeSetting({ key: "isDialog", value });
    }

    get menuBackgroundImg() {
        return SettingsModule.menuBackgroundImg;
    }

    private themeChange(value: string) {
        SettingsModule.ChangeSetting({ key: "theme", value });
    }

    private changeSidebarImage(item) {
        SettingsModule.ChangeSetting({
            key: "menuBackgroundImg",
            value: item
        });
    }
}
</script>

<style lang="less" scoped>
.drawer-container {
    padding: 24px;
    font-size: 14px;
    line-height: 1.5;
    word-wrap: break-word;

    .drawer-title {
        margin-bottom: 12px;
        color: rgba(0, 0, 0, 0.85);
    }

    .drawer-item {
        color: rgba(0, 0, 0, 0.65);
        font-size: 14px;
        padding: 12px 0;

        .dropdown-menu {
            li {
                padding: 18px 2px;
                width: 25%;
                float: left;
                &.active > a.img-holder {
                    border-color: #00bbff;
                    background-color: #ffffff;
                }
                a.img-holder {
                    border-radius: 10px;
                    background-color: #fff;
                    border: 3px solid #fff;
                    padding-left: 0;
                    padding-right: 0;
                    opacity: 1;
                    cursor: pointer;
                    display: block;
                    max-height: 100px;
                    overflow: hidden;
                    padding: 0;
                    min-width: 25%;
                }
                a.img-holder img {
                    border-radius: 0;
                    width: 100%;
                    height: 100px;
                    margin: 0 auto;
                }
                //transparent
            }
        }
    }

    .drawer-switch {
        float: right;
    }
}
</style>
