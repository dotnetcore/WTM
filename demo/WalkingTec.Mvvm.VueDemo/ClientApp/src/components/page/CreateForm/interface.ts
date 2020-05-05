import { ElForm } from "element-ui/types/form";

export interface ICreateFormOptions {
  formProps: ElForm;
  formItem: { [key: string]: FormItem };
}

export interface HiddenFun {
  /**
   * 动态判断 是否隐藏
   *
   * @param data 表单数据
   * @param status 表单状态 add/edit/daile
   */
  (data: object, status: string): boolean;
}

export interface ChildrenItem {
  Value: any;
  Text: String;
  [key: string]: object;
}

export interface FormItem {
  /**
   * 组件
   */
  type?: string;
  /**
   * 标签文案
   */
  label?: string;
  /**
   * 校验
   */
  rules?: object;
  /**
   * el-form-item span
   */
  span?: number | string;
  /**
   * 组件children项
   */
  children?: ChildrenItem[];
  /**
   * 默认值
   */
  defaultValue?: any;
  /**
   * type 组件 props
   */
  props?: object;
  /**
   * 组件数据结构 转换 接口组件数据结构 => { [mapKey]:value }
   */
  mapKey?: string;
  /**
   * 自定义 slot 数据，调用方 定义 mergeFormData
   */
  slotKey?: string;
  /**
   * 事件
   */
  events?: object;
  /**
   * 是否隐藏组件
   */
  isHidden?: boolean | HiddenFun;
}
