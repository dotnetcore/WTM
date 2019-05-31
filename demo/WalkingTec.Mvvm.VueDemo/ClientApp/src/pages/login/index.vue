<template>
    <div class="main-box clear">
        login ---- index
    </div>
</template>
<script>
import { mapState, mapMutations, mapActions } from "vuex";

const mixin = {
    computed: {
        ...mapState({
            isLogin: "isLogin"
        })
    },
    methods: {
        ...mapMutations({}),
        ...mapActions({})
    }
};
export default {
    mixins: [mixin],
    data() {
        return {};
    },
    mounted() {},
    methods: {},
    computed: {},
    components: {}
};
</script>
<style lang="less">
@import "../../assets/css/variable.less";
@import "../../assets/css/mixin.less";
.main-box {
    background-color: #f2f3f5;
}
</style>
