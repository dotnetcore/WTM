<template>
  <div class="external-wrap" style="height: 100%;">
    <iframe id="mainiframe" :src="url" width="100%" style="height: 100%;" frameborder="no" border="0" marginwidth="0" marginheight="0" scrolling="auto" allowtransparency="yes"></iframe>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
@Component({
    name: "external"
})
export default class extends Vue {
    @Prop({ type: String, default: "/" })
    url;
}
</script>
