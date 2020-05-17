<template>
    <div class="navbar">
        <hamburger id="hamburger-container" :is-active="sidebar.opened" class="hamburger-container" @toggleClick="toggleSideBar" />
        <breadcrumb id="breadcrumb-container" class="breadcrumb-container" />
        <div class="right-menu">
            <template v-if="device!=='mobile'">
                <header-search class="right-menu-item" />
                <error-log class="errLog-container right-menu-item hover-effect" />
                <screenfull class="right-menu-item hover-effect" />
                <size-select class="right-menu-item hover-effect" />
                <lang-select class="right-menu-item hover-effect" />
                <settings class="right-menu-item hover-effect" />
            </template>
            <el-dropdown class="avatar-container right-menu-item hover-effect">
                <div class="avatar-wrapper">
                    <img :src="photoUrl" class="user-avatar">
                </div>
                <el-dropdown-menu slot="dropdown">
                    <router-link to="/">
                        <el-dropdown-item>
                            {{ $t('navbar.dashboard') }}
                        </el-dropdown-item>
                    </router-link>
                    <a v-if="isDev" target="_blank" href="https://github.com/dotnetcore/WTM">
                        <el-dropdown-item>
                            {{ $t('navbar.github') }}
                        </el-dropdown-item>
                    </a>
                    <a v-if="isDev" target="_blank" href="https://wtmdoc.walkingtec.cn/">
                        <el-dropdown-item>{{$t('navbar.doc')}}</el-dropdown-item>
                    </a>
                    <a v-if="isDev" target="_blank" href="/_codegen?ui=vue">
                        <el-dropdown-item>{{$t('navbar.generation')}}</el-dropdown-item>
                    </a>
                    <a v-if="isDev" target="_blank" href="/swagger">
                        <el-dropdown-item>{{$t('navbar.api')}}</el-dropdown-item>
                    </a>
                    <a @click="onOpenPassword">
                        <el-dropdown-item>{{$t('navbar.password')}}</el-dropdown-item>
                    </a>
                    <el-dropdown-item divided>
                        <span style="display:block;" @click="logout">{{ $t('navbar.logOut') }}</span>
                    </el-dropdown-item>
                </el-dropdown-menu>
            </el-dropdown>
        </div>
        <password ref="password" />
    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";
import { UserModule } from "@/store/modules/user";
import Breadcrumb from "@/components/frame/Breadcrumb/index.vue";
import ErrorLog from "@/components/frame/ErrorLog/index.vue";
import Hamburger from "@/components/frame/Hamburger/index.vue";
import HeaderSearch from "@/components/frame/HeaderSearch/index.vue";
import LangSelect from "@/components/frame/LangSelect/index.vue";
import Screenfull from "@/components/frame/Screenfull/index.vue";
import SizeSelect from "@/components/frame/SizeSelect/index.vue";
import Password from "@/components/frame/Password/index.vue";
import Settings from "../Settings/index.vue";
import config from "@/config/index";

@Component({
    name: "Navbar",
    components: {
        Breadcrumb,
        ErrorLog,
        Hamburger,
        HeaderSearch,
        LangSelect,
        Screenfull,
        SizeSelect,
        Settings,
        Password
    }
})
export default class extends Vue {
    get isDev() {
        return config.development;
    }

    get sidebar() {
        return AppModule.sidebar;
    }

    get device() {
        return AppModule.device.toString();
    }

    get avatar() {
        return UserModule.info["PhotoId"];
    }

    get photoUrl() {
        if (this.avatar) {
            return "/api/_file/downloadFile/" + this.avatar;
        } else {
            return "static/img/user.png";
        }
    }

    private toggleSideBar() {
        AppModule.ToggleSideBar(false);
    }

    private async logout() {
        await UserModule.LogOut();
        this.$router.push(`/login.html?redirect=${this.$route.fullPath}`);
    }

    private onOpenPassword() {
        this.$refs["password"].onOpen();
    }
}
</script>

<style lang="less" scoped>
.navbar {
    height: 50px;
    overflow: hidden;
    position: relative;
    background: #fff;
    box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);

    .hamburger-container {
        line-height: 46px;
        height: 100%;
        float: left;
        padding: 0 15px;
        cursor: pointer;
        transition: background 0.3s;
        -webkit-tap-highlight-color: transparent;

        &:hover {
            background: rgba(0, 0, 0, 0.025);
        }
    }

    .breadcrumb-container {
        float: left;
    }

    .errLog-container {
        display: inline-block;
        vertical-align: top;
    }

    .right-menu {
        float: right;
        height: 100%;
        line-height: 50px;
        font-size: 16px;
        &:focus {
            outline: none;
        }
        .right-menu-item {
            display: inline-block;
            padding: 0 8px;
            height: 100%;
            color: #5a5e66;
            vertical-align: text-bottom;

            &.hover-effect {
                cursor: pointer;
                transition: background 0.3s;

                &:hover {
                    background: rgba(0, 0, 0, 0.025);
                }
            }
        }

        .avatar-container {
            margin-right: 30px;

            .avatar-wrapper {
                margin-top: 5px;
                position: relative;

                .user-avatar {
                    cursor: pointer;
                    width: 40px;
                    height: 40px;
                    border-radius: 10px;
                }

                .el-icon-caret-bottom {
                    cursor: pointer;
                    position: absolute;
                    right: -20px;
                    top: 25px;
                    font-size: 12px;
                }
            }
        }
    }
}
</style>
