import { UserModule } from "@/store/modules/user";
import config from "@/config/index";
import { DirectiveOptions } from "vue";
/**
 * 按钮 action权限
 */
const visible: DirectiveOptions = {
  inserted: (el, { value }, vnode) => {
    if (config.development) {
      return;
    }
    const actions = UserModule.actionList.map(item => item.toUpperCase());
    if (_.isArray(value)) {
      const fslist = _.filter(
        value,
        item => !!item && actions.includes(item.toUpperCase())
      );
      if (fslist.length < 1) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    } else {
      console.log("value", value && value.toUpperCase());
      if (value && !actions.includes(value.toUpperCase())) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    }
  }
};
export default visible;
