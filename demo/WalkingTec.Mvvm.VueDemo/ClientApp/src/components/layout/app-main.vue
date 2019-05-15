<template>
    <section class="app-main" :style="[]">
        <router-view class="view animated"></router-view>
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
        ...mapGetters({
            layout: 'layout'
        })
    },
    data() {
        return {
            showLevelbar: true
        };
    },
    methods: {},
    components: {}
};
</script>

<style lang="less" scoped>
@import '../../assets/css/variable';
.app-main {
    //height: 100%;
}
</style>
