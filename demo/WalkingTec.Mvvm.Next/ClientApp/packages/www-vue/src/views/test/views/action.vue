<template>
  <div class="page-action">
    <a-button type="primary"><span v-t="'action.insert'" /></a-button>
    <a-divider type="vertical" />
    <a-dropdown>
      <a-menu slot="overlay">
        <a-menu-item key="1">1st item</a-menu-item>
        <a-menu-item key="2">2nd item</a-menu-item>
        <a-menu-item key="3">3rd item</a-menu-item>
      </a-menu>
      <a-button>
        Actions
        <a-icon type="down" />
      </a-button>
    </a-dropdown>
  </div>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import PageStore from "../store";
@Component
export default class ViewAction extends Vue {
  @Prop() private PageStore: PageStore;
  mounted() {}
}
</script>
<style scoped lang="less">
.page-action {
  text-align: right;
}
</style>