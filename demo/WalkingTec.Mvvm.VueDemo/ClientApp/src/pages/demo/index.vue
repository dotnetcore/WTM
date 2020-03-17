<template>
  <div>
    json创建form:
    <el-button type="primary" @click="onTest">切换status</el-button>
    <el-button type="primary" @click="onResetFields">清空</el-button>
    <el-button @click="testSubmit">提交</el-button>
    <el-button @click="testFormItem">表单项</el-button>
    <wtm-create-form ref="wForm" :status="status" :options="options">
      <template #skey="data">
        <span v-if="data.status === $actionType.detail">{{
          JSON.stringify(data)
        }}</span>
        <el-input-number
          v-else
          v-model="num"
          :min="1"
          :max="10"
        ></el-input-number>
      </template>
    </wtm-create-form>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";

@Component({
  name: "demo",
  components: {}
})
export default class extends Vue {
  form = {};
  status = "add";
  num = 2;
  options = {
    // 表单属性 非必需
    formProps: {
      "label-width": "80px"
    },
    // 表单项组件数据 必需
    formItem: {
      accout: {
        type: "input",
        label: "账号",
        span: 24,
        props: {
          placeholder: "请输入账号...",
          "suffix-icon": "el-icon-date"
        },
        rules: {
          required: true,
          message: "请输入账号",
          trigger: "blur"
        }
      },
      pwd: {
        type: "input",
        label: "密码",
        props: {
          placeholder: "请输入密码...",
          "show-password": true
        },
        rules: {
          required: true,
          message: "请输入密码",
          trigger: "blur"
        }
      },
      city: {
        type: "select",
        label: "城市",
        children: [
          {
            Text: "天津",
            Value: "tianjin"
          },
          {
            Text: "深圳",
            Value: "shenzhen",
            disabled: true
          },
          {
            Text: "韶关",
            Value: "shaoguan"
          },
          {
            Text: "清空",
            Value: "shaoguan"
          }
        ],
        events: {
          change: this.onChange
        }
      },
      citys: {
        type: "select",
        label: "城市二级",
        children: []
      },
      gender: {
        type: "radioGroup",
        label: "性别",
        children: [
          {
            Value: 0,
            Text: "男"
          },
          {
            Value: 1,
            Text: "女"
          }
        ],
        rules: {
          required: true,
          message: "请选择性别",
          trigger: "change"
        }
      },
      phone: {
        type: "input",
        label: "手机",
        props: {
          placeholder: "请输入手机号..."
        },
        rules: {
          required: true,
          trigger: "blur",
          validator(rule, value, callback) {
            if (value === "") {
              callback(new Error("请输入手机号"));
            } else if (value.length != 11) {
              callback(new Error("请输入正确的手机号"));
            } else {
              callback();
            }
          }
        }
      },
      msg: {
        type: "input",
        label: "留言",
        props: {
          placeholder: "留言...",
          type: "textarea"
        },
        rules: {
          required: true,
          trigger: "blur",
          message: "请留下您宝贵的意见"
        }
      },
      imgid: {
        type: "wtmUploadImg",
        label: "上传图片"
      },
      testsole: {
        type: "wtmSlot",
        label: "自定义",
        slotKey: "skey"
      }
    }
  };
  created() {}
  onTest() {
    this.status = this.status === "add" ? "detail" : "add";
  }
  onResetFields() {
    this.$refs.wForm.resetFields();
  }
  testSubmit() {
    console.log("formData", JSON.stringify(this.$refs.wForm.formData));
  }
  testFormItem() {
    const item = this.$refs.wForm.getFormItem("accout");
    item.showError("错误信息");
  }
  onChange(e) {
    if (e === "qingkong") {
      this.options.formItem["citys"].children = [];
    } else {
      this.options.formItem["citys"].children = [
        {
          Text: "天津2",
          Value: "tianjin2"
        },
        {
          Text: "深圳2",
          Value: "shenzhen2",
          disabled: true
        },
        {
          Text: "韶关2",
          Value: "shaoguan2"
        }
      ];
    }
  }
}
</script>

<style lang="less" scoped></style>
