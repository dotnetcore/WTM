<template>
    <section class="left-ment">
        <el-menu default-active="2" class="el-menu-vertical-demo" @open="handleOpen" @close="handleClose" background-color="#393D49" text-color="#fff" active-text-color="#ffd04b">
            <el-submenu index="1">
                <template slot="title">
                    <i class="el-icon-location"></i>
                    <span>导航一</span>
                </template>
                <el-menu-item-group>
                    <template slot="title">分组一</template>
                    <el-menu-item index="1-1">选项1</el-menu-item>
                    <el-menu-item index="1-2">选项2</el-menu-item>
                </el-menu-item-group>
                <el-menu-item-group title="分组2">
                    <el-menu-item index="1-3">选项3</el-menu-item>
                </el-menu-item-group>
                <el-submenu index="1-4">
                    <template slot="title">选项4</template>
                    <el-menu-item index="1-4-1">选项1</el-menu-item>
                </el-submenu>
            </el-submenu>
            <el-menu-item index="2">
                <i class="el-icon-menu"></i>
                <span slot="title">导航二</span>
            </el-menu-item>
            <el-menu-item index="3" disabled>
                <i class="el-icon-document"></i>
                <span slot="title">导航三</span>
            </el-menu-item>
            <el-menu-item index="4">
                <i class="el-icon-setting"></i>
                <span slot="title">导航四</span>
            </el-menu-item>
        </el-menu>
    </section>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
    props: {
        isEmbed: {
            type: Boolean,
            default: false
        }
    },
    computed: {
        ...mapGetters({})
    },
    data() {
        return {
            showLevelbar: true
        };
    },
    methods: {
        handleOpen(key, keyPath) {
            console.log(key, keyPath);
        },
        handleClose(key, keyPath) {
            console.log(key, keyPath);
        }
    },
    components: {}
};
</script>

<style lang="less" scoped>
@import '../../assets/css/variable';
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
