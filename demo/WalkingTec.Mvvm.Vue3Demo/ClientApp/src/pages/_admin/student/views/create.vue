
<template>
  <div>
    <FormViewCreateAdd @refreshGrid="refreshGridStudent" />

  </div>

</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { ExStudentPageController,} from "../controller";
import FormViewCreateAdd from "./widgets/formViewCreateAdd.vue";
@Options({
    name: "_PageModelsStudentCreate",  
    components: {
    FormViewCreateAdd,
  },
})
export default class extends Vue {

    readonly ExStudentPageController = ExStudentPageController;

    
    get queryKey() {
      return "modelsstudentcreate";
    }

    
    refreshGridStudent() {
        this.$emit('refreshGrid');
    }
    created() {}
    mounted() {}
}
</script>
<style lang="less">
</style>

