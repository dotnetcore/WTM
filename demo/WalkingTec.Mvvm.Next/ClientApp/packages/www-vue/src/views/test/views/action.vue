<template>
  <div class="page-action">
    <a-button type="primary" @click="onInsert">
      <span v-t="'action.insert'" />
    </a-button>
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
    <a-modal title="Title" :visible="visible" @cancel="onVisible(false)">
      <p>aaaaa</p>
    </a-modal>
  </div>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Modal } from "ant-design-vue";
import PageStore from "../store";
@Component
export default class ViewAction extends Vue {
  @Prop() private PageStore: PageStore;
  visible = false;
  mounted() {}
  onVisible(visible = !this.visible) {
    this.visible = visible;
  }
  onInsert() {
    this.onVisible(true);
  }
}
</script>
<style scoped lang="less">
.page-action {
  text-align: right;
}
</style>