import { UserModule } from "@/store/modules/user";
import config from "@/config/index";
import { DirectiveOptions } from "vue";

// 按钮action权限
const visible: DirectiveOptions = {
  inserted: (el, binding, vnode) => {
    if (config.development) {
      return;
    }
    if (_.isArray(binding.value)) {
      const fslist = _.filter(binding.value, item =>
        UserModule.actionList.includes(item)
      );
      if (fslist.length < 1) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    } else {
      if (!UserModule.actionList.includes(_.toLower(binding.value))) {
        el.parentNode && el.parentNode.removeChild(el);
      }
    }
  }
};
export default visible;
