<template>
      <iframe :src="url" frameborder="0" style="width:100%;height:100%" />
  
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
    import { UserModule } from "@/store/modules/user";
    import { getToken } from "@/util/cookie";


export default class Index extends Vue {
    get url() {
        return "http://localhost:7598/_workflow/inner?access_token=" + getToken()
    }
  get isMainHost() {
    return UserModule.isMainHost;
  }
}
</script>
