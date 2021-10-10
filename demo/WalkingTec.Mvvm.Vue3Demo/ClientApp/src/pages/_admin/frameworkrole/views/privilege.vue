<template>
  <WtmDetails queryKey="privilege" :loading="loading" :onFinish="onFinish">
    <a-list item-layout="horizontal" :dataSource="dataSource">
      <template #renderItem="{ item }">
        <a-list-item :class="`list-item-` + item.Level">
          <a-list-item-meta>
            <template #title>
              <div>
                <a-checkbox
                  :checked="checkAll(item)"
                  :indeterminate="indeterminate(item)"
                  @change="onCheckAllChange(item)"
                >
                  <span v-text="item.Name"></span>
                </a-checkbox>
              </div>
            </template>
            <template #description>
              <a-checkbox-group
                v-model:value="item.Actions"
                :name="item.ID"
                :options="getOptions(item.AllActions)"
              />
            </template>
          </a-list-item-meta>
        </a-list-item>
      </template>
    </a-list>
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
    Entity: {}
  };
  loading = true;
  dataSource = [];
  get queryKey() {
    return "privilege";
  }
  created() {}
  async onLoading() {
    try {
      this.loading = true;
      const res = await this.PageController.onGetPrivilege(this.ID);
      this.dataSource = res.Pages;
      this.formState.Entity = res.Entity;
      console.log("LENG ~ extends ~ onLoading ~ onLoading", res, this);
    } catch (error) {}
    this.loading = false;
  }
  /**
   * 传递给 details 组件的 提交函数 返回一个 Promise
   * @param values
   * @returns
   */
  async onFinish(values) {
    return this.PageController.onSavePrivilege(
      this.lodash.assign({}, this.formState, { Pages: this.dataSource })
    );
  }
  checkAll(event) {
    return this.lodash.eq(
      this.lodash.size(event.Actions),
      this.lodash.size(event.AllActions)
    );
  }
  indeterminate(event) {
    if (!this.lodash.size(event.Actions)) {
      return false;
    }
    return this.lodash.size(event.Actions) < this.lodash.size(event.AllActions);
  }
  onCheckAllChange(event) {
    if (this.checkAll(event)) {
      return (event.Actions = []);
    }
    event.Actions = this.lodash.map(event.AllActions, "Value");
  }
  getOptions(AllActions) {
    return this.lodash.map(AllActions, item => ({
      label: item.Text,
      value: item.Value
    }));
  }
  mounted() {
    this.onLoading();
  }
}
</script>

<style lang="less">
.list-item {
  &-1 {
    padding-left: 30px;
  }
  &-2 {
    padding-left: 60px;
  }
  &-3 {
    padding-left: 90px;
  }
}
</style>
