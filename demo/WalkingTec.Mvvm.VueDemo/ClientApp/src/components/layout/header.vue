<template>
  <header class="app-navbar animated slideInDown">
    <nprogress-container />
    <div class="title-container">
      <i :class="[collapse?'el-icon-s-unfold':'el-icon-s-fold' ,collapse?'fold-icon':'unfold-icon','collapse-icon']" @click="toggleFold" />
      <div class="title">
        WTM
      </div>
      <div class="user-panel">
        <span class="user-btn"><i class="fa fa-mobile-phone nav-btn" />{{ userName }}</span>
        <span class="user-btn able"><i class="fa fa-power-off nav-btn" />退出</span>
      </div>
    </div>
  </header>
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
    show: Boolean = false;
    userName: String = "";
    @Mutation("toggleIsFold")
    toggleFold;
    @Getter("isCollapse")
    collapse;

    mounted() {
        this.show = true;
        this.userName = cache.getStorage(config.tokenKey, true).Name || "";
    }
}
</script>
<style lang="less" scoped>
@import "../../assets/css/variable";
@import "../../assets/css/mixin";
.app-navbar {
    position: fixed;
    min-width: 100%;
    z-index: @maxZindex + 2;
    height: @navbarHeight;

    .title-container {
        font-size: 17px;
        font-weight: bolder;
        width: 100%;
        height: 100%;
        position: relative;
        background: #fff;
        box-shadow: 0 -1px 3px #eee;
        border-bottom: 1px solid #eee;
        .collapse-icon {
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
            display: inline-block;
            margin-left: 50%;
            height: 100%;
            .centerBox(@navbarHeight);
        }
        .user-panel {
            position: absolute;
            right: 100px;
            .centerBox(@navbarHeight);
            display: inline-block;
            .user-btn {
                font-size: 13px;
                margin-left: 20px;
                color: #d8d8d8;
                cursor: pointer;
                &.able {
                    transition: all 500ms;
                    &:hover {
                        color: #303133;
                    }
                }
                .nav-btn {
                    margin-right: 15px;

                    // font-size: 18px;
                }
            }
        }
    }
}
</style>
