<template>
  <a-form class="page-search-form" :form="form" @submit="handleSearch">
    <a-row :gutter="24">
      <!-- <a-col v-for="i in 10" :key="i" :span="8" :style="{ display: i < count ? 'block' : 'none' }">
        <a-form-item :label="`Field ${i}`">
          <a-input
            v-decorator="[
                `field-${i}`
              ]"
            placeholder="placeholder"
          />
        </a-form-item>
      </a-col>-->
      <a-col :span="8">
        <a-form-item :label="`ITCode`">
          <a-input
            v-decorator="[
                `ITCode`
              ]"
            placeholder="placeholder"
          />
        </a-form-item>
      </a-col>
      <a-col :span="8">
        <a-form-item :label="`ActionUrl`">
          <a-input
            v-decorator="[
                `ActionUrl`
              ]"
            placeholder="placeholder"
          />
        </a-form-item>
      </a-col>
    </a-row>
    <a-row>
      <a-col :span="24" :style="{ textAlign: 'right' }">
        <a-button type="primary" html-type="submit" :disabled="PageStore.Loading">Search</a-button>
        <a-button
          :style="{ marginLeft: '8px' }"
          :disabled="PageStore.Loading"
          @click="handleReset"
        >Clear</a-button>
        <a :style="{ marginLeft: '8px', fontSize: '12px' }" @click="toggle">
          Collapse
          <a-icon :type="PageStore.FilterCollapse ? 'up' : 'down'" />
        </a>
      </a-col>
    </a-row>
  </a-form>
</template>
<script lang="ts">
// import { observer } from "mobx-vue";
// import { Component, Prop, Vue } from "vue-property-decorator";
// import PageStore from "../store";
// @observer
// @Component
// export default class ViewFilter extends Vue {
//   @Prop() private PageStore!: PageStore;
//   form = this.$form.createForm(this, {});
//   get count() {
//     return this.PageStore.FilterCollapse ? 11 : 7;
//   }
//   // get expand() {
//   //   return this.PageStore.FilterCollapse;
//   // }
//   mounted() {
//     console.log("TCL: ViewFilter -> PageStore", this.PageStore);
//   }
//   handleSearch(e) {
//     e.preventDefault();
//     this.form.validateFields((error, values) => {
//       console.log("error", error);
//       console.log("Received values of form: ", values);
//     });
//   }
//   handleReset() {
//     this.form.resetFields();
//   }
//   toggle() {
//     this.PageStore.onToggleFilterCollapse();
//   }
// }
export default {
  props: ["PageStore"],
  data() {
    return {
      expand: false,
      form: this.$form.createForm(this, { name: "advanced_search" })
    };
  },
  computed: {
    count() {
      return this.PageStore.FilterCollapse ? 11 : 7;
    }
  },
  mounted() {
    this.PageStore.EventSubject.next({
      EventType: "onSearch",
      AjaxRequest: {
        body: {
          Page: 1,
          Limit: 20
        }
      }
    });
  },
  methods: {
    onSearch(body) {
      this.PageStore.EventSubject.next({
        EventType: "onSearch",
        AjaxRequest: {
          body: {
            ...body,
            Page: 1,
            Limit: 20
          }
        }
      });
    },
    handleSearch(e) {
      e.preventDefault();
      this.form.validateFields((error, values) => {
        this.onSearch(values);
      });
    },

    handleReset() {
      this.form.resetFields();
      this.onSearch({});
    },

    toggle() {
      this.PageStore.onToggleFilterCollapse();
    }
  }
};
</script>
<style scoped lang="less">
</style>