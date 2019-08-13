<template>
  <header class="app-navbar animated slideInDown">
    <nprogress-container />
    <div class="title-container">
      <!-- <i :class="[collapse?'el-icon-s-fold':'el-icon-s-unfold' ,collapse?'fold-icon':'unfold-icon','collapse-icon']" @click="toggleFold" /> -->
      <div class="title">
        NSP
      </div>
      <div class="user-panel">
        <span class="user-btn"><i class="fa fa-mobile-phone nav-btn" />17988837649</span>
        <span class="user-btn able"><i class="fa fa-power-off nav-btn" />退出</span>
      </div>
    </div>
  </header>
</template>


<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";
import { Mutation, Getter } from "vuex-class";
import NprogressContainer from "./nprogress.vue";
@Component({
    components: { NprogressContainer }
})
export default class NavBar extends Vue {
    show: boolean = false;
    @Mutation("toggleIsFold")
    toggleFold;
    @Getter("isCollapse")
    collapse;
    mounted() {
        this.show = true;
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
