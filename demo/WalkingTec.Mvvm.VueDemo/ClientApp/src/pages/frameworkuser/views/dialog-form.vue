<template>
  <wtm-dialog-box :is-show.sync="isShow" :status="status" :events="formEvent">
    <wtm-create-form
      :ref="refName"
      :status="status"
      :options="formOptions"
    ></wtm-create-form>
  </wtm-dialog-box>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Action, State } from "vuex-class";
import formMixin from "@/vue-custom/mixin/form-mixin";
import { sexList } from "@/config/entity";
import UploadImg from "@/components/page/UploadImg.vue";

@Component({
  mixins: [formMixin()]
})
export default class Index extends Vue {
  @Action
  getFrameworkRoles;
  @Action
  getFrameworkGroups;
  @State
  getFrameworkRolesData;
  @State
  getFrameworkGroupsData;
  // 表单结构
  get formOptions() {
    const filterMethod = (query, item) => {
      return item.label.indexOf(query) > -1;
    };
    return {
      formProps: {
        "label-width": "100px"
      },
      formItem: {
        "Entity.ID": {
          isHidden: true
        },
        "Entity.ITCode": {
          type: "input",
          label: "账号",
          rules: {
            required: true,
            message: "请输入账号",
            trigger: "blur"
          }
        },
        "Entity.Password": {
          type: "input",
          label: "密码",
          rules: {
            required: true,
            message: "请输入账号",
            trigger: "blur"
          },
          isHidden: (res, status) => status === "edit"
        },
        "Entity.Email": {
          type: "input",
          label: "邮箱"
        },
        "Entity.Name": {
          type: "input",
          label: "姓名",
          rules: {
            required: true,
            message: "请输入姓名",
            trigger: "blur"
          }
        },
        "Entity.Sex": {
          type: "select",
          label: "性别",
          children: sexList
        },
        "Entity.CellPhone": {
          type: "input",
          label: "手机号"
        },
        "Entity.HomePhone": {
          type: "input",
          label: "座机"
        },
        "Entity.Address": {
          type: "input",
          label: "住址"
        },
        "Entity.ZipCode": {
          type: "input",
          label: "邮编"
        },
        "Entity.PhotoId": {
          type: "wtmUploadImg",
          label: "头像",
          props: {
            isHead: true,
            imageStyle: { width: "100px", height: "100px" }
          }
        },
        "Entity.IsValid": {
          type: "switch",
          label: "是否有效",
          defaultValue: true
        },
        "Entity.UserRoles": {
          type: "transfer",
          label: "角色",
          mapKey: "RoleId",
          props: {
            data: this.getFrameworkRolesData.map(item => ({
              key: item.Value,
              label: item.Text
            })),
            titles: ["所有", "已选"],
            filterable: true,
            filterMethod: filterMethod,
            "filter-placeholder": "请输入角色"
          },
          span: 24,
          defaultValue: []
        },
        "Entity.UserGroups": {
          type: "transfer",
          label: "用户组",
          mapKey: "GroupId",
          props: {
            data: this.getFrameworkGroupsData.map(item => ({
              key: item.Value,
              label: item.Text
            })),
            titles: ["所有", "已选"],
            filterable: true,
            filterMethod: filterMethod,
            "filter-placeholder": "请输入用户组"
          },
          span: 24,
          defaultValue: []
        }
      }
    };
  }
  created() {
    this.getFrameworkRoles();
    this.getFrameworkGroups();
  }
}
</script>
