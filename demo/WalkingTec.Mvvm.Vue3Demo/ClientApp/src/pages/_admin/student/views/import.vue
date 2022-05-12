
<template>

</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { ExStudentPageController,} from "../controller";
@Options({
    name: "_PageModelsStudentImport",  
    components: {
  },
})
export default class extends Vue {

    readonly ExStudentPageController = ExStudentPageController;

    
    get queryKey() {
      return "modelsstudentimport";
    }

    
    created() {}
    mounted() {}
}
</script>
<style lang="less">
</style>

