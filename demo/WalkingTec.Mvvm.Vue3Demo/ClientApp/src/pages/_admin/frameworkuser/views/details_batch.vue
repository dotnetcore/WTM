<template>
  <WtmDetails :loading="Entities.loading" :onFinish="onFinish">
    <WtmField entityKey="LinkedVM_SelectedRolesCodes" />
    <template #button>
      <a-button type="primary" html-type="submit">
        <template v-slot:icon>
          <SaveOutlined />
        </template>
        提交按钮
      </a-button>
      <a-divider type="vertical" />
      <a-button @click.stop.prevent="__wtmBackDetails()">
        <template v-slot:icon>
          <RedoOutlined />
        </template>
        关闭
      </a-button>
    </template>
  </WtmDetails>
</template>
<script lang="ts">
import { PageDetailsBasics } from "@/components";
import { Inject, mixins, Options, Provide } from "vue-property-decorator";
import { PageController } from "../controller";
@Options({ components: {} })
export default class extends mixins(PageDetailsBasics) {
  @Inject() readonly PageController: PageController;
  @Provide({ reactive: true }) formState = {
    LinkedVM: {
      SelectedRolesCodes: []
    }
    };
    async onFinish(values) {
        const Ids = this.lodash.map(this.PageController.Pagination.selectionDataSource, this.PageController.key)
        if (this.lodash.size(Ids)) {
            return this.PageController.onBatchUpdate(this.lodash.assign({ Ids }, this.formState))
        }
    }
  created() {}
  mounted() {
    this.onLoading();
  }
}
</script>
<style lang="less"></style>
