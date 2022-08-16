<template>
  <WtmDetails :queryKey="queryKey" :loading="Entities.loading" :onFinish="onFinish">
    <div style="margin-bottom:10px;"><i18n-t keypath="批量修改以下数据" /></div>
      <a-row :gutter="6">
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_Password"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_Email"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_Name"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_Sex"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_CellPhone"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_Address"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_ZipCode"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_IsValid"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_EnRollDate"/>
        </a-col>
        <a-col :span="12">
        <WtmField entityKey="LinkedVM_SelectedMajorStudent_MT_WtmsIDs"/>
        </a-col>
      </a-row>
      <div style="text-align:right;">
      </div>
  <template #button>
  
   <a-divider type="vertical" />
   <a-button type="primary" html-type="submit">
       <template v-slot:icon>
           <SaveOutlined style='margin-right:5px'/>
       </template>
       <i18n-t keypath="提交" />
   </a-button>
   <a-divider type="vertical" />
   <a-button type="primary" @click.stop.prevent="__wtmBackDetails()">
       <template v-slot:icon>
           <RedoOutlined style='margin-right:5px'/>
       </template>
       <i18n-t keypath="关闭" />
   </a-button>
  </template>
  </WtmDetails>
</template>

<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { StudentPageController,ExStudentPageController,ExStudentEntity } from "../../controller";
import {$locales, $i18n} from "@/client";

@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Provide({ reactive: true }) readonly PageController = ExStudentPageController;
  @Provide({ reactive: true }) readonly PageEntity = ExStudentEntity;
  @Provide({ reactive: true }) formState = {
      LinkedVM: {
        Password: undefined,
        Email: undefined,
        Name: undefined,
        Sex: undefined,
        CellPhone: undefined,
        Address: undefined,
        ZipCode: undefined,
        IsValid: undefined,
        EnRollDate: undefined,
        SelectedMajorStudent_MT_WtmsIDs: undefined,
    },
  };
  
  async onFinish(values) {
      if (this.lodash.get(this.$route.query,'ids')) {
          let Ids = this.$route.query.ids
          Ids = JSON.stringify(Ids).replaceAll('"','').split("|")
          await this.PageController.onBatchUpdate(this.lodash.assign({ Ids }, this.formState));
          await this.onRefreshGrid();
      }
  }
  
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>

