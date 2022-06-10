<template>
  <template v-if="_readonly">
      <span>{{readonlyText}}</span>
  </template>
  <template v-else>
      <a-tree-select
        v-model:value="value"
        style="width: 100%"
        :tree-data="dataSource"
        :multiple="multiple ? true : false"
        :tree-checkable = "multiple ? true : false"
        allow-clear
        :placeholder="_placeholder"
        search-placeholder="Please select"
      />
   </template>
</template>
<script lang="ts">
    import { Vue, Options, Watch, mixins, Inject } from "vue-property-decorator";
    import { $System, globalProperties } from "@/client";
    import { FieldBasics } from "../script";
    @Options({ components: {} })
    export default class extends mixins(FieldBasics) {
        // 表单状态值
        @Inject() readonly formState;
        // 自定义校验状态
        @Inject() readonly formValidate;
        // 实体
        @Inject() readonly PageEntity;
        // 表单类型
        @Inject({ default: "" }) readonly formType;
        get readonlyText() {
          let that = this
          let valueList = []
          if(this.lodash.isArray(this.value)){
            this.lodash.filter(this.value,function (item) {
                valueList.push(that.getText(that.dataSource,item))
            })

            return valueList.join(",")
          }else{
            return that.getText(that.dataSource,this.value)
          }
        }
        previewUrl = "";
        previewVisible = false;
        get value() {
            return this.lodash.get(this.formState, this._name);
        }
        get multiple() {
            return this.lodash.get(this._fieldProps, "multiple", false);
        }
        set value(value) {
            this.lodash.set(this.formState, this._name, value);
        }
        fileList = [];
        filedata = [];
        async mounted() {
            this.onRequest();
            console.log(this.dataSource)
        }
        @Watch("value")
        onValueChange(val, old) {
            console.log(val)
        }

        getText(treeData,value) {
            var arrnew = null;
            let convert = (arr) => {
              arr.filter((item) => {
                if (item.value == value) {
                    arrnew = item.title;
                } else {
                  if (item.children) {
                    return convert(item.children);
                  }
                }
              });
            };
            convert(treeData);
            return arrnew;
      }
    }

</script>
