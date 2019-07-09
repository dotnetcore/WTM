<template>
  <section class="left-ment">
    <el-menu default-active="2" class="el-menu-vertical-demo" background-color="#393D49" text-color="#fff" active-text-color="#ccc" :router="true" @open="handleOpen" @close="handleClose">
      <el-menu-item v-for="(item, index) in menuItems" :key="index" :index="item.path">
        <i :class="[item.meta.icon]" />
        <span>{{ item.name }}</span>
      </el-menu-item>

      <el-submenu index="1">
        <template slot="title">
          <i class="el-icon-location" />
          <span>test2</span>
        </template>
        <el-menu-item-group title="分组2">
          <el-menu-item index="1-3">
            选项3
          </el-menu-item>
        </el-menu-item-group>
        <el-submenu index="1-4">
          <template slot="title">
            选项4
          </template>
          <el-menu-item index="1-4-1">
            选项1
          </el-menu-item>
        </el-submenu>
      </el-submenu>
    </el-menu>
  </section>
</template>

<script>
import { mapGetters } from "vuex";

export default {
    props: {
        isEmbed: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            showLevelbar: true
        };
    },
    computed: {
        ...mapGetters(["menuItems"])
    },
    methods: {
        handleOpen(key, keyPath) {
            console.log(key, keyPath);
        },
        handleClose(key, keyPath) {
            console.log(key, keyPath);
        }
    }
};
</script>

<style lang="less" scoped>
@import "../../assets/css/variable";
.left-ment {
    width: 200px;
    // height: 100%;
    display: inline-block;
    position: fixed;
    left: 0;
    top: 60px;
    bottom: 0;
    z-index: 999;
    overflow-x: hidden;
    .el-menu-vertical-demo {
        height: 100%;
    }
}
</style>
