<template>
    <div class="app-main">
        sdf login
        <index></index>
    </div>
</template>

<style lang="less">
.app-main {
    // background-image: url(/static/media/background.svg);
    background-repeat: no-repeat;
    background-position: center 110px;
    background-size: 100%;
    min-height: 100vh;
}
</style>

<script>
import index from "./index";
import { mapState, mapMutations, mapActions } from "vuex";

const mixin = {
    computed: {
        ...mapState({})
    },
    methods: {
        ...mapMutations({ setIsLogin: "setIsLogin" }),
        ...mapActions({})
    }
};

export default {
    components: {
        index
    },
    mixins: [mixin],
    data() {
        return {};
    }
};
</script>
