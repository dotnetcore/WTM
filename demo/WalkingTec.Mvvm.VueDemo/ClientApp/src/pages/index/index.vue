<template>
    <div class="main-box clear">
        index
    </div>
</template>
<script>
import { mapState, mapMutations, mapActions } from "vuex";

const mixin = {
    computed: {
        ...mapState({})
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
    mounted() {
        console.log("idnex");
    },
    methods: {},
    computed: {},
    components: {}
};
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
@import "~@/assets/css/mixin.less";

.main-box {
    background-color: #f2f3f5;
}
</style>
