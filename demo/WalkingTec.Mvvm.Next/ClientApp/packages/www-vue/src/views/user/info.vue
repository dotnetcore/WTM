<template>
  <a-card title="用户面板" style="width: 300px">
    <a-button @click="UserStore.onOutLogin()" type="link" slot="extra">退出</a-button>
    <p>Name: {{UserStore.Name}}</p>
    <p>Birthday: {{UserStore.Birthday.toLocaleDateString()}}</p>
    <p>Age: {{UserStore.Age}}</p>
    <p>
      Avatar: 
      <a-avatar :src="UserStore.Avatar" />
    </p>
    <p>Address: {{UserStore.Address}}</p>
  </a-card>
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { observable, action } from "mobx";
import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import UserStore from "./user";
@observer
@Component
export default class HelloWorld extends Vue {
  UserStore = UserStore;
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
</style>
