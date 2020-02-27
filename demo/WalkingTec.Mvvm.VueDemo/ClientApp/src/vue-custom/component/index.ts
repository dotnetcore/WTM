// 内容布局
import Card from "@/components/layout/components/Card/index.vue";
// 弹出框
import DialogBox from "@/components/page/dialog/dialog-box.vue";
// 查询
import SearchBox from "@/components/page/search-box.vue";
// 列表
import TableBox from "@/components/page/table-box.vue";
// 操作按钮
import ButBox from "@/components/page/but-box.vue";
// 表单项
import FormItem from "@/components/page/form-item.vue";
export default [
  { key: "Card", value: Card },
  { key: "WtmDialogBox", value: DialogBox },
  { key: "WtmSearchBox", value: SearchBox },
  { key: "WtmTableBox", value: TableBox },
  { key: "WtmButBox", value: ButBox },
  { key: "WtmFormItem", value: FormItem }
];
