<template>
  <div id="app">
    <router-view />
    <!-- <service-worker-update-popup /> -->
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";

@Component({
    name: "App"
})
export default class extends Vue {}
</script>
