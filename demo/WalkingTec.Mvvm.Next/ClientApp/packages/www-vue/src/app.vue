<template>
  <div v-if="UserStore.Id&&UserStore.Loading" class="user-loading">
    <a-spin>
      <a-icon slot="indicator" type="loading" style="font-size: 50px" spin />
    </a-spin>
  </div>
  <w-layout v-else-if="UserStore.OnlineState" />
  <Login v-else />
</template>

<script lang="ts">
import { message } from "ant-design-vue";
import { Component, Prop, Vue } from "vue-property-decorator";
import { Basics } from "./views";
import RootStore from "./rootStore";
@Component({
  components: {
    Login: Basics.login
  }
})
export default class App extends Vue {
  UserStore = RootStore.UserStore;
  mounted() {}
  destroyed() {}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
.user-loading {
  position: fixed;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  transition: all 0.2s;
}
</style>
