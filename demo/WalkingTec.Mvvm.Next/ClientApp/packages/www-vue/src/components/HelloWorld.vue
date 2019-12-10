<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <h1>
      <span>{{ Store.count }}</span>
      <a-button type="link" @click="Store.onAdd()">Add</a-button>
    </h1>
    <h1>
      <span>{{ TimeStore.currentTime }}</span>
      <a-button type="link" @click="onToggleTime()">Toggle</a-button>
    </h1>
  </div>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { observable, action } from "mobx";
import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import { EntitiesUserStore, EntitiesTimeStore } from "@leng/public/src";
class Store {
  @observable
  count = 0;
  @action
  onAdd() {
    this.count++;
  }
}
@observer
@Component
export default class HelloWorld extends Vue {
  Store = new Store();
  TimeStore = new EntitiesTimeStore();
  @Prop() private msg!: string;
  mounted() {
    console.log("TCL: HelloWorld -> mounted -> mounted");
    this.onToggleTime();
  }
  destroyed() {
    console.log("TCL: HelloWorld -> destroyed -> destroyed");
    this.onToggleTime();
  }
  onToggleTime() {
    if (this.TimeStore.onToggleTime()) {
      message.success("计时开始");
    } else {
      message.error("计时结束");
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
</style>
