<template>
  <div class="footerbg animated2s slideInUp">
    <div class="copyright">
      Copyright
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
@Component
export default class Footer extends Vue {
    @Prop(Number)
    propA: number | undefined;
    @Prop({ default: "" })
    pageName!: string;
    goPath(path: string, param: any) {
        this.$router.push({
            path: path,
            params: param
        });
    }
}
</script>

<style lang="less">
@import "../../assets/css/mixin.less";
.footerbg {
    position: fixed;
    left: 200px;
    width: 100%;
    text-align: center;
    right: 0;
    bottom: 0;
    height: 44px;
    line-height: 44px;
    padding: 0 15px;
    background-color: #eee;
    border-top: 1px solid #eee;
    border-bottom: 1px solid #eee;
    background-color: hsla(0, 0%, 100%, 0.95);
    box-shadow: 0 -1px 3px #eee;
}
</style>
