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
    const actions = UserModule.actionList;
    if (_.isArray(value)) {
      const fslist = _.filter(value, item => actions.includes(item));
      if (fslist.length < 1) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    } else {
      // console.log("binding.value", value);
      // console.log(!actions.includes(value));
      if (!actions.includes(value)) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    }
  }
};
export default visible;
