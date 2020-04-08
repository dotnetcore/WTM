<template>
  <span>
    <a href="javascript:;" class="user-item" @click="showDrawer">
      <a-icon type="setting" />
    </a>
    <a-drawer
      title="Setting"
      placement="right"
      :closable="false"
      @close="onClose"
      :visible="visible"
      :afterVisibleChange="afterVisibleChange"
    >
      <p>Some contents...</p>
      <p>Some contents...</p>
      <p>Some contents...</p>
    </a-drawer>
  </span>
</template>

<script>
export default {
    data() {
      return {
        visible: false,
      };
    },
    methods: {
      afterVisibleChange(val) {
        console.log('visible', val)
      },
      showDrawer() {
        this.visible = true;
      },
      onClose() {
        this.visible = false;
      },
    },
};
</script>

<style>
</style>