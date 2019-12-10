<template>
  <Info v-if="UserStore.OnlineState" />
  <Login v-else />
</template>
<script lang="ts">
import { observable, action } from "mobx";
import { observer } from "mobx-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import Info from "./info.vue";
import Login from "./login.vue";
import UserStore from "./user";
@observer
@Component({
  components: {
    Info,
    Login
  }
})
export default class HelloWorld extends Vue {
  UserStore = UserStore;
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
</style>
