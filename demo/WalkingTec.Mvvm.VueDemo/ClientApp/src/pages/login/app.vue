<template>
  <div class="app-main">
    <index />
  </div>
</template>

<style lang="less">
@import "../../assets/css/mixin.less";
.app-main {
    background-image: url(~static/svg/background.svg);
    background-repeat: no-repeat;
    background-position: center 110px;
    background-size: 100%;
    min-height: 100vh;
    .center();
}
</style>

<script lang="ts">
import index from "./index.vue";
import { mapState, mapMutations, mapActions } from "vuex";
import { Component, Vue } from "vue-property-decorator";

const mixin = {
    computed: {
        ...mapState({})
    },
    methods: {
        ...mapMutations({ setIsLogin: "setIsLogin" }),
        ...mapActions({})
    }
};

@Component({
    components: {
        index
    },
    mixins: [mixin]
})
export default class App extends Vue {}
</script>
