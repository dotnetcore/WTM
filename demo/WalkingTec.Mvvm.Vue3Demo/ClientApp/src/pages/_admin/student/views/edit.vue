
<template>
  <div>
    <FormViewEditEdit @refreshGrid="refreshGridStudent" />

  </div>

</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { ExStudentPageController,} from "../controller";
import FormViewEditEdit from "./widgets/formViewEditEdit.vue";
@Options({
    name: "_PageModelsStudentEdit",  
    components: {
    FormViewEditEdit,
  },
})
export default class extends Vue {

    readonly ExStudentPageController = ExStudentPageController;

    
    get queryKey() {
      return "modelsstudentedit";
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

