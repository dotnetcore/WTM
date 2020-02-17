<template>
  <div class="external-wrap">
    <iframe
      id="mainiframe"
      :src="'?app_id=100042&is_embed=true#/admin/logs'"
      width="100%"
      height="100%"
      frameborder="no"
      border="0"
      marginwidth="0"
      marginheight="0"
      scrolling="auto"
      allowtransparency="yes"
    ></iframe>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
@Component({
  name: "external"
})
export default class extends Vue {}
</script>
