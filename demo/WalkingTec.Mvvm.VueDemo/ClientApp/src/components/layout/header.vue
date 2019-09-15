<template>
  <div class="app-navbar animated slideInDown">
    <nprogress-container />
    <div class="title-container">
      <i :class="[collapse?'el-icon-s-unfold':'el-icon-s-fold' ,collapse?'fold-icon':'unfold-icon','collapse-icon']" @click="toggleFold" />
      <div class="title">
        WTM
      </div>
      <div class="user-panel">
        <span class="user-btn" @click="onOpenGlobal"><i class="fa fa-mobile-phone nav-btn" />{{ userName }}</span>
        <span class="user-btn able"><i class="fa fa-power-off nav-btn" />退出</span>
      </div>
    </div>
    <el-drawer title="全局设置" :visible.sync="isGlobalBox" direction="rtl" @close="onGlobalClose">
      <el-form :model="global" class="global-el-form">
        <el-form-item label="弹框类型">
          <el-radio-group v-model="global.dialog">
            <el-radio label="弹框" />
            <el-radio label="抽屉" />
          </el-radio-group>
        </el-form-item>
        <el-form-item label="Tabs页签">
          <el-switch v-model="global.tabs" />
        </el-form-item>
      </el-form>
    </el-drawer>
  </div>
</template>


<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Mutation, Getter } from "vuex-class";
import NprogressContainer from "./nprogress.vue";
import cache from "@/util/cache";
import config from "@/config/index";

@Component({
    components: { NprogressContainer }
})
export default class NavBar extends Vue {
    @Mutation("toggleIsFold")
    toggleFold;
    @Getter("isCollapse")
    collapse;
    userName: String = "";
    isGlobalBox: boolean = false;
    global = {
        dialog: "弹框",
        tabs: false
    };

    mounted() {
        const userGlobal = cache.getCookieJson(config.globalKey) || {};
        this.global = {
            ...this.global,
            ...userGlobal
        };
        this.userName = cache.getStorage(config.tokenKey, true).Name || "";
    }
    onOpenGlobal() {
        this.isGlobalBox = true;
    }
    onGlobalClose() {
        cache.setCookieJson(config.globalKey, this.global);
    }
}
</script>
<style lang="less" scoped>
@import "../../assets/css/variable";
@import "../../assets/css/mixin";
.app-navbar {
    min-width: 100%;
    z-index: @maxZindex + 2;
    color: #fff;
    .title-container {
        .flexbox(space-between);
        .flexjustify(space-between);
        .collapse-icon {
            .centerBox(@navbarHeight);
            cursor: pointer;
            transition: all 500ms ease;
        }
        .unfold-icon {
            margin-left: 220px;
            font-size: 25px;
        }
        .fold-icon {
            margin-left: @foldWith;
        }
        .title {
            font-size: 20px;
            .centerBox(@navbarHeight);
        }
        .user-panel {
            .centerBox(@navbarHeight);
            .user-btn {
                font-size: 13px;
                margin-left: 20px;
                color: #d8d8d8;
                cursor: pointer;
                &.able {
                    transition: all 500ms;
                    &:hover {
                        color: #fff;
                    }
                }
                .nav-btn {
                    margin-right: 15px;
                }
            }
        }
    }
}
.global-el-form {
    padding: 0 20px;
}
</style>
