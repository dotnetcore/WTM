<template>
  <div>
    <!-- Page Action-->
    <div v-if="isPageAction" class="page-action">
      <a-button @click="onInsert">
        <a-icon type="plus" />
        <span v-t="'action.insert'" />
      </a-button>
      <a-divider type="vertical" />
      <a-button @click="onInsert">
        <a-icon type="edit" />
        <span v-t="'action.update'" />
      </a-button>
      <a-divider type="vertical" />
      <a-button @click="onInsert">
        <a-icon type="delete" />
        <span v-t="'action.delete'" />
      </a-button>
      <a-divider type="vertical" />
      <a-button @click="onInsert">
        <a-icon type="cloud-upload" />
        <span v-t="'action.import'" />
      </a-button>
      <a-divider type="vertical" />
      <a-dropdown>
        <a-menu slot="overlay">
          <a-menu-item key="1">1st item</a-menu-item>
          <a-menu-item key="2">2nd item</a-menu-item>
        </a-menu>
        <a-button>
          <span v-t="'action.export'" />
          <a-icon type="down" />
        </a-button>
      </a-dropdown>
    </div>
    <!-- 行 数据  Action-->
    <div v-else class="row-action">
      <a-button type="link" size="small" @click="onInsert">
        <a-icon type="eye" />
      </a-button>
      <a-button type="link" size="small" @click="onInsert">
        <a-icon type="edit" />
      </a-button>
      <a-popconfirm title="Are you sure delete this task?" okText="Yes" cancelText="No">
        <a-button type="link" size="small">
          <a-icon type="delete" />
        </a-button>
      </a-popconfirm>
    </div>
    <a-modal title="Title" :visible="visible" @cancel="onVisible(false)">
      <p>aaaaa</p>
    </a-modal>
  </div>
</template> 
<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import { Modal } from "ant-design-vue";
import PageStore from "../store";
import lodash from "lodash";
@Component
export default class ViewAction extends Vue {
  @Prop() private PageStore: PageStore;
  visible = false;
  get isPageAction() {
    return !lodash.has(this, "params.rowIndex");
  }
  mounted() {
  }
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