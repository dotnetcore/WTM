<template>
  <router-view v-slot="{ Component }">
    <!-- <transition name="fade"> -->
    <!-- <keep-alive> -->
    <component :is="Component" />
    <!-- </keep-alive> -->
    <!-- </transition> -->
  </router-view>
  <!-- <router-view v-else /> -->
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
  keep = true;
  onEdit() {}
  created() {}
}
</script>
<style lang="less"></style>
