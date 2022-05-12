
<template>
  <BatchFormViewBatchEdit />

</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { ExStudentPageController,} from "../controller";
import BatchFormViewBatchEdit from "./widgets/batchFormViewBatchEdit.vue";
@Options({
    name: "_PageModelsStudentBatchEdit",  
    components: {
    BatchFormViewBatchEdit,
  },
})
export default class extends Vue {

    readonly ExStudentPageController = ExStudentPageController;

    
    get queryKey() {
      return "modelsstudentbatchedit";
    }

    
    created() {}
    mounted() {}
}
</script>
<style lang="less">
</style>

