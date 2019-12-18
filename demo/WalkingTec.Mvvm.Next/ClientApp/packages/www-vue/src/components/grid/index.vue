<template>
  <div>
    <grid />
    <pagination />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import pagination from "./pagination.vue";
const grid = () =>
  ({
    // 需要加载的组件 (应该是一个 `Promise` 对象)
    component: import("./grid.vue"),
    // 异步组件加载时使用的组件
    //   loading: LoadingComponent,
    // 加载失败时使用的组件
    //   error: ErrorComponent,
    // 展示加载时组件的延时时间。默认值是 200 (毫秒)
    //   delay: 200,
    // 如果提供了超时时间且组件加载也超时了，
    // 则使用加载失败时使用的组件。默认值是：`Infinity`
    timeout: 3000
  } as any);
@Component({
  components: {
    grid,
    pagination
  }
})
export default class HelloWorld extends Vue {
  @Prop() private msg!: string;
  mounted() {
    console.dir(this.$el)

    console.log("TCL: HelloWorld -> mounted -> mounted");
  }
  destroyed() {
    console.log("TCL: HelloWorld -> destroyed -> destroyed");
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
</style>
