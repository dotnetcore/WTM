<template>
  <div class="app-main-log">
    <index />
  </div>
</template>
<script lang="ts">
import index from "./login/index.vue";
import { Component, Vue } from "vue-property-decorator";

@Component({
    components: {
        index
    }
})
export default class App extends Vue {}
</script>

<style lang="less" scoped>
@import "~@/assets/css/reset.less";
@import "~@/assets/css/mixin.less";
.app-main-log {
    background: linear-gradient(
        360deg,
        rgba(37, 39, 42, 0) 0%,
        rgba(5, 6, 6, 0.85) 87%,
        rgba(0, 0, 0, 1) 100%
    );
    background-image: url(~static/svg/background.svg);
    background-repeat: no-repeat;
    background-size: cover;
    min-height: 100vh;
    min-width: 1024px;
    .center();
}
</style>
